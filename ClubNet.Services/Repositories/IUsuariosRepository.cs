using ClubNet.Models.DTO;

namespace ClubNet.Services.Repositories
{
    public interface IUsuariosRepository
    {
        public Task<UsuarioDTO> GetUsuarioById(int id);
    }
}
