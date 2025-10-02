using ClubNet.Models;
using ClubNet.Models.DTO;

namespace ClubNet.Services.Repositories
{
    public interface IActividadRepository
    {
        public Task<ApiResponse> CreateActividad(ActividadDTO actividad);
    }
}
