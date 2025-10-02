using ClubNet.Models;
using ClubNet.Models.DTO;
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
        public async Task<ApiResponse> CreateActividad(ActividadDTO actividad)
        {
            return await Task.Run(() => _actividadService.CreateActividad(actividad));
        }
    }
}
