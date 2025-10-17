using ClubNet.Models;
using ClubNet.Models.DTO;

namespace ClubNet.Services.Repositories
{
    public interface IUsuarioRepository
    {
        public ApiResponse<UsuarioDTO> GetUsuarioByEmail(string Email);
        public ApiResponse<List<Rol>> GetRoles();
        public ApiResponse UpdateUsuario(UsuarioDTO usuario);
        public ApiResponse CreateUser(RegisterDTO usuario);
    }
}
