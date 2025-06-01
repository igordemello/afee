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
    [Authorize(Roles = "Treinador")]
    public class GrupoController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;
        public GrupoController(IConfiguration configuration, UserManager<IdentityUser> userManager, ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var treinadorid = await _context.Treinadores.Where(t => t.UserId == userId).Select(t => t.Id).FirstOrDefaultAsync();

            var grupos = await _context.Grupos
                                       .Where(g => g.TreinadorId == treinadorid)
                                       .Include(g => g.Treinador)
                                       .ToListAsync();
            return View(grupos);
        }



        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grupo = await _context.Grupos
                                      .Include(g => g.Treinador)
                                      .FirstOrDefaultAsync(m => m.Id == id);

            if (grupo == null)
            {
                return NotFound();
            }

            var players = await _context.Players
                             .Where(p => p.GrupoId == grupo.Id)
                             .ToListAsync();

            ViewBag.Players = players;

            

            return View(grupo);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,Status,Tipo")] Grupo grupo)
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

            grupo.TreinadorId = treinador.Id;

            grupo.DtCriacao = DateTime.Now;

            ModelState.Remove("Treinador");

            if (ModelState.IsValid)
            {
                _context.Add(grupo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(grupo);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grupo = await _context.Grupos.FindAsync(id);
            if (grupo == null)
            {
                return NotFound();
            }
            return View(grupo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Status,Tipo")] Grupo grupo)
        {
            if (id != grupo.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Treinador");

            if (ModelState.IsValid)
            {
                try
                {
                    var grupoExistente = await _context.Grupos.FindAsync(id);

                    if (grupoExistente == null)
                    {
                        return NotFound();
                    }

                    grupoExistente.Nome = grupo.Nome;
                    grupoExistente.Status = grupo.Status;
                    grupoExistente.Tipo = grupo.Tipo;

                    _context.Update(grupoExistente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GrupoExists(grupo.Id))
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
            return View(grupo);
        }

        private bool GrupoExists(int id)
        {
            return _context.Grupos.Any(e => e.Id == id);
        }



        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grupo = await _context.Grupos
                .Include(g => g.Treinador).FirstOrDefaultAsync(m => m.Id == id);

            if (grupo == null)
            {
                return NotFound();
            }

            return View(grupo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var grupo = await _context.Grupos.FindAsync(id);
            if (grupo == null)
            {
                return NotFound();
            }

            var players = _context.Players.Where(p => p.GrupoId == id).ToList();
            foreach (var player in players)
            {
                player.GrupoId = null; _context.Update(player);
            }

            _context.Grupos.Remove(grupo);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }




        [HttpGet]
        [Authorize(Roles = "Treinador")]
        public async Task<IActionResult> TrocarJogadores()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var treinadorid = await _context.Treinadores.Where(t => t.UserId == userId).Select(t => t.Id).FirstOrDefaultAsync();

            var jogadores = await _context.Players.Where(p => p.TreinadorId == treinadorid).ToListAsync();
            ViewBag.Jogadores = new SelectList(jogadores, "Nome", "Nome");


            var grupos = await _context.Grupos.Where(g => g.TreinadorId == treinadorid).ToListAsync();
            ViewBag.Grupos = new SelectList(grupos, "Id", "Nome");

            var grupoPlayers = new Dictionary<int, List<Player>>();


            foreach (var treinoGrupo in grupos)
            {
                var players = await _context.Players
                    .Where(p => p.GrupoId == treinoGrupo.Id)
                    .ToListAsync();
                grupoPlayers.Add(treinoGrupo.Id, players);
            }


            ViewData["GrupoPlayers"] = grupoPlayers;

            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Treinador")]
        public async Task<IActionResult> AlterarJogador()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var treinadorid = await _context.Treinadores.Where(t => t.UserId == userId).Select(t => t.Id).FirstOrDefaultAsync();

            var jogadores = await _context.Players.Where(p => p.TreinadorId == treinadorid).ToListAsync();
            ViewBag.Jogadores = new SelectList(jogadores, "Nome", "Nome");


            var grupos = await _context.Grupos.Where(g => g.TreinadorId == treinadorid).ToListAsync();
            ViewBag.Grupos = new SelectList(grupos, "Id", "Nome");

            var grupoPlayers = new Dictionary<int, List<Player>>();


            foreach (var treinoGrupo in grupos)
            {
                var players = await _context.Players
                    .Where(p => p.GrupoId == treinoGrupo.Id)
                    .ToListAsync();
                grupoPlayers.Add(treinoGrupo.Id, players);
            }


            ViewData["GrupoPlayers"] = grupoPlayers;


            return View();
        }


        [HttpPost]
        [Authorize(Roles = "Treinador")]
        public async Task<IActionResult> TrocarJogadores(string player1Nome, string player2Nome)
        {
            var player1 = await _context.Players.FirstOrDefaultAsync(p => p.Nome == player1Nome);
            var player2 = await _context.Players.FirstOrDefaultAsync(p => p.Nome == player2Nome);

            if (player1 == null || player2 == null)
            {
                return NotFound("Um ou ambos os jogadores não foram encontrados.");
            }

            var tempGrupoId = player1.GrupoId;
            player1.GrupoId = player2.GrupoId;
            player2.GrupoId = tempGrupoId;

            _context.Update(player1);
            _context.Update(player2);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        [HttpPost]
        [Authorize(Roles = "Treinador")]
        public async Task<IActionResult> AlterarJogador(string playerToMoveNome, string playerToReplaceNome)
        {
            var playerToMove = await _context.Players.FirstOrDefaultAsync(p => p.Nome == playerToMoveNome);
            var playerToReplace = await _context.Players.FirstOrDefaultAsync(p => p.Nome == playerToReplaceNome);

            if (playerToMove == null || playerToReplace == null)
            {
                return NotFound("Um ou ambos os jogadores não foram encontrados.");
            }

            playerToMove.GrupoId = playerToReplace.GrupoId;
            playerToReplace.GrupoId = null;
            _context.Update(playerToMove);
            _context.Update(playerToReplace);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        [Authorize(Roles = "Treinador")]
        public async Task<IActionResult> AdicionarJogadorLivre()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var treinadorid = await _context.Treinadores.Where(t => t.UserId == userId).Select(t => t.Id).FirstOrDefaultAsync();
            var jogadoresLivres = await _context.Players
                                     .Where(p => p.GrupoId == null && p.TreinadorId == treinadorid)
                                     .ToListAsync();

            ViewBag.JogadoresLivres = new SelectList(jogadoresLivres, "Id", "Nome");

            var grupos = await _context.Grupos.Where(g => g.TreinadorId == treinadorid).ToListAsync();
            ViewBag.Grupos = new SelectList(grupos, "Id", "Nome");


            var grupoPlayers = new Dictionary<int, List<Player>>();


            foreach (var treinoGrupo in grupos)
            {
                var players = await _context.Players
                    .Where(p => p.GrupoId == treinoGrupo.Id)
                    .ToListAsync();
                grupoPlayers.Add(treinoGrupo.Id, players);
            }


            ViewData["GrupoPlayers"] = grupoPlayers;

            return View();
        }


        [HttpPost]
        [Authorize(Roles = "Treinador")]
        public async Task<IActionResult> AdicionarJogadorLivre(int jogadorId, int grupoId)
        {
            var jogador = await _context.Players.Include(p => p.Grupo).FirstOrDefaultAsync(p => p.Id == jogadorId);

            if (jogador == null)
            {
                return NotFound("Jogador não encontrado.");
            }

            var grupo = await _context.Grupos.FindAsync(grupoId);
            if (grupo == null)
            {
                return NotFound("Grupo não encontrado.");
            }


            jogador.GrupoId = grupoId;

            _context.Update(jogador);
            await _context.SaveChangesAsync();

            var userlogado = _userManager.GetUserId(User);
            var treinador = _context.Treinadores
                .Where(t => t.UserId == userlogado)
                .FirstOrDefault();

            var historico = new PlayerHistorico
            {
                PlayerId = jogador.Id,
                DataEvento = DateTime.Now,
                TipoEvento = "Atualização no Grupo",
                Descricao = $"Grupo do Player '{jogador.Nome}' foi alterado por '{treinador.Nome}'",
                DadosAlterados = $"'{jogador.Nome}' foi adicionado no grupo '{jogador.Grupo.Nome}'"
            };


            _context.Update(historico);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> DetalharJogador(int id)
        {
            var player = await _context.Players.FirstOrDefaultAsync(p => p.Id == id);
            var jogador = player; return Json(jogador);
        }

        [HttpGet]
        [Authorize(Roles = "Treinador")]
        public async Task<IActionResult> RemoverJogador()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var treinadorid = await _context.Treinadores.Where(t => t.UserId == userId).Select(t => t.Id).FirstOrDefaultAsync();

            var jogadores = await _context.Players.Where(p => p.TreinadorId == treinadorid && p.GrupoId != null).ToListAsync();
            ViewBag.Jogadores = new SelectList(jogadores, "Nome", "Nome");


            var grupos = await _context.Grupos.Where(g => g.TreinadorId == treinadorid).ToListAsync();
            ViewBag.Grupos = new SelectList(grupos, "Id", "Nome");

            var grupoPlayers = new Dictionary<int, List<Player>>();


            foreach (var treinoGrupo in grupos)
            {
                var players = await _context.Players
                    .Where(p => p.GrupoId == treinoGrupo.Id)
                    .ToListAsync();
                grupoPlayers.Add(treinoGrupo.Id, players);
            }


            ViewData["GrupoPlayers"] = grupoPlayers;


            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Treinador")]
        public async Task<IActionResult> RemoverJogador(string playerToRemoveNome)
        {
            var playerToRemove = await _context.Players.Include(p => p.Grupo).FirstOrDefaultAsync(p => p.Nome == playerToRemoveNome);
           

            if (playerToRemove == null)
            {
                return NotFound("Um ou ambos os jogadores não foram encontrados.");
            }

            var userlogado = _userManager.GetUserId(User);
            var treinador = _context.Treinadores
                .Where(t => t.UserId == userlogado)
                .FirstOrDefault();

            var historico = new PlayerHistorico
            {
                PlayerId = playerToRemove.Id,
                DataEvento = DateTime.Now,
                TipoEvento = "Atualização no Grupo",
                Descricao = $"Grupo do Player '{playerToRemove.Nome}' foi alterado por '{treinador.Nome}'",
                DadosAlterados = $"'{playerToRemove.Nome}' foi removido do grupo '{playerToRemove.Grupo.Nome}'"
            };


            playerToRemove.GrupoId = null;


            _context.Update(historico);
            _context.Update(playerToRemove);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
