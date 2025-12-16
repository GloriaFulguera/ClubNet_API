using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubNet.Models.DTO
{
    public class CreateComunicadoDTO
    {
        public int Actividad_id { get; set; }
        public int Entrenador_id { get; set; } // Lo tomaremos del usuario logueado o del front
        public string Asunto { get; set; }
        public string Detalle { get; set; }
    }
}