
namespace ClubNet.Models.DTO
{
    public class UsuarioDTO
    {
        public int Persona_id { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public int Dni { get; set; }
        public string? Email { get; set; }
        public bool Estado { get; set; }
        public int Rol_id { get; set; }
    }
}
