using ClubNet.Models;
using ClubNet.Models.DTO;

namespace ClubNet.Services.Repositories
{
    public interface IUsuarioRepository
    {
        public Task<UsuarioDTO> GetUsuarioById(int id);
        public Task<List<Rol>> GetRoles();
    }
}
