
namespace ClubNet.Models.DTO
{
    public class GetInscripcionesDTO
    {
        public int Persona_id { get; set; }
        public int Dni { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int Actividad_id { get; set; }
        public string Actividad_nombre { get; set; }
        public bool Actividad_estado { get; set; }
        public decimal Cuota_valor { get; set; }
        public string Horario { get; set; }
        public DateTime Fecha_inscripcion { get; set; }
    }
}
