
namespace ClubNet.Models.DTO
{
    public class RegisterDTO
    {
        public int Dni { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public int Rol { get; set; }
        public string? Email { get; set; }
        public string? Clave { get; set; }
    }
}
