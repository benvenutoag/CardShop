using CardShop.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardShop.Datos
{
    public class BaseDatos : DbContext
    {
        public BaseDatos(DbContextOptions options)
            : base(options)
        {
          
                
        }
        public DbSet<Usuario> Usuario { get; set; }

        public DbSet<Carrito> Carrito { get; set; }

        public DbSet<Producto> Producto { get; set; }

        public DbSet<CarritoItem> CarritoItem { get; set; }

        public DbSet<Compra> Compra { get; set; }


    }
}
