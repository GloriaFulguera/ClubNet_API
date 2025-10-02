using ClubNet.Models;
using ClubNet.Services.Handlers;
using ClubNet.Services.Repositories;
using Newtonsoft.Json;

namespace ClubNet.Services
{
    public class ActividadService:IActividadRepository
    {
        public async Task<ApiResponse> CreateActividad(Actividad actividad)
        {
            ApiResponse createResult = new ApiResponse();
            string query = $"INSERT INTO actividades(nombre,descripcion,cupo,estado,cuota_valor,url_imagen) " +
                $"VALUES (@nombre,@descripcion,@cupo,@estado,@cuota_valor,@url_imagen)";
            bool result= PostgresHandler.Exec(query,
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

        public async Task<List<Actividad>> GetActividades()
        {
            string query = "SELECT * FROM actividades ORDER BY actividad_id ASC;";
            string result = PostgresHandler.GetJson(query);
            List<Actividad> actividades = JsonConvert.DeserializeObject<List<Actividad>>(result);
            return actividades;
        }
    }
}
