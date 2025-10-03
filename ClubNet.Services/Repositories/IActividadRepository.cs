using ClubNet.Models;
using ClubNet.Models.DTO;

namespace ClubNet.Services.Repositories
{
    public interface IActividadRepository
    {
        public Task<ApiResponse> CreateActividad(Actividad actividad);
        public Task<List<Actividad>> GetActividades();
        public Task<ApiResponse> UpdateActividad(Actividad actividad);
        public Task<Actividad> GetActividadById(int actividadId);
    }
}
