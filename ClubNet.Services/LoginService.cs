using ClubNet.Models;
using ClubNet.Models.DTO;
using ClubNet.Services.Handlers;
using ClubNet.Services.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
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

            // 1) Obtener hash de la DB (usando PostgresHandler)
            string queryHash = "SELECT Clave FROM Usuarios WHERE Email = @email";
            string? hash = PostgresHandler.GetScalar(queryHash, ("email", login.Email));

            if (hash == null)
            {
                loginResult.Success = false;
                loginResult.Message = "Usuario inexistente.";
                return loginResult;
            }

            // 2) Verificar contraseña
            bool validPassword = BCrypt.Net.BCrypt.Verify(login.Clave, hash);
            if (!validPassword)
            {
                loginResult.Success = false;
                loginResult.Message = "Credenciales inválidas.";
                return loginResult;
            }

            // 3) Obtener rol y nombre
            string queryRol = "SELECT Rol FROM Usuarios WHERE Email = @email";
            string? rol = PostgresHandler.GetScalar(queryRol, ("email", login.Email));
            if (rol == null) rol = "0";

            string queryNombre = "SELECT Nombre FROM Usuarios WHERE Email = @email";
            string? nombre = PostgresHandler.GetScalar(queryNombre, ("email", login.Email)) ?? string.Empty;

            // 4) Generar JWT
            string token;
            try
            {
                token = GenerateJwtToken(login.Email, rol, nombre);
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
    }
}
