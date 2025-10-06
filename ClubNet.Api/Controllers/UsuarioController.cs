using ClubNet.Models;
using ClubNet.Models.DTO;
using ClubNet.Services.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ClubNet.Api.Controllers
{
    [Route("api/usuario")]
    [ApiController]
    public class UsuarioController:ControllerBase
    {
        private readonly IUsuarioRepository _usuarioService;

        public UsuarioController(IUsuarioRepository usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet("GetUsuario")]
        public async Task<UsuarioDTO> GetUsuarioByEmail(string email)
        {
            return await Task.Run(() => _usuarioService.GetUsuarioByEmail(email));
        }

        [HttpGet("GetRoles")]
        public async Task<List<Rol>> GetRoles()
        {
            return await Task.Run(() => _usuarioService.GetRoles());
        }

        [HttpPost("UpdateUsuario")]
        public async Task<ApiResponse> UpdateUsuario(UsuarioDTO usuario)
        {
            return await Task.Run(() => _usuarioService.UpdateUsuario(usuario));
        }

        [HttpPost("CreateUser")]
        public async Task<ApiResponse> CreateUser(RegisterDTO usuario)
        {
            return await Task.Run(() => _usuarioService.CreateUser(usuario));
        }
    }
}
