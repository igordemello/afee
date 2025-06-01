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


namespace tcc_in305b.Controllers
{
    public class EstrategiaController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;
        public EstrategiaController(IConfiguration configuration, UserManager<IdentityUser> userManager, ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }
        [Authorize(Roles = "Treinador, Player")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            IQueryable<Estrategia> estrategias = _context.Estrategias;

            if (User.IsInRole("Treinador"))
            {
                var treinador = await _context.Treinadores
                    .FirstOrDefaultAsync(t => t.UserId == userId);

                if (treinador != null)
                {
                    estrategias = estrategias.Where(e => e.TreinadorId == treinador.Id);


                    ViewBag.Grupos = await _context.Grupos
                        .Where(g => g.TreinadorId == treinador.Id)
                        .ToListAsync();
                }
            }
            else if (User.IsInRole("Player"))
            {
                var grupoId = await _context.Players
                    .Where(p => p.UserId == userId)
                    .Select(p => p.GrupoId)
                    .FirstOrDefaultAsync();

                estrategias = estrategias
                    .Where(e => _context.EstrategiaGrupos
                        .Where(eg => eg.GrupoId == grupoId)
                        .Select(eg => eg.EstrategiaId)
                        .Contains(e.Id));
            }

            var listaEstrategias = await estrategias.ToListAsync();

            return View(listaEstrategias);
        }


        [Authorize(Roles = "Treinador")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var estrategia = await _context.Estrategias.FindAsync(id);
            if (estrategia == null)
            {
                return NotFound();
            }

            _context.Estrategias.Remove(estrategia);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Roles = "Treinador")]
        [HttpGet]
        public async Task<IActionResult> Criar()
        {
            var user = await _userManager.GetUserAsync(User);
            var treinador = await _context.Treinadores
                .FirstOrDefaultAsync(t => t.UserId == user.Id);

            if (treinador == null)
            {
                return Unauthorized();
            }

            ViewBag.Grupos = await _context.Grupos
                .Where(g => g.TreinadorId == treinador.Id)
                .ToListAsync();

            return View();
        }

        [Authorize(Roles = "Treinador")]
        [HttpPost]
        public async Task<IActionResult> Criar(string Nome, string Descricao, List<int> Grupos)
        {
            var user = await _userManager.GetUserAsync(User);
            var treinador = await _context.Treinadores
                .FirstOrDefaultAsync(t => t.UserId == user.Id);

            if (treinador == null)
            {
                return Unauthorized();
            }

            var novaEstrategia = new Estrategia
            {
                Nome = Nome,
                Descrição = Descricao,
                TreinadorId = treinador.Id
            };

            _context.Estrategias.Add(novaEstrategia);
            await _context.SaveChangesAsync();


            if (Grupos != null && Grupos.Any())
            {
                var estrategiasGrupos = Grupos.Select(grupoId => new EstrategiaGrupo
                {
                    EstrategiaId = novaEstrategia.Id,
                    GrupoId = grupoId
                });

                _context.EstrategiaGrupos.AddRange(estrategiasGrupos);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Treinador")]
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var estrategia = await _context.Estrategias
                .Include(e => e.EstrategiaGrupos)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (estrategia == null)
            {
                return NotFound();
            }

            ViewBag.Grupos = await _context.Grupos
                .Where(g => g.TreinadorId == estrategia.TreinadorId)
                .ToListAsync();

            return View(estrategia);
        }

        [Authorize(Roles = "Treinador")]
        [HttpPost]
        public async Task<IActionResult> Editar(int Id, string Nome, string Descricao, List<int> Grupos)
        {
            var estrategia = await _context.Estrategias
                .Include(e => e.EstrategiaGrupos)
                .FirstOrDefaultAsync(e => e.Id == Id);

            if (estrategia == null)
            {
                return NotFound();
            }

            estrategia.Nome = Nome;
            estrategia.Descrição = Descricao;
            estrategia.EstrategiaGrupos.Clear();
            foreach (var grupoId in Grupos)
            {
                estrategia.EstrategiaGrupos.Add(new EstrategiaGrupo { EstrategiaId = Id, GrupoId = grupoId });
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


    }
}
