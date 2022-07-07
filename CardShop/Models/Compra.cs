using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardShop.Models
{
    public class Compra
    {
        public Guid CompraID { get; set; }

        public Guid UsuarioId { get; set; }

        public Usuario Usuario { get; set; }

        public Guid CarritoId { get; set; }

        public Carrito Carrito { get; set; }

        public double Total { get; set; }

        public DateTime Fecha { get; set; }

        public String Estado { get; set; }

    }
}
