using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardShop.Models
{
    public class CarritoItem
    {
    
        public Guid CarritoItemId { get; set; }


        public Guid CarritoId { get; set; }
        public Carrito Carrito { get; set; }


        public Guid ProductoId { get; set; }
        public Producto Producto { get; set; }


        public int Cantidad { get; set; }

    }
}
