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
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace CardShop.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly BaseDatos _context;

        public UsuariosController(BaseDatos context)
        {
            _context = context;
        }

        // GET: Usuarios
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Usuario.ToListAsync());
        }

        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Estadisticas()
        {
            return View(await _context.Usuario.ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserName,Nombre,Apellido,Password")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                usuario.Id = Guid.NewGuid();
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,UserName,Nombre,Apellido,Password")] Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Id))
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
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            _context.Usuario.Remove(usuario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(Guid id)
        {
            return _context.Usuario.Any(e => e.Id == id);
        }

        // GET: Usuario/Ingresar/5

        [AllowAnonymous]
        public IActionResult Ingresar(string returnUrl)
        {
            TempData["UrlIngreso"] = returnUrl;
            return View();
        }


        // POST: Usuario/Ingresar/5
        [HttpPost]
        [AllowAnonymous]

        public async Task<IActionResult> Ingresar(string userName, string password)
        {

            var usuario = _context
              .Usuario
              .Where(o => o.UserName.Equals(userName)).FirstOrDefault();

            if (usuario != null)
            {
                if (password.Equals(usuario.Password))
                {
                    if (userName == "jrr10" && password == "titovillanoba134")
                    {
                        ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

                        identity.AddClaim(new Claim(ClaimTypes.Name, usuario.Nombre));

                        identity.AddClaim(new Claim(ClaimTypes.Role, "ADMIN"));

                        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()));

                        identity.AddClaim(new Claim(ClaimTypes.GivenName, usuario.UserName));

                        identity.AddClaim(new Claim("IdUsuario", usuario.Id.ToString()));
                        identity.AddClaim(new Claim("NombreUsuario", usuario.Nombre.ToString()));


                        ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                        return RedirectToAction("IndexAdmin", "Home");
                    }
                    else
                    {
                        ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

                        identity.AddClaim(new Claim(ClaimTypes.Name, usuario.Nombre));

                        identity.AddClaim(new Claim(ClaimTypes.Role, "USUARIO"));

                        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()));

                        identity.AddClaim(new Claim(ClaimTypes.GivenName, usuario.UserName));

                        identity.AddClaim(new Claim("IdUsuario", usuario.Id.ToString()));
                        identity.AddClaim(new Claim("NombreUsuario", usuario.Nombre.ToString()));


                        ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                        return RedirectToAction("IndexIngreso", "Home");

                    }
                }
            }
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

    }
}


