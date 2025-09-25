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
            string query = $"SELECT COUNT (*) as Existe FROM Usuarios WHERE Email = @email and Clave = @clave";
            string resultado = PostgresHandler.GetScalar(query, ("email", login.Email), ("@clave", login.Clave));

            ApiResponse loginResult = new ApiResponse();

            if (resultado == "0")
            {
                loginResult.Success = false;
                loginResult.Message = "Credenciales invalidas o usuario inexistente.";
            }
            else
            {
                loginResult.Success = true;
                loginResult.Message = "Usuario validado correctamente.";
                //loginResult.Id = Convert.ToInt32(PostgresHandler.Exec($"SELECT id FROM Usuarios WHERE Email = '{login.Email}' AND Clave = '{login.Clave}'"));
            }

            return loginResult;
        }

        public async Task<ApiResponse> Register(RegisterDTO register)
        {
            //TO DO: corregir los mensajes de error
            ApiResponse response = new ApiResponse();

            string queryUsuario = "INSERT INTO Usuarios(email,clave) VALUES (@email, @clave);";
            bool resultUsuario = PostgresHandler.Exec(queryUsuario, ("email", register.Email), ("clave",register.Clave));

            if(!resultUsuario)
            {
                response.Success = false;
                response.Message = "Ocurrio un problema al generar el usuario para ingresar al sistema";
            }
            else
            {
                string queryPersona = $"INSERT INTO Personas(dni,nombre,apellido,fealta,estado,rol_id) " +
                $"VALUES (@dni,@nombre,@apellido,NOW(),@estado,@rol);";
                bool resultPersona = PostgresHandler.Exec(queryPersona, ("dni", register.Dni), ("nombre", register.Nombre),
                    ("apellido", register.Apellido), ("estado", true), ("rol", register.Rol));
                
                if(!resultPersona)
                {
                    response.Success = false;
                    response.Message = "Ocurrio un problema al crear el usuario";
                }
                else
                {
                    response.Success= true;
                    response.Message=string.Empty;
                }
            }

            return response;
        }

    }
}
