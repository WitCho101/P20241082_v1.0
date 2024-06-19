using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Rdcn_Cliente
    {
        public string Nombre_cliente {  get; set; }
        public int Recencia {  get; set; }
        public int Frecuencia {  get; set; }
        public decimal Valor_monetario {  get; set; }
        public string Segmento {  get; set; }
        public string Recomendacion_cliente { get; set; }
    }
}
