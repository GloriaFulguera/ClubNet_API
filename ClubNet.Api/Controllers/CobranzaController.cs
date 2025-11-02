using ClubNet.Models.DTO;
using ClubNet.Services.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ClubNet.Api.Controllers
{
    [Route("api/cobranza")]
    [ApiController]
    public class CobranzaController : ControllerBase
    {
        private readonly ICobranzaRepository _cobranzaService;

        public CobranzaController(ICobranzaRepository cobranzaService)
        {
            _cobranzaService = cobranzaService;
        }

        [HttpPost("CreatePaymentRequest")]
        public async Task<IActionResult> GenerarCobranza(SolicitudPagoDTO solicitud)
        {
            var result = await _cobranzaService.CreatePreferences(solicitud);
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result);
        }

        [HttpPost("NotificarStatus")]
        public async Task<IActionResult> NotificarStatus(NotificarEstadoPagoDTO estado)
        {
            var result = await _cobranzaService.NotificarStatus(estado.Data.Id);

                return Ok(result.Data);

        }

        [HttpGet("Success")]
        public IActionResult Success([FromQuery] PaymentResponse response)
        {
            Console.WriteLine(JsonConvert.SerializeObject(response));
            return Ok(response);
        }
        [HttpGet("Failure")]
        public IActionResult Failure([FromQuery] PaymentResponse response)
        {
            Console.WriteLine(JsonConvert.SerializeObject(response));
            return Ok(response);
        }
    }
}
