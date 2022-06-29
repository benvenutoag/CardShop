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
        

        //GET: Carritos/Compras/5
        [Authorize(Roles = "USUARIO")]
        public async Task<IActionResult> Compras(Guid Id)
        {
            var carrito = _context.Carrito.Include(n => n.CarritosItems)
                                .ThenInclude(ci => ci.Producto)
                              .Include(c => c.Usuario)
                              .FirstOrDefaultAsync(n => n.UsuarioID == Id);

            return View(await carrito);
        }

        //GET: Carritos/Compras/5
        [Authorize(Roles = "USUARIO")]
        public async Task<IActionResult> CompraRealizada(Guid Id)
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

                var carrito = await _context.Carrito.FirstOrDefaultAsync(p => p.UsuarioID == usuarioId);

                if (carrito == null)
                {

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

                var precioProducto = await _context.Producto.SingleOrDefaultAsync(p => p.ProductoId == carritoItem.ProductoId);


                carrito.CarritosItems.Add(carritoItem);

                carrito.Subtotal += carritoItem.Cantidad * precioProducto.PrecioVigente;

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

        // GET: Vaciar
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
            carrito.Subtotal = 0;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(CarritoUsuario), new { id = carrito.UsuarioID });
        }


        public async Task<IActionResult> Cerrar(Guid id)
        {
            var carrito = await _context.Carrito.Include(c => c.CarritosItems)
                .FirstOrDefaultAsync(m => m.CarritoId == id);
            if (carrito == null)
            {
                return NotFound();
            }



            if (carrito.Subtotal <= 0)
            {
                return NotFound();
            }
            // se borra la lista de carritos y se crea uno nuevo despues de hacer la compra (esta mal porque si volves para atras lo borra pero mantiene el subtotal)

            //var carritoNuevo = new Carrito();
            //carritoNuevo.CarritoId = Guid.NewGuid();
            //carritoNuevo.UsuarioID = carrito.UsuarioID;
            //carritoNuevo.Subtotal = 0;
            carrito.CarritosItems.Clear();
            carrito.Subtotal = 0;

            //_context.Carrito.Add(carritoNuevo);


            await _context.SaveChangesAsync();



            return RedirectToAction(nameof(CompraRealizada), new { id = carrito.UsuarioID });
        }

        [HttpGet]
        public async Task<IActionResult> PrepararCompra(Guid id)
        {
            var carrito = await _context.Carrito.Include(c => c.CarritosItems)
                .FirstOrDefaultAsync(m => m.CarritoId == id);
            if (carrito == null)
            {
                return NotFound();
            }


            if (carrito.Subtotal <= 0)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Compras), new { id = carrito.UsuarioID });

        }

       // GET: Carritos/Edit/5
        [Authorize]
        public async Task<IActionResult> EditarCantidad(Guid? id, int Cantidad)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carritoItem = await _context.CarritoItem.FirstOrDefaultAsync(n => n.CarritoItemId == id);
            if (carritoItem == null)
            {
                return NotFound();
            }
            ViewData["Cantidad"] = Cantidad;

            return View(carritoItem);
        }
        // POST: Carritos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> EditarCantidad (Guid id, CarritoItem carritoItem)
        {
            if (id != carritoItem.CarritoItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carritoItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarritoExists(carritoItem.CarritoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                var carrito = await _context.Carrito
                    .FirstOrDefaultAsync(m => m.CarritoId == carritoItem.CarritoId);
                return RedirectToAction(nameof(CarritoUsuario), new { id = carrito.UsuarioID });
            }

            return View(id);
        }

        // GET: Carritos/Delete/5
        [Authorize]
        public async Task<IActionResult> BorrarItem(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carritoItem = await _context.CarritoItem
                .Include(c => c.Producto)
                .Include(c => c.Carrito)
                .FirstOrDefaultAsync(m => m.CarritoItemId == id);
            if (carritoItem == null)
            {
                return NotFound();
            }

            return View(carritoItem);
        }

        // POST: Carritos/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> BorrarItem(Guid id)
        {
            var carritoItem = await _context.CarritoItem
                  .FirstOrDefaultAsync(m => m.CarritoItemId == id);
            var carrito = await _context.Carrito
                .FirstOrDefaultAsync(m => m.CarritoId == carritoItem.CarritoId);
            carrito.CarritosItems.Remove(carritoItem);
            _context.CarritoItem.Remove(carritoItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(CarritoUsuario), new { id = carrito.UsuarioID });
        }
    }
}
