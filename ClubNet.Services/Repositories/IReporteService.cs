using ClubNet.Models;
using ClubNet.Models.DTO;

namespace ClubNet.Services.Repositories
{
    public interface IReporteService
    {
        public ApiResponse<List<ReporteIngresosDTO>> GetIngresosMensuales();
        public ApiResponse<List<ReporteDeudaDTO>> GetEstadoDeuda();
        public ApiResponse<List<ReporteOcupacionDTO>> GetOcupacion();
        public ApiResponse<List<ReporteNuevosSociosDTO>> GetNuevosSocios();
    }
}
