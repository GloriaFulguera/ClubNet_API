using ClubNet.Models;
using ClubNet.Services.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ClubNet.Api.Controllers
{
    [Route("api/actividad")]
    [ApiController]
    public class ActividadController:ControllerBase
    {
        private readonly IActividadRepository _actividadService;

        public ActividadController(IActividadRepository actividadService)
        {
            _actividadService = actividadService;
        }

        [HttpPost("CreateActividad")]
        public async Task<ApiResponse> CreateActividad(Actividad actividad)
        {
            return await Task.Run(() => _actividadService.CreateActividad(actividad));
        }

        [HttpGet("GetActividades")]
        public async Task<List<Actividad>> GetActividades()
        {
            return await Task.Run(() => _actividadService.GetActividades());
        }

        [HttpPost("UpdateActividad")]
        public async Task<ApiResponse> UpdateActividad(Actividad actividad)
        {
            return await Task.Run(() => _actividadService.UpdateActividad(actividad));
        }

    }
}
