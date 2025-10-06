using ClubNet.Models;
using ClubNet.Models.DTO;

namespace ClubNet.Services.Repositories
{
    public interface IUsuarioRepository
    {
        public Task<UsuarioDTO> GetUsuarioByEmail(string Email);
        public Task<List<Rol>> GetRoles();
        public Task<ApiResponse> UpdateUsuario(UsuarioDTO usuario);
        public Task<ApiResponse> CreateUser(RegisterDTO usuario);
    }
}
