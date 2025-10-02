using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubNet.Models.DTO
{
    public class ActividadDTO
    {
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public int Cupo { get; set; }
        public decimal Cuota_valor { get; set; }
        public bool Estado { get; set; }
        public string Url_imagen { get; set; }
    }
}
