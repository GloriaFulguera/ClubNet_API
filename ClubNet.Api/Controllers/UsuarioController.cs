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
        public IActionResult GetUsuarioByEmail(string email)
        {
            var result= _usuarioService.GetUsuarioByEmail(email);
            if(result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result);
        }

        [HttpGet("GetRoles")]
        public IActionResult GetRoles()
        {
            var result= _usuarioService.GetRoles();
            if(result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result);
        }

        [HttpPost("UpdateUsuario")]
        public IActionResult UpdateUsuario(UsuarioDTO usuario)
        {
            var result= _usuarioService.UpdateUsuario(usuario);
            if(result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpPost("CreateUser")]
        public IActionResult CreateUser(RegisterDTO usuario)
        {
            var result= _usuarioService.CreateUser(usuario);
            if(result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }
    }
}
