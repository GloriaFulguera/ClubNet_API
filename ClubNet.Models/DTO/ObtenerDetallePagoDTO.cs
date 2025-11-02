using System.Text.Json.Serialization;

namespace ClubNet.Models.DTO
{
    public class ObtenerDetallePagoDTO
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } // <-- ¡Este es el campo que te importa!

        [JsonPropertyName("status_detail")]
        public string StatusDetail { get; set; }

        [JsonPropertyName("date_created")]
        public DateTime DateCreated { get; set; }

        [JsonPropertyName("date_approved")]
        public DateTime? DateApproved { get; set; }

        [JsonPropertyName("payment_method")]
        public PaymentMethodDetails PaymentMethod { get; set; }

        [JsonPropertyName("payment_type_id")]
        public string PaymentTypeId { get; set; }

        [JsonPropertyName("payer")]
        public PayerDetails Payer { get; set; }

        [JsonPropertyName("currency_id")]
        public string CurrencyId { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("external_reference")]
        public string ExternalReference { get; set; }

        [JsonPropertyName("transaction_amount")]
        public decimal TransactionAmount { get; set; }

        [JsonPropertyName("transaction_details")]
        public TransactionDetails TransactionDetails { get; set; }

        [JsonPropertyName("installments")]
        public int Installments { get; set; }

        // Objetos que están vacíos en el ejemplo, pero pueden venir con datos
        [JsonPropertyName("metadata")]
        public object Metadata { get; set; }

        [JsonPropertyName("additional_info")]
        public object AdditionalInfo { get; set; }

        [JsonPropertyName("card")]
        public object Card { get; set; }
    }

    public class PayerDetails
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } // A veces es ID de customer, mejor string

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("identification")]
        public PayerIdentification Identification { get; set; }
    }

    public class PayerIdentification
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("number")]
        public string Number { get; set; } // DNI/CUIT es mejor como string
    }

    public class PaymentMethodDetails
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } // ej. "master"

        [JsonPropertyName("issuer_id")]
        public string IssuerId { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; } // ej. "credit_card"
    }

    public class TransactionDetails
    {
        [JsonPropertyName("net_received_amount")]
        public decimal NetReceivedAmount { get; set; }

        [JsonPropertyName("total_paid_amount")]
        public decimal TotalPaidAmount { get; set; }

        [JsonPropertyName("overpaid_amount")]
        public decimal OverpaidAmount { get; set; }

        [JsonPropertyName("installment_amount")]
        public decimal InstallmentAmount { get; set; }
    }
}
