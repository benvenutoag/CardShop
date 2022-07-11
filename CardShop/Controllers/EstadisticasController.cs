using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CardShop.Datos;
using CardShop.Models;

namespace CardShop.Controllers
{
    public class EstadisticasController : Controller
    {
        private readonly BaseDatos _context;

        public EstadisticasController(BaseDatos context)
        {
            _context = context;
        }

        // GET: Estadisticas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Estadistica.ToListAsync());
        }

        // GET: Estadisticas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estadistica = await _context.Estadistica
                .FirstOrDefaultAsync(m => m.Id == id);
            if (estadistica == null)
            {
                return NotFound();
            }

            return View(estadistica);
        }

        // GET: Estadisticas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Estadisticas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CantBebida,CantPostre,CantComida")] Estadistica estadistica)
        {
            if (ModelState.IsValid)
            {
                _context.Add(estadistica);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(estadistica);
        }

        // GET: Estadisticas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estadistica = await _context.Estadistica.FindAsync(id);
            if (estadistica == null)
            {
                return NotFound();
            }
            return View(estadistica);
        }

        // POST: Estadisticas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CantBebida,CantPostre,CantComida")] Estadistica estadistica)
        {
            if (id != estadistica.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(estadistica);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EstadisticaExists(estadistica.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(estadistica);
        }

        // GET: Estadisticas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estadistica = await _context.Estadistica
                .FirstOrDefaultAsync(m => m.Id == id);
            if (estadistica == null)
            {
                return NotFound();
            }

            return View(estadistica);
        }

        // POST: Estadisticas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var estadistica = await _context.Estadistica.FindAsync(id);
            _context.Estadistica.Remove(estadistica);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EstadisticaExists(int id)
        {
            return _context.Estadistica.Any(e => e.Id == id);
        }


        public async Task<IActionResult> ObtenerEstadisticas(int id)
        {
            var estadistica = await _context.Estadistica
     .FirstOrDefaultAsync(m => m.Id == id);
            int cantparcialpostre = 0;
            int cantparcialbebebida = 0;
            int cantparcialcomida = 0;


            foreach(var item in estadistica.ListaCompras)
            {
                foreach(var car in item.Carrito.CarritosItems) {
                    if (car.Producto.Categoria.Equals("Postre"))
                    {
                        cantparcialpostre += car.Cantidad;
                    }
                    else if (car.Producto.Categoria.Equals("Bebida"))
                    {
                        cantparcialbebebida += car.Cantidad;
                    }
                    else if (car.Producto.Categoria.Equals("Comida"))
                    {
                        cantparcialcomida += car.Cantidad;
                    }



                }
            }

            estadistica.CantComida = cantparcialcomida;
            estadistica.CantBebida = cantparcialbebebida;
            estadistica.CantPostre = cantparcialpostre;

            _context.Update(estadistica);

            int totalMayor = 0;
            Compra Compraparcial = null;
            
            foreach(var item in estadistica.ListaCompras)
                if (item.Total > totalMayor)
                {
                    Compraparcial = item;
                    totalMayor = (int)item.Total;
                }

            estadistica.CompraCara = Compraparcial;

            _context.Update(estadistica);

            return RedirectToAction("Details", "Estadiscicas", new { id = estadistica.Id });
        }
    }
}
