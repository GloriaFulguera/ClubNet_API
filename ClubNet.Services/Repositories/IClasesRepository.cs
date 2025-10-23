
using ClubNet.Models;
using ClubNet.Models.DTO;

namespace ClubNet.Services.Repositories
{
    public interface IClasesRepository
    {
        public ApiResponse CreateClase(CreateClaseDTO clase);
        public ApiResponse UpdateClase(ClaseDTO clase);
        public ApiResponse<List<ClaseDTO>> GetClases(int actividadId);
        public ApiResponse DeleteClase(int claseId);
    }
}
