using ClubNet.Models;
using ClubNet.Models.DTO;
using ClubNet.Services.Handlers;
using ClubNet.Services.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;

namespace ClubNet.Services
{
    public class LoginService : ILoginRepository
    {
        private readonly IConfiguration _config;

        public LoginService(IConfiguration config)
        {
            _config = config;
        }

        public ApiResponse<string> Login(LoginDTO login)
        {
            var loginResult = new ApiResponse<string>();

            // 1) Obtener hash, estado, rol_id y nombre en una sola consulta con JOIN
            string queryAll = "SELECT u.Clave, p.estado, p.rol_id, p.nombre " +
                              "FROM Usuarios u " +
                              "INNER JOIN rel_usuarios_personas rup ON rup.user_id = u.user_id " +
                              "INNER JOIN personas p ON p.persona_id = rup.persona_id " +
                              "WHERE u.Email = @email";

            // GetDt para manejar múltiples campos como 'estado'
            DataTable userDataTable = PostgresHandler.GetDt(queryAll, ("email", login.Email));

            if (userDataTable == null || userDataTable.Rows.Count == 0)
            {
                loginResult.Success = false;
                loginResult.Message = "Usuario inexistente.";
                return loginResult;
            }

            DataRow userRow = userDataTable.Rows[0];
            string? hash = userRow["clave"]?.ToString();
            bool estado = (bool)userRow["estado"]; // Casteamos el estado
            string? rol = userRow["rol_id"]?.ToString();
            string? nombre = userRow["nombre"]?.ToString();

            // 2) VERIFICACIÓN DE ESTADO: No permitir login si estado es false
            if (!estado)
            {
                loginResult.Success = false;
                loginResult.Message = "El usuario se encuentra inactivo. Contacte a un administrador.";
                return loginResult;
            }

            // 3) Verificar contraseña
            bool validPassword = BCrypt.Net.BCrypt.Verify(login.Clave, hash);
            if (!validPassword)
            {
                loginResult.Success = false;
                loginResult.Message = "Credenciales inválidas.";
                return loginResult;
            }

            // 4) Generar JWT (si pasó todas las verificaciones)
            string token;
            try
            {
                token = GenerateJwtToken(login.Email, rol!, nombre!);
            }
            catch (Exception ex)
            {
                loginResult.Success = false;
                loginResult.Message = "Error generando token: " + ex.Message;
                return loginResult;
            }

            // 5) Devolver respuesta con token en Data
            loginResult.Success = true;
            loginResult.Message = "Usuario validado correctamente.";
            loginResult.Data = token;

            return loginResult;
        }

