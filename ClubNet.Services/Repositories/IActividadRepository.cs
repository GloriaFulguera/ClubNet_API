using ClubNet.Models;
using ClubNet.Models.DTO;

namespace ClubNet.Services.Repositories
{
    public interface IActividadRepository
    {
        public ApiResponse CreateActividad(CreateActividadDTO actividad);
        public ApiResponse<List<GetActividadDTO>> GetActividades(int? entrenadorId = null);
        public ApiResponse UpdateActividad(UpdateActividadDTO actividad);
        public ApiResponse<Actividad> GetActividadById(int actividadId);
        public ApiResponse DeleteActividad(int id);
        public ApiResponse DeleteActividadEntrenador(int actividadId);
        public ApiResponse<List<GetInscripcionesDTO>> GetInscripciones(string email);
        public ApiResponse CrearComunicado(CreateComunicadoDTO dto);
        public ApiResponse<List<NotificacionDTO>> GetNotificacionesUsuario(string email);
        public ApiResponse MarcarComoLeido(int comunicadoId, string email);
    }
}
