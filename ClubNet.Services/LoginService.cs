using ClubNet.Models.DTO;
using ClubNet.Services.Handlers;
using ClubNet.Services.Repositories;

namespace ClubNet.Services
{
    public class LoginService : ILoginRepository
    {
        public async Task<LoginResultDTO> Login(LoginDTO login)
        {
            string query = $"select count (*) as Existe from Usuarios where Email = '{login.Email}' and Clave = '{login.Clave}'";
            string resultado = PostgresHandler.Exec(query);

            LoginResultDTO loginResult = new LoginResultDTO();

            if (resultado == "0")
            {
                loginResult.Result = false;
                loginResult.Mensaje = "Credenciales invalidas o usuario inexistente.";
            }
            else
            {
                loginResult.Result = true;
                loginResult.Mensaje = "Usuario validado correctamente.";
                loginResult.Id = Convert.ToInt32(PostgresHandler.Exec($"SELECT id FROM Usuarios WHERE Email = '{login.Email}' AND Clave = '{login.Clave}'"));
            }

            return loginResult;
        }

        public async Task<bool> Register(Login register)
        {
            if (register.Email == "" || register.clave == "")
            {
                return false;
            }

            string query = $"INSERT INTO Usuarios VALUES (null, '{register.Email}', '{register.Clave}');";
            return PostgresHandler.Exec(query);
        }

        public async Task<List<Login>> GetUsuarios()
        {
            string query = "SELECT * FROM Usuarios";
            string json = PostgresHandler.Exec(query);
            return JsonConvert.DeserializeObject<List<Login>>(json);
        }
    }
}
