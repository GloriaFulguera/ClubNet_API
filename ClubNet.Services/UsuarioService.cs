
using ClubNet.Models;
using ClubNet.Models.DTO;
using ClubNet.Services.Handlers;
using ClubNet.Services.Repositories;
using Newtonsoft.Json;

namespace ClubNet.Services
{
    public class UsuarioService: IUsuariosRepository
    {
        public async Task<UsuarioDTO> GetUsuarioById(int id)
        {
            string query = $"SELECT p.persona_id,p.nombre,p.apellido,p.dni,u.email,p.estado,p.rol_id FROM personas p " +
                $"INNER JOIN rel_usuarios_personas up on up.persona_id = p.persona_id " +
                $"LEFT JOIN usuarios u on u.user_id = up.user_id " +
                $"WHERE p.persona_id = @id;";

            string result = PostgresHandler.GetJson(query, ("id", id));

            List<UsuarioDTO> usuario = JsonConvert.DeserializeObject<List<UsuarioDTO>>(result);

            return usuario.FirstOrDefault();
        }

        public async Task<List<Rol>> GetRoles()
        {
            string query = "SELECT * FROM roles ORDER BY rol_id ASC;";
            string result = PostgresHandler.GetJson(query);
            List<Rol> roles = JsonConvert.DeserializeObject<List<Rol>>(result);
            return roles;
        }
    }
}
