using ClubNet.Models;
using ClubNet.Models.DTO;
using ClubNet.Services.Handlers;
using ClubNet.Services.Repositories;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Preference;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ClubNet.Services
{
    public class CobranzaService : ICobranzaRepository
    {
        private readonly IConfiguration _configuration;

        public CobranzaService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ApiResponse<string>> CreatePreferences(SolicitudPagoDTO solicitud)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            try
            {
                string queryCobro = "SELECT cobro_id " +
                    "FROM Cobros " +
                    "WHERE inscripcion_id = @id AND estado = 'PENDIENTE' AND monto=@monto " +
                    "ORDER BY cobro_id ASC LIMIT 1";

                string cobroIdReal = PostgresHandler.GetScalar(queryCobro, 
                    ("id", solicitud.Inscripcion_id),
                    ("monto",solicitud.Monto));

                MercadoPagoConfig.AccessToken = _configuration["MercadoPago:AccessToken"];
                // Crea el objeto de request de la preference
                var request = new PreferenceRequest
                {
                    Items = new List<PreferenceItemRequest>
                    {
                        new PreferenceItemRequest
                        {
                            Id = solicitud.Inscripcion_id.ToString(),
                            Title = solicitud.Concepto,
                            Quantity = 1,
                            CurrencyId = solicitud.Moneda,
                            UnitPrice = solicitud.Monto,
                        },
                    },
                    BackUrls = new PreferenceBackUrlsRequest
                    {
                        Success = _configuration["MercadoPago:URL_Success"],
                        Failure = _configuration["MercadoPago:URL_Failure"],
                        Pending = _configuration["MercadoPago:URL_Pending"],
                    },
                    NotificationUrl = _configuration["MercadoPago:URL_Notification"],
                    AutoReturn = "approved",
                    PaymentMethods = new PreferencePaymentMethodsRequest
                    {
                        ExcludedPaymentMethods = [],
                        ExcludedPaymentTypes = []
                    },
                    ExternalReference = cobroIdReal,
                    ExpirationDateFrom = DateTime.Now,
                    ExpirationDateTo = DateTime.Now.AddMinutes(10),
                    Expires = true
                };

                // Crea la preferencia usando el client
                var client = new PreferenceClient();
                Preference preference = await client.CreateAsync(request);

                response.Success = true;
                response.Data = preference.InitPoint;
                return response;
            }
            catch
            {
                response.Success = false;
                response.Message = "Ocurrio un error generando el link de pago.";
                return response;
            }
        }

        public ApiResponse NotificarStatus(string payment_id)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                string validacionQuery = "SELECT COUNT(*) FROM PagosPendientesVerificacion WHERE payment_id = @payment_id;";
                string exists = PostgresHandler.GetScalar(validacionQuery, ("payment_id", payment_id));

                if (exists == "0")
                {
                    string query = "INSERT INTO PagosPendientesVerificacion(payment_id) VALUES (@payment_id);";

                    bool result = PostgresHandler.Exec(query, ("payment_id", payment_id));

                    response.Success = result;

                    if (result)
                        response.Message = "notificacion de estado recibida correctamente.";
                    else
                        response.Message = "Error al registrar la notificacion de estado.";
                }
                return response;
            }
            catch
            {
                response.Success = false;
                response.Message = "Ocurrio un error ejecutando NotificarStatus.";
                return response;
            }
        }

        public async Task<ApiResponse<ObtenerDetallePagoDTO>> ObtenerDetallePago(string payment_id)
        {
            ApiResponse<ObtenerDetallePagoDTO> response = new ApiResponse<ObtenerDetallePagoDTO>();
            try
            {
                var url = _configuration["MercadoPago:URL_GetPaymentInfo"] + "/payments/" + payment_id;
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["MercadoPago:AccessToken"]);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                var res = await client.GetAsync(url);
                if (!res.IsSuccessStatusCode)
                {
                    response.Success = false;
                    response.Message = $"No se pudo obtener el detalle del pago id: {payment_id}";
                    return response;
                }

                var jsonResponse = await res.Content.ReadAsStringAsync();

                var pagoDetalle = JsonConvert.DeserializeObject<ObtenerDetallePagoDTO>(jsonResponse);

                response.Success = true;
                response.Data = pagoDetalle;
                return response;
            }
            catch
            {
                response.Success = false;
                response.Message = "Ocurrio un error obteniendo el detalle del pago.";
                return response;
            }
        }

        public async Task<PagoPendienteDTO> ObtenerSiguientePendiente()
        {
            string query = "SELECT id, payment_id, estado, intentos " +
                "FROM pagospendientesverificacion " +
                "WHERE estado = 'PENDIENTE' " +
                "ORDER BY fechacreacion ASC LIMIT 1";

            var result = PostgresHandler.GetJson(query);
            var lista = JsonConvert.DeserializeObject<List<PagoPendienteDTO>>(result);

            return lista?.FirstOrDefault();
        }

        public async Task ProcesarPagoPendienteAsync(PagoPendienteDTO pendiente)
        {
            try
            {
                var infoPago = await ObtenerDetallePago(pendiente.Payment_id);

                if (infoPago == null) throw new Exception("No se pudo obtener info de MP");
                if (infoPago.Data.Status == "approved")
                {
                    string refStr = infoPago.Data.ExternalReference;

                    if (int.TryParse(refStr, out int cobroId))
                    {
                        string updateCobro = "UPDATE Cobros SET Estado = 'PAGADO', Saldo = 0 WHERE cobro_id = @id";
                        PostgresHandler.Exec(updateCobro, ("id", cobroId));
                    }
                }

                string updateCola = "UPDATE PagosPendientesVerificacion SET Estado = 'PROCESADO',fechaProcesado = CURRENT_TIMESTAMP WHERE Id = @id";
                PostgresHandler.Exec(updateCola, ("id", pendiente.Id));
            }
            catch (Exception ex)
            {
                string updateError = "UPDATE PagosPendientesVerificacion SET Estado = 'ERROR', Intentos = Intentos + 1, MensajeError = @msg, fechaProcesado = CURRENT_TIMESTAMP WHERE Id = @id";
                PostgresHandler.Exec(updateError, ("id", pendiente.Id), ("msg", ex.Message));
            }
        }

        public ApiResponse<List<GetCobrosDTO>> GetActividades(int? actividad_id, int? persona_id)
        {
            ApiResponse<List<GetCobrosDTO>> response = new ApiResponse<List<GetCobrosDTO>>();
            try
            {
                string query = "SELECT c.cobro_id,i.persona_id,concat(p.nombre,' ',p.apellido) AS socio,c.periodo,c.monto,c.estado,i.actividad_id,a.nombre,i.dia_vencimiento,a.estado AS activo,c.periodo " +
                    "FROM cobros c " +
                    "LEFT JOIN inscripciones i ON i.inscripcion_id = c.inscripcion_id " +
                    "LEFT JOIN actividades a ON a.actividad_id = i.actividad_id " +
                    "LEFT JOIN personas p ON p.persona_id = i.persona_id ";

                if(actividad_id.HasValue)
                {
                    query += "WHERE i.actividad_id = @actividad_id ";
                }
                else if(persona_id.HasValue)
                {
                    query += "WHERE i.persona_id = @persona_id ";
                }

                string result = PostgresHandler.GetJson(query, 
                    ("actividad_id", actividad_id.HasValue ? actividad_id.Value : 0),
                    ("persona_id", persona_id.HasValue ? persona_id.Value : 0));
                var lista = JsonConvert.DeserializeObject<List<GetCobrosDTO>>(result);

                response.Success = true;
                response.Data = lista;
            }
            catch(Exception ex) {
                response.Success = false;
                response.Message = "Ocurrio un error obteniendo los cobros. " + ex.Message;
            }
            return response;
        }
    }
}
