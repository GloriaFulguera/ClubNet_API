
namespace ClubNet.Models.DTO
{
    public class UpdateActividadDTO
    {
        public int Actividad_id { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public int Cupo { get; set; }
        public int Inicio { get; set; } //Debemos guardar en formato YYYYMM
        public decimal Cuota_valor { get; set; }
        public bool Estado { get; set; }
        public string Url_imagen { get; set; }
        public int Entrenador_id { get; set; }
    }
}
