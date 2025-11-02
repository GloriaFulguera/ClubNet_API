
using ClubNet.Models;
using ClubNet.Models.DTO;

namespace ClubNet.Services.Repositories
{
    public interface ICobranzaRepository
    {
        public Task<ApiResponse<string>> CreatePreferences(SolicitudPagoDTO solicitud);
        public Task<ApiResponse<ObtenerDetallePagoDTO>> NotificarStatus(string payment_id);
    }
}
