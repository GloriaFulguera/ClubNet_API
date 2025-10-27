using ClubNet.Models;
using ClubNet.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClubNet.Api.Controllers
{
    [Route("api/actividad")]
    [ApiController]
    [Authorize]
    public class ActividadController : ControllerBase
    {
        private readonly IActividadRepository _actividadService;

        public ActividadController(IActividadRepository actividadService)
        {
            _actividadService = actividadService;
        }

        [HttpPost("CreateActividad")]
        public IActionResult CreateActividad(Actividad actividad)
        {
            var result = _actividadService.CreateActividad(actividad);

            if(result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpGet("GetActividades")]
        public IActionResult GetActividades()
        {
            var result = _actividadService.GetActividades();
            if(result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result);
        }

        [HttpPut("UpdateActividad")]
        public IActionResult UpdateActividad(Actividad actividad)
        {
            var result = _actividadService.UpdateActividad(actividad);
            if(result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpDelete("DeleteActividad")]
        public IActionResult DeleteActividad(int id)
        {
            var result = _actividadService.DeleteActividad(id);
            if(result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpGet("GetActividadById/{id}")]
        public IActionResult GetActividadById(int id)
        {
            var result = _actividadService.GetActividadById(id);
            if(result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result);
        }
    }
}
