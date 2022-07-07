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
    public class ComprasController : Controller
    {
        private readonly BaseDatos _context;

        public ComprasController(BaseDatos context)
        {
            _context = context;
        }

        // GET: Compras
        public async Task<IActionResult> Index()
        {
            var baseDatos = _context.Compras.Include(c => c.Carrito).Include(c => c.Usuario);
            return View(await baseDatos.ToListAsync());
        }

        // GET: Compras/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compra = await _context.Compras
                .Include(c => c.Carrito)
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(m => m.CompraID == id);
            if (compra == null)
            {
                return NotFound();
            }

            return View(compra);
        }

        // GET: Compras/Create
        public IActionResult Create()
        {
            ViewData["CarritoId"] = new SelectList(_context.Carrito, "CarritoId", "CarritoId");
            ViewData["UsuarioId"] = new SelectList(_context.Usuario, "Id", "Id");
            return View();
        }

        // POST: Compras/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CompraID,UsuarioId,CarritoId,Total,Fecha,Estado")] Compra compra)
        {
            if (ModelState.IsValid)
            {
                compra.CompraID = Guid.NewGuid();
                _context.Add(compra);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarritoId"] = new SelectList(_context.Carrito, "CarritoId", "CarritoId", compra.CarritoId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuario, "Id", "Id", compra.UsuarioId);
            return View(compra);
        }

        // GET: Compras/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compra = await _context.Compras.FindAsync(id);
            if (compra == null)
            {
                return NotFound();
            }
            ViewData["CarritoId"] = new SelectList(_context.Carrito, "CarritoId", "CarritoId", compra.CarritoId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuario, "Id", "Id", compra.UsuarioId);
            return View(compra);
        }

        // POST: Compras/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CompraID,UsuarioId,CarritoId,Total,Fecha,Estado")] Compra compra)
        {
            if (id != compra.CompraID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(compra);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompraExists(compra.CompraID))
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
            ViewData["CarritoId"] = new SelectList(_context.Carrito, "CarritoId", "CarritoId", compra.CarritoId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuario, "Id", "Id", compra.UsuarioId);
            return View(compra);
        }

        // GET: Compras/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compra = await _context.Compras
                .Include(c => c.Carrito)
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(m => m.CompraID == id);
            if (compra == null)
            {
                return NotFound();
            }

            return View(compra);
        }

        // POST: Compras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var compra = await _context.Compras.FindAsync(id);
            _context.Compras.Remove(compra);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompraExists(Guid id)
        {
            return _context.Compras.Any(e => e.CompraID == id);
        }


        //get comprar

        public async Task<IActionResult> Comprar(Guid carritoid)
        {

            var carrito = await _context.Carrito
               .Include(c => c.CarritosItems)
                   .ThenInclude(ci => ci.Producto)
               .FirstOrDefaultAsync(m => m.CarritoId == carritoid);
            List<CarritoItem> rej = new List<CarritoItem>();






            var compra = new Compra();
            compra.CompraID = Guid.NewGuid();
            compra.CarritoId = carritoid;
            compra.Fecha = DateTime.Now;



            double tot = 0.00;
            tot = carrito.Subtotal + ((carrito.Subtotal * 10) / 100);

            compra.Total = tot;
            compra.UsuarioId = carrito.UsuarioID;
            compra.Usuario = carrito.Usuario;
            var carritoNuevo = new Carrito();
            carritoNuevo.CarritoId = Guid.NewGuid();
            carritoNuevo.CarritosItems = carrito.CarritosItems;
            carritoNuevo.Subtotal = carrito.Subtotal;
            carritoNuevo.Usuario = carrito.Usuario;
            carritoNuevo.UsuarioID = carrito.UsuarioID;

            compra.Carrito = carritoNuevo;
            compra.CarritoId = carritoNuevo.CarritoId;
            _context.Carrito.Add(carritoNuevo);
            _context.Compras.Add(compra);

            carrito.CarritosItems.Clear();
            await _context.SaveChangesAsync();


            





                return RedirectToAction("Details", "Compras", new { id = compra.CompraID });
            }
        }
}
