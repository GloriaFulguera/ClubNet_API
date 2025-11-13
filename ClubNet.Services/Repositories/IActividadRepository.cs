using ClubNet.Models;
using ClubNet.Models.DTO;

namespace ClubNet.Services.Repositories
{
    public interface IActividadRepository
    {
        public ApiResponse CreateActividad(Actividad actividad);
        public ApiResponse<List<GetActividadDTO>> GetActividades();
        public ApiResponse UpdateActividad(Actividad actividad);
        public ApiResponse<Actividad> GetActividadById(int actividadId);
        public ApiResponse DeleteActividad(int id);
    }
}
