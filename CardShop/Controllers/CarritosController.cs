using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CardShop.Datos;
using CardShop.Models;
using Microsoft.AspNetCore.Authorization;

namespace CardShop.Controllers
{
    public class CarritosController : Controller
    {
        private readonly BaseDatos _context;

        public CarritosController(BaseDatos context)
        {
            _context = context;
        }

        // GET: Carritos
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Index()
        {
            var baseDatos = _context.Carrito.Include(c => c.Usuario);
            return View(await baseDatos.ToListAsync());
        }
        //GET:Carritos
        [Authorize(Roles = "USUARIO")]
        public async Task<IActionResult> CarritoUsuario(Guid Id)
        {
            var carrito = _context.Carrito.Include(n => n.CarritosItems)
                                            .ThenInclude(ci => ci.Producto)
                                          .Include(c => c.Usuario)
                                          .FirstOrDefaultAsync(n => n.UsuarioID == Id);
            return View(await carrito);
        }

        // GET: Carritos/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carrito = await _context.Carrito
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(m => m.CarritoId == id);
            if (carrito == null)
            {
                return NotFound();
            }

            return View(carrito);
        }

        // GET: Carritos/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["UsuarioID"] = new SelectList(_context.Usuario, "Id", "Id");
            return View();
        }

        // POST: Carritos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CarritoId,UsuarioID,Subtotal")] Carrito carrito)
        {
            if (ModelState.IsValid)
            {
                carrito.CarritoId = Guid.NewGuid();
                _context.Add(carrito);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UsuarioID"] = new SelectList(_context.Usuario, "Id", "Id", carrito.UsuarioID);
            return View(carrito);
        }
        // POST: Carritos/Agregar/5
        // Agrega un CarritoItem al Carrito
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Agregar(Producto producto, int Cantidad)
        {
            if (Cantidad >= 1)
            {
                var usuarioId = Guid.Parse(User.FindFirst("IdUsuario").Value);
                var usuario = await _context.Usuario.SingleOrDefaultAsync(u => u.Id == usuarioId);

                var carrito = await _context.Carrito.FirstOrDefaultAsync(p => p.UsuarioID == usuarioId );
                if(carrito==null){
                    
                    var CardShop = new Carrito();
                    CardShop.CarritoId = Guid.NewGuid();
                    CardShop.UsuarioID = usuarioId;
                    CardShop.Usuario = usuario;
                    CardShop.Subtotal = 0;
                    CardShop.CarritosItems = new List<CarritoItem>();

                    carrito = CardShop;
                    _context.Carrito.Add(carrito);
                    await _context.SaveChangesAsync();
                }

                var carritoItem = new CarritoItem();
                carritoItem.CarritoItemId = new Guid();
                carritoItem.Cantidad = Cantidad;
                carritoItem.ProductoId = producto.ProductoId;
                carritoItem.CarritoId = carrito.CarritoId;

                _context.CarritoItem.Add(carritoItem);

                carrito.CarritosItems.Add(carritoItem);
                carrito.Subtotal = carrito.Subtotal + (producto.PrecioVigente * Cantidad);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(CarritoUsuario), new { id = carrito.UsuarioID });
            }
            TempData["error"] = "Ingrese 1 o más productos";
            return RedirectToAction("Agregar", "Productos", new { id = producto.ProductoId });
        }
        // GET: Carritos/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carrito = await _context.Carrito.FindAsync(id);
            if (carrito == null)
            {
                return NotFound();
            }
            ViewData["UsuarioID"] = new SelectList(_context.Usuario, "Id", "Id", carrito.UsuarioID);
            return View(carrito);
        }

        // POST: Carritos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CarritoId,UsuarioID,Subtotal")] Carrito carrito)
        {
            if (id != carrito.CarritoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carrito);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarritoExists(carrito.CarritoId))
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
            ViewData["UsuarioID"] = new SelectList(_context.Usuario, "Id", "Id", carrito.UsuarioID);
            return View(carrito);
        }

        // GET: Carritos/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carrito = await _context.Carrito
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(m => m.CarritoId == id);
            if (carrito == null)
            {
                return NotFound();
            }

            return View(carrito);
        }

        // POST: Carritos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var carrito = await _context.Carrito.FindAsync(id);
            _context.Carrito.Remove(carrito);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarritoExists(Guid id)
        {
            return _context.Carrito.Any(e => e.CarritoId == id);
        }

        // GET: VACIAR EL CARRITO
        [Authorize]
        public async Task<IActionResult> Vaciar(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carrito = await _context.Carrito.Include(c => c.CarritosItems)
                .FirstOrDefaultAsync(m => m.CarritoId == id);
            if (carrito == null)
            {
                return NotFound();
            }
            carrito.CarritosItems.Clear();
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(CarritoUsuario), new { id = carrito.UsuarioID });
        }
    }
}
