using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubNet.Models.DTO
{
    public class GetCobrosDTO
    {
        public int Cobro_id { get; set; }
        public int Persona_id { get; set; }
        public int Periodo { get; set; }
        public decimal Monto { get; set; }
        public string Estado { get; set; }
        public int Actividad_id { get; set; }
        public string Nombre { get; set; }
        public int Vencimiento { get; set; }
        public bool Activo { get; set; }
    }
}
