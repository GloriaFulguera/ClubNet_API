
using ClubNet.Models;
using ClubNet.Models.DTO;

namespace ClubNet.Services.Repositories
{
    public interface ICobranzaRepository
    {
        public Task<ApiResponse<string>> CreatePreferences(SolicitudPagoDTO solicitud);
        public ApiResponse NotificarStatus(string payment_id);
        public Task<ApiResponse<ObtenerDetallePagoDTO>> ObtenerDetallePago(string payment_id);
        public Task<PagoPendienteDTO> ObtenerSiguientePendiente();
        public Task ProcesarPagoPendienteAsync(PagoPendienteDTO pendiente);
        public ApiResponse<List<GetCobrosDTO>> GetActividades(int? actividad_id, int? persona_id);
    }
}
