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
    public class FaseController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;
        public FaseController(IConfiguration configuration, UserManager<IdentityUser> userManager, ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            List<FaseViewModel> faseDetalhes = new List<FaseViewModel>();

            if (User.IsInRole("Treinador"))
            {
                var treinador = await _context.Treinadores.FirstOrDefaultAsync(t => t.UserId == userId);

                var faseTreinadores = await _context.FaseTreinadores
                    .Where(ft => ft.TreinadorId == treinador.Id)
                    .Include(ft => ft.Fase)
                    .Include(ft => ft.Treinador)
                    .ToListAsync();

                faseDetalhes = faseTreinadores
                    .GroupBy(ft => ft.Fase)
                    .Select(g => new FaseViewModel
                    {
                        FaseId = g.Key.Id,
                        FaseNome = g.Key.Nome,
                        Status = g.Key.Status,
                        TreinadoresNomes = g.Select(ft => ft.Treinador.Nome).ToList()
                    })
                    .ToList();
            }
            else if (User.IsInRole("Admin"))
            {
                var faseTreinadores = await _context.FaseTreinadores
                    .Include(ft => ft.Fase)
                    .Include(ft => ft.Treinador)
                    .ToListAsync();

                faseDetalhes = faseTreinadores
                    .GroupBy(ft => ft.Fase)
                    .Select(g => new FaseViewModel
                    {
                        FaseId = g.Key.Id,
                        FaseNome = g.Key.Nome,
                        Status = g.Key.Status,
                        TreinadoresNomes = g.Select(ft => ft.Treinador.Nome).ToList()
                    })
                    .ToList();
            }

            var viewModel = new FaseTreinadorViewModel
            {
                FaseDetalhes = faseDetalhes
            };

            return View(viewModel);
        }
        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new CreateFaseViewModel
            {
                Treinadores = _context.Treinadores
                    .Select(t => new SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.Nome
                    })
                    .ToList()
            };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateFaseViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var treinador = await _context.Treinadores
                  .FirstOrDefaultAsync(t => t.UserId == userId);

                var novaFase = new Fase
                {
                    Nome = model.Nome,
                    Status = model.Status
                };

                _context.Fases.Add(novaFase);
                await _context.SaveChangesAsync();

                if (User.IsInRole("Admin")) { 
                    if (model.TreinadorIds != null && model.TreinadorIds.Any())
                    {
                        foreach (var treinadorId in model.TreinadorIds)
                        {
                            _context.FaseTreinadores.Add(new FaseTreinador
                            {
                                FaseId = novaFase.Id,
                                TreinadorId = treinadorId
                            });
                        }

                        await _context.SaveChangesAsync();
                    }
                }
                else{
                    Console.WriteLine("Entrou no else");
                    _context.FaseTreinadores.Add(new FaseTreinador
                    {
                        FaseId = novaFase.Id,
                        TreinadorId = treinador.Id
                    });
                    Console.WriteLine("Fase Id: "+novaFase.Id+" Treinador Id: "+treinador.Id);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Index");
            }

            model.Treinadores = _context.Treinadores
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Nome
                })
                .ToList();

            return View(model);
        }
        public IActionResult Detalhes(int id)
        {
            var fase = _context.Fases.Include(f => f.Players).FirstOrDefault(f => f.Id == id);
            if (fase == null)
            {
                return NotFound();
            }

            var treinadores = _context.FaseTreinadores
                .Where(ft => ft.FaseId == id)
                .Select(ft => ft.Treinador.Nome)
                .ToList();

            var players = _context.Players
                .Where(p => p.FaseId == id)
                .Select(p => new PlayerViewModel
                {
                    Id = p.Id,
                    Nome = p.Nome
                })
                .ToList();

            var viewModel = new FaseDetalhesViewModel
            {
                Id = fase.Id,
                Nome = fase.Nome,
                Status = fase.Status,
                Treinadores = treinadores,
                Players = players
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int faseId)
        {
            var userId = _userManager.GetUserId(User);

            if (User.IsInRole("Admin"))
            {
                var fase = await _context.Fases
                    .Include(f => f.FaseTreinadores)
                    .FirstOrDefaultAsync(f => f.Id == faseId);

                _context.FaseTreinadores.RemoveRange(fase.FaseTreinadores);

                _context.Fases.Remove(fase);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Fase excluída com sucesso.";
            }
            else if (User.IsInRole("Treinador"))
            {
                var treinador = await _context.Treinadores.FirstOrDefaultAsync(t => t.UserId == userId);

                var faseTreinador = await _context.FaseTreinadores
                    .FirstOrDefaultAsync(ft => ft.FaseId == faseId && ft.TreinadorId == treinador.Id);

                _context.FaseTreinadores.Remove(faseTreinador);

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Relacionamento removido com sucesso.";
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var fase = _context.Fases.Include(f => f.FaseTreinadores).FirstOrDefault(f => f.Id == id);

            if (fase == null)
            {
                return NotFound();
            }

            var viewModel = new EditFaseViewModel
            {
                Id = fase.Id,
                Nome = fase.Nome,
                Status = fase.Status,
                TreinadorIds = fase.FaseTreinadores.Select(ft => ft.TreinadorId).ToList(),
                Treinadores = _context.Treinadores
                    .Select(t => new SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.Nome
                    })
                    .ToList()
            };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditFaseViewModel model)
        {
            if (ModelState.IsValid)
            {
                var fase = await _context.Fases.Include(f => f.FaseTreinadores).FirstOrDefaultAsync(f => f.Id == model.Id);

                if (fase == null)
                {
                    return NotFound();
                }
                fase.Nome = model.Nome;
                fase.Status = model.Status;
                var treinadoresExistentes = fase.FaseTreinadores.Select(ft => ft.TreinadorId).ToList();
                var treinadoresParaAdicionar = model.TreinadorIds.Except(treinadoresExistentes).ToList();
                var treinadoresParaRemover = treinadoresExistentes.Except(model.TreinadorIds).ToList();

                foreach (var treinadorId in treinadoresParaAdicionar)
                {
                    _context.FaseTreinadores.Add(new FaseTreinador
                    {
                        FaseId = fase.Id,
                        TreinadorId = treinadorId
                    });
                }

                var faseTreinadoresParaRemover = _context.FaseTreinadores
                    .Where(ft => ft.FaseId == fase.Id && treinadoresParaRemover.Contains(ft.TreinadorId));
                _context.FaseTreinadores.RemoveRange(faseTreinadoresParaRemover);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Fase atualizada com sucesso.";
                return RedirectToAction("Index");
            }
            model.Treinadores = _context.Treinadores
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Nome
                })
                .ToList();

            return View(model);
        }


        public IActionResult AdicionarPlayer(int faseId)
        {
            var jogadoresDisponiveis = _context.Players
                .Where(p => p.FaseId != faseId)
                .ToList();

            var viewModel = new AdicionarPlayerViewModel
            {
                FaseId = faseId,
                Jogadores = jogadoresDisponiveis
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AdicionarPlayer(int faseId, int playerId)
        {
            var player = _context.Players.Find(playerId);
            if (player != null)
            {
                player.FaseId = faseId;
                _context.SaveChanges();
            }
            return RedirectToAction("Detalhes", new { id = faseId });
        }


        [HttpPost]
        public IActionResult RemoverPlayer(int faseId, int playerId)
        {
            var player = _context.Players.FirstOrDefault(p => p.Id == playerId && p.FaseId == faseId);
            if (player != null)
            {
                player.FaseId = null; 
                _context.SaveChanges();
            }
            return RedirectToAction("Detalhes", new { id = faseId });
        }

    }
}
