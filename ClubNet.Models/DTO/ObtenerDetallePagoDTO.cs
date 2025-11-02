using Newtonsoft.Json;

namespace ClubNet.Models.DTO
{
    public class ObtenerDetallePagoDTO
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; } // <-- ¡Este es el campo que te importa!

        [JsonProperty("status_detail")]
        public string StatusDetail { get; set; }

        [JsonProperty("date_created")]
        public DateTime DateCreated { get; set; }

        [JsonProperty("date_approved")]
        public DateTime? DateApproved { get; set; }

        [JsonProperty("payment_method")]
        public PaymentMethodDetails PaymentMethod { get; set; }

        [JsonProperty("payment_type_id")]
        public string PaymentTypeId { get; set; }

        [JsonProperty("payer")]
        public PayerDetails Payer { get; set; }

        [JsonProperty("currency_id")]
        public string CurrencyId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("external_reference")]
        public string ExternalReference { get; set; }

        [JsonProperty("transaction_amount")]
        public decimal TransactionAmount { get; set; }

        [JsonProperty("transaction_details")]
        public TransactionDetails TransactionDetails { get; set; }

        [JsonProperty("installments")]
        public int Installments { get; set; }

        // Objetos que están vacíos en el ejemplo, pero pueden venir con datos
        [JsonProperty("metadata")]
        public object Metadata { get; set; }

        [JsonProperty("additional_info")]
        public object AdditionalInfo { get; set; }

        [JsonProperty("card")]
        public object Card { get; set; }
    }

    public class PayerDetails
    {
        [JsonProperty("id")]
        public string Id { get; set; } // A veces es ID de customer, mejor string

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("identification")]
        public PayerIdentification Identification { get; set; }
    }

    public class PayerIdentification
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("number")]
        public string Number { get; set; } // DNI/CUIT es mejor como string
    }

    public class PaymentMethodDetails
    {
        [JsonProperty("id")]
        public string Id { get; set; } // ej. "master"

        [JsonProperty("issuer_id")]
        public string IssuerId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; } // ej. "credit_card"
    }

    public class TransactionDetails
    {
        [JsonProperty("net_received_amount")]
        public decimal NetReceivedAmount { get; set; }

        [JsonProperty("total_paid_amount")]
        public decimal TotalPaidAmount { get; set; }

        [JsonProperty("overpaid_amount")]
        public decimal OverpaidAmount { get; set; }

        [JsonProperty("installment_amount")]
        public decimal InstallmentAmount { get; set; }
    }
}
