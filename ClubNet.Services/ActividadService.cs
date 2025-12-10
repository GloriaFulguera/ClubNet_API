using ClubNet.Models;
using ClubNet.Models.DTO;
using ClubNet.Services.Handlers;
using ClubNet.Services.Repositories;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace ClubNet.Services
{
    public class ActividadService : IActividadRepository
    {
        public ApiResponse CreateActividad(CreateActividadDTO actividad)
        {
            ApiResponse createResult = new ApiResponse();
            try
            {
                string query = @"INSERT INTO actividades(nombre,descripcion,cupo,inicio,estado,cuota_valor,url_imagen) 
                         VALUES (@nombre,@descripcion,@cupo,@inicio,@estado,@cuota_valor,@url_imagen) 
                         RETURNING actividad_id;";

                string idStr = PostgresHandler.GetScalar(query,
                ("nombre", actividad.Nombre),
                ("descripcion", actividad.Descripcion),
                ("cupo", actividad.Cupo),
                ("inicio", actividad.Inicio),
                ("estado", actividad.Estado),
                ("cuota_valor", actividad.Cuota_valor),
                ("url_imagen", actividad.Url_imagen));

                if (!int.TryParse(idStr, out int newId))
                    throw new Exception("No se pudo obtener el ID de la actividad creada.");

                // Asignar Entrenador si viene en el DTO
                if (actividad.Entrenador_id != null && actividad.Entrenador_id > 0)
                {
                    string queryEnt = "INSERT INTO asignacion_entrenadores(persona_id, actividad_id) VALUES (@persona, @actividad);";
                    PostgresHandler.Exec(queryEnt,
                        ("persona", actividad.Entrenador_id),
                        ("actividad", newId));
                }

                createResult.Success = true;
                createResult.Message = "Actividad creada correctamente.";
            }
            catch (Exception ex)
            {
                createResult.Success = false;
                createResult.Message = "Error: " + ex.Message;
            }
            return createResult;
        }

        public ApiResponse<List<GetActividadDTO>> GetActividades()
        {
            ApiResponse<List<GetActividadDTO>> getResult = new ApiResponse<List<GetActividadDTO>>();
            string query = @"
                SELECT 
                    a.actividad_id,
                    a.nombre,
                    a.descripcion,
                    a.cupo,
                    a.inicio,
                    a.cuota_valor,
                    a.estado,
                    a.url_imagen,
                    p.nombre AS ent_nombre,
                    p.apellido AS ent_apellido,
                    p.persona_id AS entrenador_id
                FROM actividades a 
                LEFT JOIN asignacion_entrenadores ae ON ae.actividad_id = a.actividad_id  
                LEFT JOIN personas p ON p.persona_id = ae.persona_id  
                ORDER BY a.actividad_id ASC;";
            string result = PostgresHandler.GetJson(query);
            List<GetActividadDTO> actividades = JsonConvert.DeserializeObject<List<GetActividadDTO>>(result);
            getResult.Data = actividades;

            if (actividades == null)
            {
                getResult.Success = false;
                getResult.Message = "Ocurrió un problema al obtener las actividades.";
            }
            else
                getResult.Success = true;

            return getResult;
        }

        public ApiResponse UpdateActividad(UpdateActividadDTO actividad)
        {
            ApiResponse updateResult = new ApiResponse();

            string query = $"UPDATE actividades SET nombre=@nombre, descripcion=@descripcion, cupo=@cupo, inicio=@inicio, " +
                $"estado=@estado, cuota_valor=@cuota_valor, url_imagen=@url_imagen WHERE actividad_id=@id";

            bool result = PostgresHandler.Exec(query,
                ("nombre", actividad.Nombre),
                ("descripcion", actividad.Descripcion),
                ("cupo", actividad.Cupo),
                ("inicio", actividad.Inicio),
                ("estado", actividad.Estado),
                ("cuota_valor", actividad.Cuota_valor),
                ("url_imagen", actividad.Url_imagen),
                ("id", actividad.Actividad_id));

            bool entrenadorResult = false;

            if (actividad.Entrenador_id == 0) // O null dependiendo de tu lógica frontend
            {
                // Lógica opcional si quieres desasignar
                entrenadorResult = true;
            }
            else
            {
                // Verifica si ya existe asignación para hacer UPDATE o INSERT
                string checkQuery = "SELECT COUNT(*) FROM asignacion_entrenadores WHERE actividad_id=@actividad";
                string count = PostgresHandler.GetScalar(checkQuery, ("actividad", actividad.Actividad_id));

                if (count == "0")
                {
                    string queryEnt = "INSERT INTO asignacion_entrenadores(persona_id,actividad_id) VALUES (@persona,@actividad);";
                    entrenadorResult = PostgresHandler.Exec(queryEnt,
                        ("persona", actividad.Entrenador_id),
                        ("actividad", actividad.Actividad_id));
                }
                else
                {
                    string queryEnt = "UPDATE asignacion_entrenadores SET persona_id=@persona WHERE actividad_id=@actividad;";
                    entrenadorResult = PostgresHandler.Exec(queryEnt,
                        ("persona", actividad.Entrenador_id),
                        ("actividad", actividad.Actividad_id));
                }
            }

            if (result)
            {
                updateResult.Success = true;
                updateResult.Message = "ok";
            }
            else
            {
                updateResult.Success = false;
                updateResult.Message = "Ocurrio un problema actualizando la actividad";
            }

            return updateResult;
        }

        public ApiResponse<Actividad> GetActividadById(int actividadId)
        {
            ApiResponse<Actividad> getResult = new ApiResponse<Actividad>();
            string query = $"SELECT * FROM actividades WHERE actividad_id=@id";
            string result = PostgresHandler.GetJson(query, ("id", actividadId));

            List<Actividad> actividades = JsonConvert.DeserializeObject<List<Actividad>>(result);

            if (actividades == null || actividades.Count == 0)
            {
                getResult.Success = false;
                getResult.Message = "No se encontró la actividad.";
            }
            else
            {
                getResult.Success = true;
                getResult.Data = actividades.FirstOrDefault();
            }
            return getResult;
        }

        public ApiResponse DeleteActividad(int id)
        {
            ApiResponse deleteResult = new ApiResponse();
            string query = "UPDATE actividades SET estado = false WHERE actividad_id = @id";
            bool result = PostgresHandler.Exec(query, ("id", id));

            deleteResult.Success = result;
            if (result)
                deleteResult.Message = "La actividad se ha dado de baja correctamente.";
            else
                deleteResult.Message = "Ocurrió un problema al dar de baja la actividad.";

            return deleteResult;
        }

        public ApiResponse DeleteActividadEntrenador(int actividadId)
        {
            ApiResponse deleteResult = new ApiResponse();
            string query = "DELETE FROM asignacion_entrenadores WHERE actividad_id=@actividadId";
            bool result = PostgresHandler.Exec(query, ("actividadId", actividadId));
            deleteResult.Success = result;
            if (!result)
                deleteResult.Message = "Ocurrio un problema al eliminar el entrenador de la actividad.";
            return deleteResult;
        }

        public ApiResponse<List<GetInscripcionesDTO>> GetInscripciones(string email)
        {
            ApiResponse<List<GetInscripcionesDTO>> result = new ApiResponse<List<GetInscripcionesDTO>>();
            try
            {
                string query = "SELECT p.*,a.actividad_id,a.nombre AS actividad_nombre,a.estado AS actividad_estado,a.cuota_valor,a.horario,i.fecha_inscripcion " +
                    "FROM usuarios u " +
                    "LEFT JOIN rel_usuarios_personas r ON r.user_id = u.user_id " +
                    "LEFT JOIN personas p ON p.persona_id =r.persona_id " +
                    "LEFT JOIN inscripciones i ON i.persona_id = p.persona_id " +
                    "LEFT JOIN actividades a ON a.actividad_id =i.actividad_id " +
                    "WHERE u.email =@mail;";

                var queryResult = PostgresHandler.GetJson(query, ("mail", email));
                var inscripciones = JsonConvert.DeserializeObject<List<GetInscripcionesDTO>>(queryResult);
                result.Data = inscripciones;
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Error: " + ex.Message;
            }
            return result;
        }

        // ==========================================
        //  NUEVOS MÉTODOS PARA COMUNICADOS
        // ==========================================

        public ApiResponse CrearComunicado(CreateComunicadoDTO dto)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                string query = @"INSERT INTO comunicados (actividad_id, entrenador_id, asunto, detalle) 
                                 VALUES (@act_id, @ent_id, @asunto, @detalle)";

                bool result = PostgresHandler.Exec(query,
                    ("act_id", dto.Actividad_id),
                    ("ent_id", dto.Entrenador_id),
                    ("asunto", dto.Asunto),
                    ("detalle", dto.Detalle));

                response.Success = result;
                response.Message = result ? "Comunicado enviado correctamente." : "Error al guardar en base de datos.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ApiResponse<List<NotificacionDTO>> GetNotificacionesUsuario(string email)
        {
            ApiResponse<List<NotificacionDTO>> response = new ApiResponse<List<NotificacionDTO>>();
            try
            {
                // Busca comunicados de actividades del usuario que NO estén en la tabla comunicados_leidos
                string query = @"
                    SELECT 
                        c.comunicado_id, 
                        c.asunto, 
                        c.detalle, 
                        a.nombre AS actividad_nombre, 
                        c.fecha_creacion AS fecha,
                        CASE WHEN cl.id IS NOT NULL THEN true ELSE false END as leido
                    FROM comunicados c
                    INNER JOIN actividades a ON a.actividad_id = c.actividad_id
                    INNER JOIN inscripciones i ON i.actividad_id = a.actividad_id
                    INNER JOIN personas p ON p.persona_id = i.persona_id
                    INNER JOIN rel_usuarios_personas rup ON rup.persona_id = p.persona_id
                    INNER JOIN usuarios u ON u.user_id = rup.user_id
                    LEFT JOIN comunicados_leidos cl ON cl.comunicado_id = c.comunicado_id AND cl.user_id = u.user_id
                    WHERE u.email = @email
                    AND cl.id IS NULL
                    ORDER BY c.fecha_creacion DESC";

                string json = PostgresHandler.GetJson(query, ("email", email));
                var lista = JsonConvert.DeserializeObject<List<NotificacionDTO>>(json);

                response.Data = lista ?? new List<NotificacionDTO>();
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ApiResponse MarcarComoLeido(int comunicadoId, string email)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                // 1. Obtener ID de usuario
                string queryUser = "SELECT u.user_id FROM usuarios u WHERE u.email = @email";
                string userIdStr = PostgresHandler.GetScalar(queryUser, ("email", email));

                if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out int userId))
                {
                    // 2. Insertar en leídos (ON CONFLICT requiere constraint en BD, o validamos antes)
                    // Validamos antes para simplicidad en PostgreSQL genérico
                    string check = "SELECT COUNT(*) FROM comunicados_leidos WHERE comunicado_id=@cid AND user_id=@uid";
                    string existe = PostgresHandler.GetScalar(check, ("cid", comunicadoId), ("uid", userId));

                    if (existe == "0")
                    {
                        string insert = "INSERT INTO comunicados_leidos (comunicado_id, user_id) VALUES (@cid, @uid)";
                        bool res = PostgresHandler.Exec(insert, ("cid", comunicadoId), ("uid", userId));
                        response.Success = res;
                    }
                    else
                    {
                        response.Success = true; // Ya estaba leído
                    }
                }
                else
                {
                    response.Success = false;
                    response.Message = "Usuario no encontrado";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}