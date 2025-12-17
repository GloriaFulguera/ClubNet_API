using ClubNet.Models;
using ClubNet.Models.DTO;
using ClubNet.Services.Handlers;
using ClubNet.Services.Repositories;
using Newtonsoft.Json;

namespace ClubNet.Services
{
    public class ReporteService : IReporteService
    {
        public ApiResponse<List<ReporteIngresosDTO>> GetIngresosMensuales()
        {
            var response = new ApiResponse<List<ReporteIngresosDTO>>();
            try
            {
                string query = @"
                    SELECT 
                        periodo as Mes, 
                        SUM(monto) as Total
                    FROM cobros
                    WHERE estado = 'PAGADO'
                    GROUP BY periodo
                    ORDER BY periodo ASC
                    LIMIT 12;"; // Últimos 12 periodos registrados

                string json = PostgresHandler.GetJson(query);
                response.Data = JsonConvert.DeserializeObject<List<ReporteIngresosDTO>>(json);
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error obteniendo ingresos: " + ex.Message;
            }
            return response;
        }

        public ApiResponse<List<ReporteDeudaDTO>> GetEstadoDeuda()
        {
            var response = new ApiResponse<List<ReporteDeudaDTO>>();
            try
            {
                string query = @"
                    SELECT 
                        estado, 
                        COUNT(*) as Cantidad, 
                        SUM(monto) as MontoTotal
                    FROM cobros
                    GROUP BY estado;";

                string json = PostgresHandler.GetJson(query);
                response.Data = JsonConvert.DeserializeObject<List<ReporteDeudaDTO>>(json);
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error obteniendo estado de deuda: " + ex.Message;
            }
            return response;
        }

        public ApiResponse<List<ReporteOcupacionDTO>> GetOcupacion()
        {
            var response = new ApiResponse<List<ReporteOcupacionDTO>>();
            try
            {
                string query = @"
                    SELECT 
                        a.nombre as Actividad, 
                        a.cupo, 
                        COUNT(i.inscripcion_id) as Inscriptos
                    FROM actividades a
                    LEFT JOIN inscripciones i ON a.actividad_id = i.actividad_id
                    WHERE a.estado = true -- Solo actividades activas
                    GROUP BY a.actividad_id, a.nombre, a.cupo
                    ORDER BY Inscriptos DESC;";

                string json = PostgresHandler.GetJson(query);
                response.Data = JsonConvert.DeserializeObject<List<ReporteOcupacionDTO>>(json);
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error obteniendo ocupación: " + ex.Message;
            }
            return response;
        }

        public ApiResponse<List<ReporteNuevosSociosDTO>> GetNuevosSocios()
        {
            var response = new ApiResponse<List<ReporteNuevosSociosDTO>>();
            try
            {
                string query = @"
                    SELECT 
                        TO_CHAR(fecha_inscripcion, 'YYYY-MM') as Mes,
                        COUNT(*) as CantidadNuevos
                    FROM inscripciones
                    GROUP BY Mes
                    ORDER BY Mes ASC
                    LIMIT 12;";

                string json = PostgresHandler.GetJson(query);
                response.Data = JsonConvert.DeserializeObject<List<ReporteNuevosSociosDTO>>(json);
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error obteniendo nuevos socios: " + ex.Message;
            }
            return response;
        }
    }
}
