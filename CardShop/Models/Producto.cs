using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardShop.Models
{
    public class Producto
    {
      
        public Guid ProductoId { get; set; }

        
        public string Nombre { get; set; }

        public string Descripcion { get; set; }


        public float PrecioVigente { get; set; }

        public string Categoria { get; set; }

        
    }
}
