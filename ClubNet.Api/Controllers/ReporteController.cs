using ClubNet.Services;
using ClubNet.Services.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ClubNet.Api.Controllers
{
    [ApiController]
    [Route("api/reportes")]
    public class ReporteController:ControllerBase
    {
        private readonly IReporteService _reporteService;

        public ReporteController(IReporteService reporteService)
        {
            _reporteService = reporteService;
        }

        [HttpGet("GetIngresos")]
        public IActionResult GetIngresos()
        {
            var result = _reporteService.GetIngresosMensuales();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetDeuda")]
        public IActionResult GetDeuda()
        {
            var result = _reporteService.GetEstadoDeuda();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetEstadoCupos")]
        public IActionResult GetOcupacion()
        {
            var result = _reporteService.GetOcupacion();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetNuevosSocios")]
        public IActionResult GetNuevosSocios()
        {
            var result = _reporteService.GetNuevosSocios();
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
