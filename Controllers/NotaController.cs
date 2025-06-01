using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Data;
using tcc_in305b.Data;
using tcc_in305b.Models;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using tcc_in305b.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.RegularExpressions;

namespace tcc_in305b.Controllers
{
    public class NotaController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;

        public NotaController(IConfiguration configuration, UserManager<IdentityUser> userManager, ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        // GET: Nota
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var treinador = _context.Treinadores.FirstOrDefault(t => t.UserId == userId);

            var applicationDbContext = _context.Notas.Where(n=>n.TreinadorId == treinador.Id).Include(n => n.Treinador);
            return View(await applicationDbContext.ToListAsync());
        }

        

        // GET: Nota/Create
        public IActionResult Create()
        {
            
            return View();
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome")] Nota nota)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var treinador = await _context.Treinadores
                              .FirstOrDefaultAsync(t => t.UserId == userId);

            if (treinador == null)
            {
                return NotFound("Treinador associado ao usuário logado não encontrado.");
            }

            nota.TreinadorId = treinador.Id;

            ModelState.Remove("Treinador");


            if (ModelState.IsValid)
            {
                _context.Add(nota);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            return View(nota);
        }

        // GET: Nota/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nota = await _context.Notas.FindAsync(id);
            if (nota == null)
            {
                return NotFound();
            }
            ViewData["TreinadorId"] = new SelectList(_context.Treinadores, "Id", "Id", nota.TreinadorId);
            return View(nota);
        }

        // POST: Nota/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, TreinadorId, Nome")] Nota nota)
        {
            if (id != nota.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Treinador");

            if (ModelState.IsValid)
            {
                try
                {
                    var notaExistente = await _context.Notas.FindAsync(id);

                    if (notaExistente == null)
                    {
                        return NotFound();
                    }

                    notaExistente.Nome = nota.Nome;
                    notaExistente.TreinadorId = nota.TreinadorId;
                    
                    _context.Update(notaExistente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotaExists(nota.Id))
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

            return View(nota);
        }

        // GET: Nota/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nota = await _context.Notas
                .Include(n => n.Treinador)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (nota == null)
            {
                return NotFound();
            }

            return View(nota);
        }

        // POST: Nota/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nota = await _context.Notas.FindAsync(id);
            if (nota != null)
            {
                _context.Notas.Remove(nota);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotaExists(int id)
        {
            return _context.Notas.Any(e => e.Id == id);
        }
    }
}
