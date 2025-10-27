using ClubNet.Models;
using ClubNet.Services.Handlers;
using ClubNet.Services.Repositories;
using Newtonsoft.Json;

namespace ClubNet.Services
{
    public class ActividadService : IActividadRepository
    {
        public ApiResponse CreateActividad(Actividad actividad)
        {
            ApiResponse createResult = new ApiResponse();
            string query = $"INSERT INTO actividades(nombre,descripcion,cupo,estado,cuota_valor,url_imagen) " +
                $"VALUES (@nombre,@descripcion,@cupo,@estado,@cuota_valor,@url_imagen)";
            bool result = PostgresHandler.Exec(query,
                ("nombre", actividad.Nombre),
                ("descripcion", actividad.Descripcion),
                ("cupo", actividad.Cupo),
                ("estado", actividad.Estado),
                ("cuota_valor", actividad.Cuota_valor),
                ("url_imagen", actividad.Url_imagen));

            createResult.Success = result;
            if (!result)
                createResult.Message = "Ocurrio un problema al crear la actividad, contacte al administrador.";

            return createResult;
        }

        public ApiResponse<List<Actividad>> GetActividades()
        {
            ApiResponse<List<Actividad>> getResult = new ApiResponse<List<Actividad>>();
            string query = "SELECT * FROM actividades ORDER BY actividad_id ASC;";
            string result = PostgresHandler.GetJson(query);
            List<Actividad> actividades = JsonConvert.DeserializeObject<List<Actividad>>(result);
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

        public ApiResponse UpdateActividad(Actividad actividad)
        {
            ApiResponse updateResult = new ApiResponse();

            string query = $"UPDATE actividades SET nombre=@nombre, descripcion=@descripcion, cupo=@cupo, " +
                $"estado=@estado, cuota_valor=@cuota_valor, url_imagen=@url_imagen WHERE actividad_id=@id";

            bool result = PostgresHandler.Exec(query,
                ("nombre", actividad.Nombre),
                ("descripcion", actividad.Descripcion),
                ("cupo", actividad.Cupo),
                ("estado", actividad.Estado),
                ("cuota_valor", actividad.Cuota_valor),
                ("url_imagen", actividad.Url_imagen),
                ("id", actividad.Actividad_id));

            updateResult.Success = result;

            if (!result)
                updateResult.Message = "Ocurrio un problema al actualizar la actividad, contacte al administrador.";

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

    }
}