        private string GenerateJwtToken(string email, string rol, string nombre)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email ?? string.Empty),
                new Claim(ClaimTypes.Role, rol ?? string.Empty),
                new Claim(ClaimTypes.Name, nombre ?? string.Empty)
            };

            var keyString = _config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key no configurado en appsettings.json");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            double minutes = 60;
            if (!double.TryParse(_config["Jwt:ExpiresInMinutes"], out minutes))
            {
                minutes = 60;
            }

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(minutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ApiResponse Register(RegisterDTO register)
        {
            ApiResponse result = new ApiResponse();
            string query = "CALL public.SP_ALTA_USUARIO(@p_email::varchar,@p_clave::varchar,@p_nombre::varchar,@p_apellido::varchar,@p_dni,@p_rol)";
            bool resultExec = PostgresHandler.Exec(query,
                ("p_email", register.Email),
                ("p_clave", BCrypt.Net.BCrypt.HashPassword(register.Clave)),
                ("p_nombre", register.Nombre),
                ("p_apellido", register.Apellido),
                ("p_dni", register.Dni),
                ("p_rol", 3)
            );

            result.Success = resultExec;
            result.Message = "Usuario registrado correctamente.";
            if (!resultExec)
                result.Message = "Ocurrio un problema al registrar el usuario, contacte al administrador.";

            return result;
        }

        public ApiResponse RecuperarClave(string email)
        {
            var response = new ApiResponse();

            // 1. Verificar si el usuario existe
            string queryCheck = "SELECT COUNT(*) FROM usuarios WHERE email = @email";
            string count = PostgresHandler.GetScalar(queryCheck, ("email", email));

            if (count == "0")
            {
                // Por seguridad, a veces se dice "Si el correo existe, se envió", 
                // pero para desarrollo diremos que no existe.
                response.Success = false;
                response.Message = "El correo no se encuentra registrado.";
                return response;
            }

            // 2. Generar nueva contraseña temporal
            string nuevaClave = GenerarClaveRandomd(8);
            string nuevoHash = BCrypt.Net.BCrypt.HashPassword(nuevaClave);

            // 3. Actualizar en Base de Datos
            string queryUpdate = "UPDATE usuarios SET clave = @clave WHERE email = @email";
            bool dbResult = PostgresHandler.Exec(queryUpdate, ("clave", nuevoHash), ("email", email));

            if (!dbResult)
            {
                response.Success = false;
                response.Message = "Error al actualizar la contraseña.";
                return response;
            }

            // 4. Enviar Email
            bool emailSent = SendRecoveryEmail(email, nuevaClave);

            if (emailSent)
            {
                response.Success = true;
                response.Message = "Se ha enviado una nueva contraseña a tu correo.";
            }
            else
            {
                response.Success = false;
                // Nota: La contraseña ya se cambió en DB, esto podría ser un problema si el mail falla.
                // Idealmente se debería hacer rollback, pero para este MVP está bien.
                response.Message = "La contraseña se restableció, pero falló el envío del correo. Contacta al admin.";
            }

            return response;
        }

        private string GenerarClaveRandomd(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private bool SendRecoveryEmail(string destinatario, string nuevaClave)
        {
            try
            {
                // 1. Leer configuración desde el .env (o appsettings)
                string fromEmail = _config["Email:SenderAddress"];
                string fromName = _config["Email:SenderName"];
                string fromPassword = _config["Email:SenderPassword"];

                // Validar que la configuración exista para evitar errores raros
                if (string.IsNullOrEmpty(fromEmail) || string.IsNullOrEmpty(fromPassword))
                {
                    Console.WriteLine("Error: Faltan configuraciones de Email en el .env");
                    return false;
                }

                // 2. Usar las variables
                var fromAddress = new MailAddress(fromEmail, fromName);
                var toAddress = new MailAddress(destinatario);
                const string subject = "Recuperación de Contraseña - ClubNet";

                // ... resto del cuerpo del correo ...
                string body = $@"
            <h1>Recuperación de Contraseña</h1>
            <p>Hola,</p>
            <p>Has solicitado recuperar tu contraseña. Tu nueva contraseña temporal es:</p>
            <h2 style='color: #2563eb;'>{nuevaClave}</h2>
            <p>Por favor, inicia sesión y cámbiala lo antes posible.</p>";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    // Usar la variable here
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    smtp.Send(message);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error enviando mail: " + ex.Message);
                return false;
            }
        }

        public ApiResponse CambiarClave(string email, CambiarClaveDTO datos)
        {
            var response = new ApiResponse();

            // 1. Buscar la clave actual del usuario
            string queryGet = "SELECT u.clave FROM usuarios u WHERE u.email = @email";
            DataTable table = PostgresHandler.GetDt(queryGet, ("email", email));

            if (table.Rows.Count == 0)
            {
                response.Success = false;
                response.Message = "Usuario no encontrado.";
                return response;
            }

            string hashActual = table.Rows[0]["clave"].ToString();

            // 2. Verificar que la contraseña actual ingresada coincida con el hash
            bool passwordEsValida = BCrypt.Net.BCrypt.Verify(datos.ClaveActual, hashActual);

            if (!passwordEsValida)
            {
                response.Success = false;
                response.Message = "La contraseña actual es incorrecta.";
                return response;
            }

            // 3. Generar nuevo hash y actualizar
            string nuevoHash = BCrypt.Net.BCrypt.HashPassword(datos.NuevaClave);
            string queryUpdate = "UPDATE usuarios SET clave = @clave WHERE email = @email";

            bool result = PostgresHandler.Exec(queryUpdate, ("clave", nuevoHash), ("email", email));

            response.Success = result;
            response.Message = result ? "Contraseña actualizada correctamente." : "Error al actualizar en base de datos.";

            return response;
        }


    }
}