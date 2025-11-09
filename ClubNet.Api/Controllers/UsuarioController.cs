using ClubNet.Models;
using ClubNet.Models.DTO;
using ClubNet.Services.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http; // Necesario para StatusCodes
using System.Collections.Generic;

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

        /// <summary>
        /// Obtiene los datos básicos de un usuario por su email.
        /// </summary>
        /// <param name="email">El email del usuario.</param>
        /// <returns>Los datos del usuario.</returns>
        [HttpGet("GetUsuario")]
        [ProducesResponseType(typeof(UsuarioDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public IActionResult GetUsuarioByEmail(string email)
        {
            var result = _usuarioService.GetUsuarioByEmail(email);
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result);
        }

        /// <summary>
        /// Obtiene un listado de todos los roles de usuario disponibles. (Requiere Rol Admin - '1').
        /// </summary>
        /// <returns>Un listado de roles.</returns>
        [HttpGet("GetRoles")]
        [Authorize(Roles = "1")]
        [ProducesResponseType(typeof(List<Rol>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult GetRoles()
        {
            var result = _usuarioService.GetRoles();
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result);
        }

        /// <summary>
        /// Actualiza los datos de un usuario. (Requiere Rol Admin - '1').
        /// </summary>
        /// <param name="usuario">Los datos actualizados del usuario (incluyendo Persona_id, Nombre, Email, etc.).</param>
        /// <returns>Un estado de la operación.</returns>
        [HttpPost("UpdateUsuario")]
        [Authorize(Roles = "1")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult UpdateUsuario(UsuarioDTO usuario)
        {
            var result = _usuarioService.UpdateUsuario(usuario);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        /// <summary>
        /// Crea un nuevo usuario. (Requiere Rol Admin - '1').
        /// </summary>
        /// <param name="usuario">Datos del nuevo usuario (Dni, Nombre, Apellido, Rol, Email, Clave).</param>
        /// <returns>Un estado de la operación.</returns>
        [HttpPost("CreateUser")]
        [Authorize(Roles = "1")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult CreateUser(RegisterDTO usuario)
        {
            var result = _usuarioService.CreateUser(usuario);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        /// <summary>
        /// Registra la inscripción de un usuario a una actividad.
        /// </summary>
        /// <param name="registro">Datos de la inscripción (Dni del usuario, ID de la Actividad).</param>
        /// <returns>Un objeto ApiResponse que contiene el ID de la nueva inscripción si es exitoso.</returns>
        [HttpPost("RegisterToActivity")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public IActionResult RegisterToActivity(RegisterToActivityDTO registro)
        {
            var result = _usuarioService.RegisterToActivity(registro);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        /// <summary>
        /// Obtiene un listado de todos los usuarios registrados. (Requiere Rol Admin - '1').
        /// </summary>
        /// <returns>Un listado de usuarios.</returns>
        [HttpGet("GetUsuarios")]
        [Authorize(Roles = "1")]
        [ProducesResponseType(typeof(List<UsuarioDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult GetUsuarios()
        {
            var result = _usuarioService.GetUsuarios();
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result);
        }

        /// <summary>
        /// Realiza la baja lógica de un usuario (cambia el estado a inactivo). (Requiere Rol Admin - '1').
        /// </summary>
        /// <param name="id">El ID de la persona (Persona_id) a dar de baja.</param>
        /// <returns>Un estado de la operación.</returns>
        [HttpDelete("DeleteUsuario")]
        [Authorize(Roles = "1")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
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