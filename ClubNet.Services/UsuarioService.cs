
using ClubNet.Models;
using ClubNet.Models.DTO;
using ClubNet.Services.Handlers;
using ClubNet.Services.Repositories;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace ClubNet.Services
{
    public class UsuarioService : IUsuarioRepository
    {
        public ApiResponse<UsuarioDTO> GetUsuarioByEmail(string email)
        {
            ApiResponse<UsuarioDTO> getResult = new ApiResponse<UsuarioDTO>();
            string query = $"SELECT p.persona_id,p.nombre,p.apellido,p.dni,u.email,p.estado,p.rol_id FROM personas p " +
                $"INNER JOIN rel_usuarios_personas up on up.persona_id = p.persona_id " +
                $"LEFT JOIN usuarios u on u.user_id = up.user_id " +
                $"WHERE u.email = @email AND p.estado = true;";

            string result = PostgresHandler.GetJson(query, ("email", email));

            List<UsuarioDTO> usuario = JsonConvert.DeserializeObject<List<UsuarioDTO>>(result);
            if (usuario == null || usuario.Count == 0)
            {
                getResult.Success = false;
                getResult.Message = "Usuario no encontrado.";
            }
            else
            {
                getResult.Success = true;
                getResult.Data = usuario.FirstOrDefault();
            }
            return getResult;
        }

        public ApiResponse<List<Rol>> GetRoles()
        {
            ApiResponse<List<Rol>> getResult = new ApiResponse<List<Rol>>();
            string query = "SELECT * FROM roles ORDER BY rol_id ASC;";
            string result = PostgresHandler.GetJson(query);
            List<Rol> roles = JsonConvert.DeserializeObject<List<Rol>>(result);

            if (roles == null)
            {
                getResult.Success = false;
                getResult.Message = "Ocurrió un problema al obtener los roles.";
            }
            else
            {
                getResult.Success = true;
                getResult.Data = roles;
            }
            return getResult;
        }

        public ApiResponse UpdateUsuario(UsuarioDTO usuario)
        {
            ApiResponse result = new ApiResponse();
            string query = "UPDATE personas SET nombre = @nombre, apellido = @apellido, dni = @dni, estado = @estado, rol_id = @rol_id " +
                "WHERE persona_id = @persona_id;";
            bool success = PostgresHandler.Exec(query,
                ("nombre", usuario.Nombre),
                ("apellido", usuario.Apellido),
                ("dni", usuario.Dni),
                ("estado", usuario.Estado),
                ("rol_id", usuario.Rol_id),
                ("persona_id", usuario.Persona_id)
            );

            result.Success = success;
            if (!success)
                result.Message = "Error al actualizar el usuario.";

            return result;
        }

        public ApiResponse CreateUser(RegisterDTO usuario)
        {
            ApiResponse result = new ApiResponse();
            string query = "CALL public.SP_ALTA_USUARIO(@p_email::varchar,@p_clave::varchar,@p_nombre::varchar,@p_apellido::varchar,@p_dni,@p_rol)";
            bool resultExec = PostgresHandler.Exec(query,
                ("p_email", usuario.Email),
                ("p_clave", BCrypt.Net.BCrypt.HashPassword(usuario.Clave)),
                ("p_nombre", usuario.Nombre),
                ("p_apellido", usuario.Apellido),
                ("p_dni", usuario.Dni),
                ("p_rol", usuario.Rol)
            );

            result.Success = resultExec;
            if (!resultExec)
                result.Message = "Ocurrio un problema al registrar el usuario, contacte al administrador.";

            return result;
        }

        public ApiResponse RegisterToActivity(RegisterToActivityDTO registro)
        {
            ApiResponse result = new ApiResponse();

            string query = "CALL SP_ALTA_USUARIO_ACTIVIDAD(@p_dni,@p_actividadid)";
            bool resultExec = PostgresHandler.Exec(query,
                ("p_dni", registro.Dni),
                ("p_actividadid", registro.Actividad_id)
                );
            result.Success = resultExec;
            if (!resultExec) result.Message = "Ocurrio un problema durante la inscripción.";

            return result;
        }
    }
}
