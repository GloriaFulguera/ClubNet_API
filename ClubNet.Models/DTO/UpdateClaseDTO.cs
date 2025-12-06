using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubNet.Models.DTO
{
    public class UpdateClaseDTO
    {
        public int Clase_id { get; set; }
        public string Actividad { get; set; }
        public string Titulo { get; set; }
        public string Detalle { get; set; }
        public string Intensidad { get; set; }
        public string Url_multimedia { get; set; }
    }
}
