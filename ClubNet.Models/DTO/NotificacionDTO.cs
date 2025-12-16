using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubNet.Models.DTO
{
    public class NotificacionDTO
    {
        public int Comunicado_id { get; set; }
        public string Asunto { get; set; }
        public string Detalle { get; set; }
        public string Actividad_nombre { get; set; }
        public DateTime Fecha { get; set; }
        public bool Leido { get; set; }
    }
}