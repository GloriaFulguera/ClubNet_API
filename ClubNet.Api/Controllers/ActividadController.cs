using ClubNet.Models;
using ClubNet.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace ClubNet.Api.Controllers
{
    [Route("api/actividad")]
    [ApiController]
    public class ActividadController : ControllerBase
    {
        private readonly IActividadRepository _actividadService;

        public ActividadController(IActividadRepository actividadService)
        {
            _actividadService = actividadService;
        }

        /// <summary>
        /// Crea una nueva actividad en el club.
        /// </summary>
        /// <param name="actividad">Los datos de la actividad a crear.</param>
        /// <returns>Un estado de la operación (éxito o fallo).</returns>
        [HttpPost("CreateActividad")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public IActionResult CreateActividad(Actividad actividad)
        {
            var result = _actividadService.CreateActividad(actividad);

            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        /// <summary>
        /// Obtiene un listado de todas las actividades disponibles.
        /// </summary>
        /// <returns>Un listado de actividades.</returns>
        [HttpGet("GetActividades")]
        [ProducesResponseType(typeof(List<Actividad>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public IActionResult GetActividades()
        {
            var result = _actividadService.GetActividades();
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result);
        }

        /// <summary>
        /// Actualiza los datos de una actividad existente.
        /// </summary>
        /// <param name="actividad">Los datos actualizados de la actividad (incluye Actividad_id).</param>
        /// <returns>Un estado de la operación.</returns>
        [HttpPut("UpdateActividad")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public IActionResult UpdateActividad(Actividad actividad)
        {
            var result = _actividadService.UpdateActividad(actividad);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        /// <summary>
        /// Elimina (baja física) una actividad por su ID.
        /// </summary>
        /// <param name="id">El ID de la actividad a eliminar.</param>
        /// <returns>Un estado de la operación.</returns>
        [HttpDelete("DeleteActividad")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public IActionResult DeleteActividad(int id)
        {
            var result = _actividadService.DeleteActividad(id);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        /// <summary>
        /// Obtiene el detalle de una actividad por su ID.
        /// </summary>
        /// <param name="id">El ID de la actividad.</param>
        /// <returns>Los datos de la actividad específica.</returns>
        [HttpGet("GetActividadById/{id}")]
        [ProducesResponseType(typeof(Actividad), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public IActionResult GetActividadById(int id)
        {
            var result = _actividadService.GetActividadById(id);
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result);
        }
    }
}