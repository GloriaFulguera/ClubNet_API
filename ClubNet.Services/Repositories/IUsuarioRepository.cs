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
        public ApiResponse<int> RegisterToActivity(RegisterToActivityDTO registro);
        public ApiResponse<List<UsuarioDTO>> GetUsuarios();
        public ApiResponse DeleteUsuario(int personaId);
        public ApiResponse RegisterToActivityEntrenador(RegisterToActivityEntrenadorDTO registro);
        public ApiResponse<List<UsuarioDTO>> GetUsuariosByRol(int rol);
    }
}
