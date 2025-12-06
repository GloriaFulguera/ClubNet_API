using ClubNet.Models;
using ClubNet.Models.DTO;
using ClubNet.Services.Handlers;
using ClubNet.Services.Repositories;
using Newtonsoft.Json;

namespace ClubNet.Services
{
    public class ClaseService : IClasesRepository
    {
        public ApiResponse CreateClase(CreateClaseDTO clase)
        {
            ApiResponse response = new ApiResponse();

            string query = $"INSERT INTO clases(actividad_id,actividad,titulo,detalle,intensidad) " +
                $"VALUES (@actividad_id,@titulo,@detalle)";
            bool result = PostgresHandler.Exec(query,
                ("actividad_id", clase.Actividad_id),
                ("actividad", clase.Actividad),
                ("titulo", clase.Titulo),
                ("detalle", clase.Detalle),
                ("intensidad", clase.Intensidad));

            response.Success = result;
            if (!result)
                response.Message = "Ocurrio un problema al crear la clase, contacte al administrador.";

            return response;
        }

        public ApiResponse UpdateClase(UpdateClaseDTO clase)
        {
            ApiResponse response = new ApiResponse();
            string query = $"UPDATE clases SET actividad=@actividad, titulo=@titulo, detalle=@detalle " +
                $"WHERE clase_id=@clase_id";
            bool result = PostgresHandler.Exec(query,
                ("actividad", clase.Actividad),
                ("titulo", clase.Titulo),
                ("detalle", clase.Detalle),
                ("clase_id", clase.Clase_id));

            response.Success = result;
            if (!result)
                response.Message = "Ocurrio un problema al actualizar la clase, contacte al administrador.";
            return response;
        }

        public ApiResponse<List<ClaseDTO>> GetClases(int actividadId)
        {
            ApiResponse<List<ClaseDTO>> response = new ApiResponse<List<ClaseDTO>>();
            string query = "SELECT * FROM clases WHERE actividad_id=@actividadId ORDER BY clase_id ASC;";
            string result = PostgresHandler.GetJson(query,("actividadId",actividadId));
            List<ClaseDTO> clases = JsonConvert.DeserializeObject<List<ClaseDTO>>(result);

            if (clases == null)
            {
                response.Success = false;
                response.Message = "Ocurrió un problema al obtener las clases.";
            }
            else
            {
                response.Success = true;
                response.Data = clases;
            }
            return response;
        }

        public ApiResponse DeleteClase(int claseId)
        {
            ApiResponse response = new ApiResponse();
            string query = "DELETE FROM clases WHERE clase_id=@clase_id";
            bool result = PostgresHandler.Exec(query, ("clase_id", claseId));

            response.Success = result;
            if (!result)
                response.Message = "Ocurrio un problema al eliminar la clase, contacte al administrador.";
            return response;
        }
    }
}
