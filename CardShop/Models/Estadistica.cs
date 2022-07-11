using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CardShop.Models
{
    public class Estadistica
    {
        [Key]
        public int Id { get; set; }
        public int CantBebida { get; set; }

        public int CantPostre { get; set; }

        public int CantComida { get; set; }

        public List<Compra> ListaCompras { get; set; }

        public Compra CompraCara { get; set; }


    }
}
