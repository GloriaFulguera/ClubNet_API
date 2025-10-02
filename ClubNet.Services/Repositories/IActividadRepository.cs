using ClubNet.Models;

namespace ClubNet.Services.Repositories
{
    public interface IActividadRepository
    {
        public Task<ApiResponse> CreateActividad(Actividad actividad);
        public Task<List<Actividad>> GetActividades();
    }
}
