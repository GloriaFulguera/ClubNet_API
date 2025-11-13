using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubNet.Models.DTO
{
    public class GetActividadDTO
    {
        public int Actividad_id { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string Ent_nombre { get; set; }
        public string Ent_apellido { get; set; }
        public int Cupo { get; set; }
        public int Inicio { get; set; } //Debemos guardar en formato YYYYMM
        public decimal Cuota_valor { get; set; }
        public bool Estado { get; set; }
        public string Url_imagen { get; set; }
    }
}
