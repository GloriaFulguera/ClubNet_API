
namespace ClubNet.Models.DTO
{
    public class ClaseDTO
    {
        public int Clase_id { get; set; }
        public int Actividad_id { get; set; }
        public string Actividad { get; set; }
        public string Titulo { get; set; }
        public string Detalle { get; set; }
        public string Intensidad { get; set; }
        public string? Url_multimedia { get; set; }
    }
}
