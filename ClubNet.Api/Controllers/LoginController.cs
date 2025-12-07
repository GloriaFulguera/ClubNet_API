using ClubNet.Models;
using ClubNet.Models.DTO;
using ClubNet.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http; // Necesario para StatusCodes
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClubNet.Api.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginRepository _loginService;
        public LoginController(ILoginRepository loginService)
        {
            _loginService = loginService;
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema con el Rol 'Usuario' (3) por defecto.
        /// </summary>
        /// <param name="usuario">Datos del usuario para el registro.</param>
        /// <returns>Un estado de la operación.</returns>
        [HttpPost("Register")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public IActionResult Registro(RegisterDTO usuario)
        {
            var result = _loginService.Register(usuario);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        /// <summary>
        /// Autentica un usuario con email y clave, y genera un JSON Web Token (JWT).
        /// </summary>
        /// <param name="usuario">Credenciales del usuario (Email, Clave).</param>
        /// <returns>Un objeto ApiResponse que contiene el JWT en su Data si es exitoso.</returns>
        [HttpPost("Login")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public IActionResult Login(LoginDTO usuario)
        {
            var result = _loginService.Login(usuario);
            if (result.Success)
                return Ok(result);
            else
                return Unauthorized(result);
        }

        [HttpPost("RecuperarClave")]
        public IActionResult RecuperarClave([FromBody] OlvidoClaveDTO request)
        {
            var result = _loginService.RecuperarClave(request.Email);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpPost("CambiarClave")]
        [Authorize]
        public IActionResult CambiarClave([FromBody] CambiarClaveDTO datos)
        {
            // Extraemos el email del Token JWT automáticamente
            string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
                return Unauthorized(new { message = "Token inválido o sin email." });

            var result = _loginService.CambiarClave(email, datos);

            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

    }
}