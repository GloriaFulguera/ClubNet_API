using ClubNet.Models.DTO;
using ClubNet.Services.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // Necesario para StatusCodes
using System.Collections.Generic;
using ClubNet.Models;

namespace ClubNet.Api.Controllers
{
    [ApiController]
    [Route("api/clase")]
    public class ClaseController : ControllerBase
    {
        private readonly IClasesRepository _claseService;

        public ClaseController(IClasesRepository claseService)
        {
            _claseService = claseService;
        }

        /// <summary>
        /// Crea una nueva clase asociada a una actividad.
        /// </summary>
        /// <param name="clase">Los datos de la clase a crear (Actividad_id, Titulo, Detalle).</param>
        /// <returns>Un estado de la operación.</returns>
        [HttpPost("CreateClase")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public IActionResult CreateClase(CreateClaseDTO clase)
        {
            var result = _claseService.CreateClase(clase);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        /// <summary>
        /// Actualiza los datos de una clase existente.
        /// </summary>
        /// <param name="clase">Los datos actualizados de la clase (Clase_id, Titulo, Detalle).</param>
        /// <returns>Un estado de la operación.</returns>
        [HttpPut("UpdateClase")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public IActionResult UpdateClase(UpdateClaseDTO clase)
        {
            var result = _claseService.UpdateClase(clase);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        /// <summary>
        /// Obtiene un listado de clases para una actividad específica.
        /// </summary>
        /// <param name="actividadId">El ID de la actividad.</param>
        /// <returns>Un listado de clases.</returns>
        [HttpGet("GetClases")]
        [ProducesResponseType(typeof(List<ClaseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public IActionResult GetClases(int actividadId)
        {
            var result = _claseService.GetClases(actividadId);
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result);
        }

        /// <summary>
        /// Elimina una clase por su ID.
        /// </summary>
        /// <param name="claseId">El ID de la clase a eliminar.</param>
        /// <returns>Un estado de la operación.</returns>
        [HttpDelete("DeleteClase")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public IActionResult DeleteClase(int claseId)
        {
            var result = _claseService.DeleteClase(claseId);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }
    }
}