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

                // 2. Asignar Entrenador si viene en el DTO
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

    //bool result = PostgresHandler.Exec(query,
    //    ("nombre", actividad.Nombre),
    //    ("descripcion", actividad.Descripcion),
    //    ("cupo", actividad.Cupo),
    //    ("inicio", actividad.Inicio),
    //    ("estado", actividad.Estado),
    //    ("cuota_valor", actividad.Cuota_valor),
    //    ("url_imagen", actividad.Url_imagen));

    //createResult.Success = result;
    //        if (!result)
    //            createResult.Message = "Ocurrio un problema al crear la actividad, contacte al administrador.";

    //        return createResult;
    //    }

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
                    p.nombre AS ent_nombre,       /* Alias exacto para el DTO */
                    p.apellido AS ent_apellido,   /* Alias exacto para el DTO */
                    p.persona_id AS entrenador_id /* Alias exacto para el DTO */
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

            if (actividad.Entrenador_id == null)
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
            
            if(result && entrenadorResult)
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
            //TO DO: Corregir, vamos a hacer baja lógica en vez de eliminar registros.
            ApiResponse deleteResult = new ApiResponse();

            // 1. Eliminar clases asociadas a la actividad.
            PostgresHandler.Exec("DELETE FROM clases WHERE actividad_id=@id", ("id", id));

            // 2. Eliminar inscripciones de usuarios a esta actividad.
            PostgresHandler.Exec("DELETE FROM rel_usuarios_actividades WHERE actividad_id=@id", ("id", id));

            // 3. Eliminar la actividad principal.
            string query = "DELETE FROM actividades WHERE actividad_id=@id";
            bool result = PostgresHandler.Exec(query, ("id", id));

            deleteResult.Success = result;
            if (!result)
                deleteResult.Message = "Ocurrio un problema al eliminar la actividad, contacte al administrador.";

            return deleteResult;
        }

        public ApiResponse DeleteActividadEntrenador(int actividadId)
        {
            //No llamamos a este metodo desde ningun lado por el momento.
            //Por el momento solo se asigna un entrenador por actividad, por lo que eliminamos todo registro asociado a la actividad.
            ApiResponse deleteResult = new ApiResponse();
            string query = "DELETE FROM asignacion_entrenadores WHERE actividad_id=@actividadId";
            bool result = PostgresHandler.Exec(query, ("actividadId", actividadId));
            deleteResult.Success = result;
            if (!result)
                deleteResult.Message = "Ocurrio un problema al eliminar el entrenador de la actividad, contacte al administrador.";
            return deleteResult;
        }

    }
}
