using ClubNet.Models.DTO;
using ClubNet.Services.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ClubNet.Api.Controllers
{
    [Route("api/usuario")]
    [ApiController]
    public class UsuarioController:ControllerBase
    {
        private readonly IUsuariosRepository _usuarioService;

        public UsuarioController(IUsuariosRepository usuarioService)
        {
            _usuarioService = usuarioService;
        }
        [HttpGet("GetUsuario")]
        public async Task<UsuarioDTO> GetUsuarioById(int id)
        {
            return await Task.Run(() => _usuarioService.GetUsuarioById(id));
        }
    }
}
