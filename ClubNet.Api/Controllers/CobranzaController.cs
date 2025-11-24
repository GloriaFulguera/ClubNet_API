using ClubNet.Models.DTO;
using ClubNet.Services.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http; // Necesario para StatusCodes
using ClubNet.Models;
using System.Threading.Tasks;

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

        /// <summary>
        /// Genera una solicitud de pago a través de MercadoPago.
        /// </summary>
        /// <param name="solicitud">Los detalles de la solicitud de pago (Inscripcion_id, Concepto, Monto, Moneda).</param>
        /// <returns>Un objeto ApiResponse que contiene el 'InitPoint' (link de pago) en su Data.</returns>
        [HttpPost("CreatePaymentRequest")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GenerarCobranza(SolicitudPagoDTO solicitud)
        {
            var result = await _cobranzaService.CreatePreferences(solicitud);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        /// <summary>
        /// Recibe notificaciones de cambio de estado de pago desde MercadoPago (Webhook).
        /// </summary>
        /// <remarks>
        /// Este endpoint siempre devuelve HTTP 200 para confirmar la recepción a MercadoPago, incluso si el procesamiento interno falla.
        /// </remarks>
        /// <param name="estado">El DTO con el ID de la notificación de estado de pago.</param>
        /// <returns>Un Ok (200).</returns>
        [HttpPost("NotificarStatus")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult NotificarStatus(NotificarEstadoPagoDTO estado)
        {
            var result = _cobranzaService.NotificarStatus(estado.Data.Id);
            return Ok();
        }

        /// <summary>
        /// Endpoint de redirección para pagos exitosos de MercadoPago.
        /// </summary>
        /// <returns>La respuesta del pago.</returns>
        [HttpGet("Success")]
        [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
        public IActionResult Success([FromQuery] PaymentResponse response)
        {
            string url = "http://localhost:4200/pago/exito";
            Console.WriteLine(JsonConvert.SerializeObject(response));
            return Redirect(url);
        }

        /// <summary>
        /// Endpoint de redirección para pagos fallidos de MercadoPago.
        /// </summary>
        /// <returns>La respuesta del pago.</returns>
        [HttpGet("Failure")]
        [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
        public IActionResult Failure([FromQuery] PaymentResponse response)
        {
            Console.WriteLine(JsonConvert.SerializeObject(response));
            return Ok(response);
        }

        /// <summary>
        /// Endpoint de redirección para pagos pendientes de MercadoPago.
        /// </summary>
        /// <returns>La respuesta del pago.</returns>
        [HttpGet("Pending")]
        [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
        public IActionResult Pending([FromQuery] PaymentResponse response)
        {
            Console.WriteLine(JsonConvert.SerializeObject(response));
            return Ok(response);
        }

        /// <summary>
        /// Endpoint para obtener el estado de cobros de un usuario o actividad.
        /// </summary>
        /// <returns>La respuesta del pago.</returns>
        [HttpGet("GetCobros")]
        public IActionResult GetCobros(int? actividad_id, int? persona_id)
        {
            var result = _cobranzaService.GetActividades(actividad_id, persona_id);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }
    }
}