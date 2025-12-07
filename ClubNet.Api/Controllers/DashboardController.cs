using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ClubNet.Models;

namespace ClubNet.Api.Controllers
{
    /// <summary>
    /// Controlador base para rutas protegidas del Dashboard (requiere autenticación JWT).
    /// </summary>
    [Route("api/dashboard")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        /// <summary>
        /// Obtiene el estado del dashboard (prueba de autenticación).
        /// </summary>
        /// <returns>Un mensaje de éxito si el token JWT es válido.</returns>
        [HttpGet("Status")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetStatus()
        {
            return Ok(new ApiResponse { Success = true, Message = "Acceso al Dashboard autorizado." });
        }
    }
}