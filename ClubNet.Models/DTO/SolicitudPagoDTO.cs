
namespace ClubNet.Models.DTO
{
    public class SolicitudPagoDTO
    {
        public int Inscripcion_id { get; set; }
        public string? Concepto { get; set; }
        public decimal Monto { get; set; }
        public string Moneda { get; set; }
    }
}
