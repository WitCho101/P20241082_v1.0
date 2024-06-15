using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Ticket
    {
        public int IdTicket {  get; set; }

        public Usuarios oUsuario { get; set; }

        public string Asunto { get; set; }

        public bool Prioridad { get; set; }

        public bool Estado { get; set; }


    }
}
