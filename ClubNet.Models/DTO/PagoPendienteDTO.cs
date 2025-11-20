using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubNet.Models.DTO
{
    public class PagoPendienteDTO
    {
        public int Id { get; set; }
        public string Payment_id { get; set; }
        public string Estado { get; set; }
        public int Intentos { get; set; }
    }
}
