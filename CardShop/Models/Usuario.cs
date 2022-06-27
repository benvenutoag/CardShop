using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CardShop.Models
{
    public class Usuario
    {


     [Key]
        public Guid Id { get; set; }


        
        public string UserName { get; set; }

       
        public string Nombre { get; set; }

        
        public string Apellido { get; set; }


        
        public string Password { get; set; }

       
    }
}
