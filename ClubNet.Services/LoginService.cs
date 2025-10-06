using ClubNet.Models;
using ClubNet.Models.DTO;
using ClubNet.Services.Handlers;
using ClubNet.Services.Repositories;

namespace ClubNet.Services
{
    public class LoginService : ILoginRepository
    {
        public async Task<ApiResponse> Login(LoginDTO login)
        {
            string query = "SELECT Clave FROM Usuarios WHERE Email = @email";
            string? hash = PostgresHandler.GetScalar(query, ("email", login.Email));

            ApiResponse loginResult = new ApiResponse();

            if (hash == null)
            {
                loginResult.Success = false;
                loginResult.Message = "Usuario inexistente.";
                return loginResult;
            }

            // Comparar la contraseña en crudo contra el hash almacenado
            bool validPassword = BCrypt.Net.BCrypt.Verify(login.Clave, hash);

            if (!validPassword)
            {
                loginResult.Success = false;
                loginResult.Message = "Credenciales inválidas.";
            }
            else
            {
                loginResult.Success = true;
                loginResult.Message = "Usuario validado correctamente.";
            }

            return loginResult;
        }


        public async Task<ApiResponse> Register(RegisterDTO register)
        {
            ApiResponse result = new ApiResponse();
            string query = "CALL public.SP_ALTA_USUARIO(@p_email::varchar,@p_calve::varchar,@p_nombre::varchar,@p_apellido::varchar,@p_dni,@p_rol)";
            bool resultExec = PostgresHandler.Exec(query,
                ("p_email", register.Email),
                ("p_calve", BCrypt.Net.BCrypt.HashPassword(register.Clave)),
                ("p_nombre", register.Nombre),
                ("p_apellido", register.Apellido),
                ("p_dni", register.Dni),
                ("p_rol", 3)
            );

            result.Success = resultExec;
            if (!resultExec)
                result.Message = "Ocurrio un problema al registrar el usuario, contacte al administrador.";

            return result;
        }

    }
}
