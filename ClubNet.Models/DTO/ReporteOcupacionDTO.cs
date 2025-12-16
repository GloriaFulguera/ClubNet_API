namespace ClubNet.Models.DTO
{
    public class ReporteOcupacionDTO
    {
        public string Actividad { get; set; }
        public int Cupo { get; set; }
        public int Inscriptos { get; set; }
        public int Disponibles => Cupo - Inscriptos; 
    }
}
