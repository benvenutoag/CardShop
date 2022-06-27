using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CardShop.Models
{
    public class Carrito
    {
       [Key]
        public Guid CarritoId { get; set; }
        
       [ForeignKey(nameof(Usuario))]
        public Guid UsuarioID { get; set; }
        public Usuario Usuario { get; set; }

        public List<CarritoItem> CarritosItems { get; set; }
        public double Subtotal { get; set; }

    }
}
