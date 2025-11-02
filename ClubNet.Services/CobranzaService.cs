using ClubNet.Models;
using ClubNet.Models.DTO;
using ClubNet.Services.Repositories;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Preference;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;

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
                MercadoPagoConfig.AccessToken=_configuration["MercadoPago:AccessToken"];
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
                    ExpirationDateFrom = DateTime.Now,
                    ExpirationDateTo = DateTime.Now.AddMinutes(10),
                    Expires= true
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

        public async Task<ApiResponse<ObtenerDetallePagoDTO>> NotificarStatus(string payment_id)
        {
            ApiResponse<ObtenerDetallePagoDTO> response = new ApiResponse<ObtenerDetallePagoDTO>();
            try
            {
                var url = _configuration["MercadoPago:URL_GetPaymentInfo"]+"/payments/"+payment_id;
                var client=new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["MercadoPago:AccessToken"]);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                var res= await client.GetAsync(url);
                if(!res.IsSuccessStatusCode)
                {
                    response.Success = false;
                    response.Message = $"No se pudo obtener el detalle del pago id: {payment_id}";
                    return response;
                }

                var jsonResponse= await res.Content.ReadAsStringAsync();

                var pagoDetalle= JsonConvert.DeserializeObject<ObtenerDetallePagoDTO>(jsonResponse);

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
    }
}
