using ClubNet.Models;
using ClubNet.Models.DTO;
using ClubNet.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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

        [HttpPost("CreateActividad")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public IActionResult CreateActividad(CreateActividadDTO actividad)
        {
            var result = _actividadService.CreateActividad(actividad);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetActividades")]
        [ProducesResponseType(typeof(List<GetActividadDTO>), StatusCodes.Status200OK)]
        public IActionResult GetActividades()
        {
            var result = _actividadService.GetActividades();
            return result.Success ? Ok(result.Data) : BadRequest(result);
        }

        [HttpPut("UpdateActividad")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public IActionResult UpdateActividad(UpdateActividadDTO actividad)
        {
            var result = _actividadService.UpdateActividad(actividad);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("DeleteActividad")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public IActionResult DeleteActividad(int id)
        {
            var result = _actividadService.DeleteActividad(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetActividadById/{id}")]
        [ProducesResponseType(typeof(Actividad), StatusCodes.Status200OK)]
        public IActionResult GetActividadById(int id)
        {
            var result = _actividadService.GetActividadById(id);
            return result.Success ? Ok(result.Data) : BadRequest(result);
        }

        [HttpGet("GetInscripciones/{email}")]
        public IActionResult GetInscripcionesUser(string email)
        {
            var result = _actividadService.GetInscripciones(email);
            return result.Success ? Ok(result.Data) : BadRequest(result);
        }

        // ==========================================
        //  ENDPOINTS NUEVOS PARA COMUNICADOS
        // ==========================================

        [HttpPost("CrearComunicado")]
        public IActionResult CrearComunicado(CreateComunicadoDTO dto)
        {
            var result = _actividadService.CrearComunicado(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetNotificaciones")]
        public IActionResult GetNotificaciones(string email)
        {
            var result = _actividadService.GetNotificacionesUsuario(email);
            return result.Success ? Ok(result.Data) : BadRequest(result);
        }

        [HttpPost("MarcarLeido")]
        public IActionResult MarcarLeido([FromQuery] int id, [FromQuery] string email)
        {
            var result = _actividadService.MarcarComoLeido(id, email);
            return Ok(result);
        }
    }
}