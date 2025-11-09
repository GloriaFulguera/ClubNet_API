using ClubNet.Models;
using ClubNet.Models.DTO;
using ClubNet.Services.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ClubNet.Api.Controllers
{
    [Route("api/usuario")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioService;

        public UsuarioController(IUsuarioRepository usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet("GetUsuario")]
        public IActionResult GetUsuarioByEmail(string email)
        {
            var result = _usuarioService.GetUsuarioByEmail(email);
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result);
        }

        [HttpGet("GetRoles")]
        [Authorize(Roles = "1")]
        public IActionResult GetRoles()
        {
            var result = _usuarioService.GetRoles();
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result);
        }

        [HttpPost("UpdateUsuario")]
        [Authorize(Roles = "1")]
        public IActionResult UpdateUsuario(UsuarioDTO usuario)
        {
            var result = _usuarioService.UpdateUsuario(usuario);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpPost("CreateUser")]
        [Authorize(Roles = "1")]
        public IActionResult CreateUser(RegisterDTO usuario)
        {
            var result = _usuarioService.CreateUser(usuario);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpPost("RegisterToActivity")]
        public IActionResult RegisterToActivity(RegisterToActivityDTO registro)
        {
            var result = _usuarioService.RegisterToActivity(registro);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpGet("GetUsuarios")]
        [Authorize(Roles = "1")]
        public IActionResult GetUsuarios()
        {
            var result = _usuarioService.GetUsuarios();
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result);
        }

        // CORRECCIÓN: Se añade [FromQuery] para forzar el binding del parámetro 'id'
        [HttpDelete("DeleteUsuario")]
        [Authorize(Roles = "1")]
        public IActionResult DeleteUsuario([FromQuery] int id)
        {
            var result = _usuarioService.DeleteUsuario(id);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }
    }
}