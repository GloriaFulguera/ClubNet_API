
using System.Text.Json.Serialization;

namespace ClubNet.Models.DTO
{
    public class NotificarEstadoPagoDTO
    {
        //ID de la notificación
        [JsonPropertyName("id")]
        public long Id { get; set; }

        //Indica si la URL ingresada es válida.
        [JsonPropertyName("live_mode")]
        public bool LiveMode { get; set; }

        //Tipo de notificacion recebida e acuerdo con el tópico previamente seleccionado
        //(payments, mp-connect, subscription, claim, automatic-payments, etc)
        [JsonPropertyName("type")]
        public string Type { get; set; }

        //Fecha de creación del recurso notificado
        [JsonPropertyName("date_created")]
        public DateTime DateCreated { get; set; }

        //Identificador del vendedor
        [JsonPropertyName("user_id")]
        public long UserId { get; set; }

        //Valor que indica la versión de la API que envía la notificación
        [JsonPropertyName("api_version")]
        public string ApiVersion { get; set; }

        //Evento notificado, que indica si es una actualización de un recurso o la creación de uno nuevo
        [JsonPropertyName("action")]
        public string Action { get; set; }

        //ID del pago, de la orden comercial o del reclamo.
        [JsonPropertyName("data")]
        public NotificacionData Data { get; set; }
    }

    public class NotificacionData
    {
        // Mapea el "id" de adentro del "data"
        // Este es el ID del PAGO (ej. "999999999")
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
