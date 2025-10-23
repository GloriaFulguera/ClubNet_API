using ClubNet.Models.DTO;
using ClubNet.Services.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ClubNet.Api.Controllers
{
    [ApiController]
    [Route("api/clase")]
    public class ClaseController:ControllerBase
    {
        private readonly IClasesRepository _claseService;

        public ClaseController(IClasesRepository claseService)
        {
            _claseService = claseService;
        }

        [HttpPost("CreateClase")]
        public IActionResult CreateClase(CreateClaseDTO clase)
        {
            var result = _claseService.CreateClase(clase);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpPut("UpdateClase")]
        public IActionResult UpdateClase(ClaseDTO clase)
        {
            var result = _claseService.UpdateClase(clase);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpGet("GetClases")]
        public IActionResult GetClases(int actividadId)
        {
            var result = _claseService.GetClases(actividadId);
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result);
        }

        [HttpDelete("DeleteClase")]
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
