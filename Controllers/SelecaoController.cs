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
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Immutable;


namespace tcc_in305b.Controllers
{
    public class SelecaoController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;
        public SelecaoController(IConfiguration configuration, UserManager<IdentityUser> userManager, ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        

        public async Task<IActionResult> Index()
        {
            var selecoesAtivas = await _context.Selecoes
                .Include(s => s.Treinador)
                .Where(s => s.DataFim == null)
                .ToListAsync();

            var selecoesFinalizadas = await _context.Selecoes
                .Include(s => s.Treinador)
                .Where(s => s.DataFim != null)
                .ToListAsync();

            var viewModel = new SelecaoIndexViewModel
            {
                SelecoesAtivas = selecoesAtivas,
                SelecoesFinalizadas = selecoesFinalizadas
            };

            return View(viewModel);
        }

        public class SelecaoIndexViewModel
        {
            public List<Selecao> SelecoesAtivas { get; set; }
            public List<Selecao> SelecoesFinalizadas { get; set; }
        }

        // GET: Selecao/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var treinadorid = await _context.Treinadores.Where(t => t.UserId == userId).Select(t => t.Id).FirstOrDefaultAsync();
            if (id == null)
            {
                return NotFound();
            }

            var selecao = await _context.Selecoes
                .Include(s => s.Treinador)
                .Include(s => s.Rounds)
                    .ThenInclude(r => r.Partidas)
                        .ThenInclude(p => p.EquipePartidas)
                            .ThenInclude(ep => ep.Equipe)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (selecao == null)
            {
                return NotFound();
            }

            var players = await _context.Players
            .Where(p => p.SelecaoPlayers
            .Any(sp => sp.SelecaoId == id))
            .ToListAsync();

            ViewBag.Players = players;

            selecao.Rounds = selecao.Rounds.OrderBy(r => r.Data).ToList();

            var equipes = await _context.Equipes
            .Where(e => e.SelecaoId == id)
            .Distinct()
            .ToListAsync();
            ViewBag.Equipes = equipes;


            var playersEquipe = await _context.Players
                .Where(p => p.SelecaoPlayers != null && p.SelecaoPlayers.Any(sp => sp.SelecaoId == selecao.Id))
                .ToListAsync();

            if (playersEquipe == null || !playersEquipe.Any())
            {
                _logger.LogWarning("Nenhum jogador encontrado para a seleção {SelecaoId}.", selecao.Id);
            }

            ViewBag.PlayersEquipe = playersEquipe;



            var jogadoresCandidatos = await _context.Players
            .Where(p => p.FgCandidato == true && p.TreinadorId == treinadorid && !p.SelecaoPlayers
            .Any(sp => sp.SelecaoId == selecao.Id))
            .ToListAsync();

            ViewBag.PlayersCan = jogadoresCandidatos;

            return View(selecao);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,DataInicio,Convidado")] Selecao selecao)
        {
            

            if (selecao.DataInicio < DateTime.Now.Date)
            {
                TempData["ErrorMessage"] = "A data de início deve ser maior ou igual à data de hoje.";
                return RedirectToAction(nameof(Index));
            }


            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);


                if (userId == null)
                {
                    Console.WriteLine("Usuário não está logado.");
                    return RedirectToAction(nameof(Index));
                }

                var treinador = _context.Treinadores.FirstOrDefault(t => t.UserId == userId);

                if (treinador == null)
                {
                    Console.WriteLine("Nenhum treinador encontrado para o usuário logado.");
                    return RedirectToAction(nameof(Index));
                }

                selecao.TreinadorId = treinador.Id;
                selecao.DataFim = null;
                _context.Add(selecao);
                await _context.SaveChangesAsync();
                Console.WriteLine("Seleção criada com sucesso.");
                TempData["SuccessMessage"] = "Seleção criada com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            Console.WriteLine("ModelState inválido. Erros: " + string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))));
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> AddRound(int selecaoId, DateTime data, List<int>? equipesSelecionadas)
        {
            var selecao = await _context.Selecoes
                .FirstOrDefaultAsync(s => s.Id == selecaoId);

            if (selecao == null)
            {
                TempData["ErrorMessage"] = "Seleção não encontrada.";
                return RedirectToAction("Index");
            }
            var equipesParticipantes = equipesSelecionadas?.Any() == true
                ? await _context.Equipes.Where(e => equipesSelecionadas.Contains(e.Id)).ToListAsync()
                : await _context.Equipes.Where(e => e.SelecaoId == selecao.Id).ToListAsync();
            var random = new Random();
            equipesParticipantes = equipesParticipantes.OrderBy(e => random.Next()).ToList();

            if (equipesParticipantes.Count < 2)
            {
                TempData["ErrorMessage"] = "Não há equipes suficientes para criar um round.";
                return RedirectToAction("Details", new { id = selecaoId });
            }

            if (equipesParticipantes.Count % 2 != 0)
            {
                TempData["ErrorMessage"] = "Não é possível criar um round com equipes ímpares.";
                return RedirectToAction("Details", new { id = selecaoId });
            }

            var round = new Round
            {
                SelecaoId = selecao.Id,
                Data = data
            };
            if (round.Data < selecao.DataInicio)
            {
                TempData["ErrorMessage"] = "Não é possível criar um round com data menor que data de inicio.";
                return RedirectToAction("Details", new { id = selecaoId });
            }
            _context.Rounds.Add(round);
            await _context.SaveChangesAsync();

            for (int i = 0; i < equipesParticipantes.Count; i += 2)
            {
                var partida = new Partida
                {
                    RoundId = round.Id,
                    Data = data,
                    Tipo = "Seleção"
                };

                _context.Partidas.Add(partida);

                var equipePartida1 = new EquipePartida
                {
                    EquipeId = equipesParticipantes[i].Id,
                    Partida = partida
                };

                var equipePartida2 = new EquipePartida
                {
                    EquipeId = equipesParticipantes[i + 1].Id,
                    Partida = partida
                };

                _context.EquipePartidas.Add(equipePartida1);
                _context.EquipePartidas.Add(equipePartida2);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Round adicionado com sucesso!";
            return RedirectToAction("Details", new { id = selecaoId });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRound(int roundId, int selecaoId)
        {
            var round = await _context.Rounds.FirstOrDefaultAsync(r => r.Id == roundId);
            if (round == null)
            {
                TempData["ErrorMessage"] = "Round não encontrado.";
                return RedirectToAction(nameof(Details), new { id = selecaoId });
            }

            _context.Rounds.Remove(round);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Round excluído com sucesso!";
            return RedirectToAction(nameof(Details), new { id = selecaoId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEquipe([Bind("Nome")] Equipe equipe, int selecaoId)
        {
            var userId = _userManager.GetUserId(User);
            var selecao = await _context.Selecoes
                .FirstOrDefaultAsync(s => s.Id == selecaoId);
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Usuário não está logado.";
                return RedirectToAction(nameof(Details), new { id = selecaoId });
            }

            var treinador = await _context.Treinadores.FirstOrDefaultAsync(t => t.UserId == userId);

            if (treinador == null)
            {
                TempData["ErrorMessage"] = "Nenhum treinador encontrado para o usuário logado.";
                return RedirectToAction(nameof(Details), new { id = selecaoId });
            }

            equipe.TreinadorId = treinador.Id;
            equipe.Tipo = "Seleção";
            equipe.Status = "Ativo";
            equipe.DtCriacao = DateTime.Now;
            equipe.SelecaoId = selecaoId;

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                Console.WriteLine("Erros de validação: " + string.Join(", ", errors));
                TempData["ErrorMessage"] = "Erro de validação ao criar equipe: " + string.Join(", ", errors);
                return RedirectToAction(nameof(Details), new { id = selecaoId });
            }

            try
            {
                _context.Equipes.Add(equipe);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Equipe criada com sucesso!";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao salvar equipe: " + ex.Message);
                TempData["ErrorMessage"] = "Erro ao salvar equipe. Tente novamente.";
            }

            return RedirectToAction(nameof(Details), new { id = selecaoId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FinalizarSelecao(int selecaoId, List<int> selectedPlayers)
        {
            var selecao = await _context.Selecoes
                .Include(s => s.Equipes)
                .Include(s => s.SelecaoPlayers)
                .ThenInclude(sp => sp.Player)
                .FirstOrDefaultAsync(s => s.Id == selecaoId);

            if (selecao == null)
            {
                TempData["ErrorMessage"] = "Seleção não encontrada.";
                return RedirectToAction("Details", new { id = selecaoId });
            }

            // Atualizar a data de término da seleção
            selecao.DataFim = DateTime.Today;

            if (selectedPlayers != null && selectedPlayers.Any())
            {
                // Apenas jogadores selecionados terão FgCandidato ajustado
                var jogadores = await _context.Players
                    .Where(p => selectedPlayers.Contains(p.Id))
                    .ToListAsync();
                

                foreach (var jogador in jogadores)
                {
                    jogador.FgCandidato = null; // Remover a candidatura dos jogadores efetivados
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Seleção finalizada com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro ao finalizar seleção: {ex.Message}";
            }

            return RedirectToAction("Details", new { id = selecaoId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPlayersToSelecao(int selecaoId, List<int> selectedPlayers)
        {
            var selecao = await _context.Selecoes
                .Include(s => s.SelecaoPlayers)
                .FirstOrDefaultAsync(s => s.Id == selecaoId);

            if (selecao == null)
            {
                TempData["ErrorMessage"] = "Seleção não encontrada.";
                return RedirectToAction("Details", new { id = selecaoId });
            }

            if (selectedPlayers == null || !selectedPlayers.Any())
            {
                TempData["ErrorMessage"] = "Nenhum jogador selecionado.";
                return RedirectToAction("Details", new { id = selecaoId });
            }

            var jogadores = await _context.Players
                .Where(p => selectedPlayers.Contains(p.Id) && p.FgCandidato == true)
                .ToListAsync();

            foreach (var jogador in jogadores)
            {
                selecao.SelecaoPlayers.Add(new SelecaoPlayer
                {
                    SelecaoId = selecao.Id,
                    PlayerId = jogador.Id
                });
            }

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Jogadores adicionados à seleção com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro ao adicionar jogadores: {ex.Message}";
            }

            return RedirectToAction("Details", new { id = selecaoId });
        }


        //AÇÕES DE EQUIPE:



        [HttpGet]
        [Authorize(Roles = "Treinador")]
        public async Task<IActionResult> TrocarJogadores(int selecaoId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var treinadorid = await _context.Treinadores.Where(t => t.UserId == userId).Select(t => t.Id).FirstOrDefaultAsync();

            var jogadores = await _context.Players
                .Where(p => p.TreinadorId == treinadorid && p.FgCandidato == true)
                .Where(p => p.SelecaoPlayers.Any(sp => sp.SelecaoId == selecaoId))
                .ToListAsync();
            ViewBag.Jogadores = new SelectList(jogadores, "Nome", "Nome");
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

            var tempEquipeId = player1.EquipeId;
            player1.EquipeId = player2.EquipeId;
            player2.EquipeId = tempEquipeId;

            _context.Update(player1);
            _context.Update(player2);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }




        [HttpGet]
        [Authorize(Roles = "Treinador")]
        public async Task<IActionResult> AlterarJogador(int selecaoId)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var treinadorid = await _context.Treinadores.Where(t => t.UserId == userId).Select(t => t.Id).FirstOrDefaultAsync();

            var jogadores = await _context.Players
                .Where(p => p.TreinadorId == treinadorid && p.FgCandidato == true)
                .Where(p => p.SelecaoPlayers.Any(sp => sp.SelecaoId == selecaoId))
                .ToListAsync();
            ViewBag.Jogadores = new SelectList(jogadores, "Nome", "Nome");
            return View();
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

            playerToMove.EquipeId = playerToReplace.EquipeId;
            playerToReplace.EquipeId = null;
            _context.Update(playerToMove);
            _context.Update(playerToReplace);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        [Authorize(Roles = "Treinador")]
        public async Task<IActionResult> AdicionarJogadorLivre(int selecaoId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var treinadorid = await _context.Treinadores.Where(t => t.UserId == userId).Select(t => t.Id).FirstOrDefaultAsync();
            var jogadoresLivres = await _context.Players
                                     .Where(p => p.EquipeId == null && p.TreinadorId == treinadorid && p.FgCandidato == true)
                                     .Where(p => p.SelecaoPlayers.Any(sp => sp.SelecaoId == selecaoId))
                                     .ToListAsync();

            ViewBag.JogadoresLivres = new SelectList(jogadoresLivres, "Id", "Nome");

            var equipes = await _context.Equipes
                .Where(e => e.TreinadorId == treinadorid && e.SelecaoId == selecaoId)
                .ToListAsync();
            ViewBag.Equipes = new SelectList(equipes, "Id", "Nome");

            return View();
        }


        [HttpPost]
        [Authorize(Roles = "Treinador")]
        public async Task<IActionResult> AdicionarJogadorLivre(int jogadorId, int equipeId)
        {
            Console.WriteLine("ID EQUIPE ALVO: " + equipeId);

            var jogador = await _context.Players.FindAsync(jogadorId);

            if (jogador == null)
            {
                return NotFound("Jogador não encontrado.");
            }

            var equipe = await _context.Equipes.FindAsync(equipeId);
            if (equipe == null)
            {
                return NotFound("Equipe não encontrado.");
            }

            jogador.EquipeId = equipeId;

            _context.Update(jogador);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePlayersFromSelecao(int selecaoId, List<int> selectedPlayers)
        {
            var selecao = await _context.Selecoes
                .Include(s => s.SelecaoPlayers)
                .FirstOrDefaultAsync(s => s.Id == selecaoId);

            if (selecao == null)
            {
                TempData["ErrorMessage"] = "Seleção não encontrada.";
                return RedirectToAction("Details", new { id = selecaoId });
            }

            if (selectedPlayers == null || !selectedPlayers.Any())
            {
                TempData["ErrorMessage"] = "Nenhum jogador selecionado para remoção.";
                return RedirectToAction("Details", new { id = selecaoId });
            }

            var selecaoPlayersToRemove = selecao.SelecaoPlayers
                .Where(sp => selectedPlayers.Contains(sp.PlayerId))
                .ToList();

            _context.SelecaoPlayers.RemoveRange(selecaoPlayersToRemove);

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Jogadores removidos com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro ao remover jogadores: {ex.Message}";
            }

            return RedirectToAction("Details", new { id = selecaoId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEquipePartida([FromBody] UpdateEquipePartidaViewModel model)
        {
            try
            {
                if (model == null || model.DraggedEquipeId == null || model.TargetEquipeId == null ||
                    model.DraggedPartidaId == null || model.TargetPartidaId == null)
                {
                    _logger.LogError("Dados inválidos recebidos no UpdateEquipePartida.");
                    return BadRequest("Dados inválidos.");
                }

                // Obter os registros existentes
                var draggedEquipePartida = await _context.EquipePartidas
                    .FirstOrDefaultAsync(ep => ep.EquipeId == model.DraggedEquipeId && ep.PartidaId == model.DraggedPartidaId);

                var targetEquipePartida = await _context.EquipePartidas
                    .FirstOrDefaultAsync(ep => ep.EquipeId == model.TargetEquipeId && ep.PartidaId == model.TargetPartidaId);

                if (draggedEquipePartida == null || targetEquipePartida == null)
                {
                    _logger.LogError("Associações não encontradas no banco de dados.");
                    return NotFound("Uma ou ambas as associações não foram encontradas.");
                }

                // Remover as associações existentes
                _context.EquipePartidas.Remove(draggedEquipePartida);
                _context.EquipePartidas.Remove(targetEquipePartida);
                await _context.SaveChangesAsync();

                // Criar as novas associações trocadas
                var newDraggedEquipePartida = new EquipePartida
                {
                    EquipeId = model.DraggedEquipeId.Value,
                    PartidaId = model.TargetPartidaId.Value
                };

                var newTargetEquipePartida = new EquipePartida
                {
                    EquipeId = model.TargetEquipeId.Value,
                    PartidaId = model.DraggedPartidaId.Value
                };

                _context.EquipePartidas.Add(newDraggedEquipePartida);
                _context.EquipePartidas.Add(newTargetEquipePartida);

                await _context.SaveChangesAsync();

                _logger.LogInformation("Equipes atualizadas com sucesso.");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar as equipes.");
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<IActionResult> InserirPontuacao([FromBody] PontuacaoViewModel model)
        {
            if (model == null || model.PartidaId <= 0 || model.Pontuacoes == null || !model.Pontuacoes.Any())
            {
                return BadRequest("Dados inválidos.");
            }

            var partida = await _context.Partidas
                .Include(p => p.EquipePartidas)
                .FirstOrDefaultAsync(p => p.Id == model.PartidaId);

            if (partida == null)
            {
                return NotFound("Partida não encontrada.");
            }

            foreach (var pontuacao in model.Pontuacoes)
            {
                var equipePartida = partida.EquipePartidas.FirstOrDefault(ep => ep.EquipeId == pontuacao.EquipeId);
                if (equipePartida != null)
                {
                    equipePartida.Pontuacao = pontuacao.Pontuacao;
                }
            }

            await _context.SaveChangesAsync();

            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> ExcluirSelecao(int selecaoId)
        {
            var selecao = await _context.Selecoes
                .Include(s => s.Equipes)
                .Include(s => s.SelecaoPlayers)
                .ThenInclude(sp => sp.Player)
                .FirstOrDefaultAsync(s => s.Id == selecaoId);

            if (selecao == null)
            {
                return NotFound();
            }
            _context.SelecaoPlayers.RemoveRange(selecao.SelecaoPlayers);
            _context.Equipes.RemoveRange(selecao.Equipes);
            _context.Selecoes.RemoveRange(selecao);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
        public async Task<IActionResult> DetailsEquipe(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipe = await _context.Equipes
                                      .Include(g => g.Treinador)
                                      .FirstOrDefaultAsync(m => m.Id == id);

            if (equipe == null)
            {
                return NotFound();
            }

            var players = await _context.Players
                             .Where(p => p.EquipeId == equipe.Id)
                             .ToListAsync();

            ViewBag.Players = players;



            return View(equipe);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipe = await _context.Equipes.FindAsync(id);
            if (equipe == null)
            {
                return NotFound();
            }
            return View(equipe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Status,Tipo")] Equipe equipe)
        {
            if (id != equipe.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Treinador");

            if (ModelState.IsValid)
            {
                try
                {
                    var equipeExistente = await _context.Equipes.FindAsync(id);

                    if (equipeExistente == null)
                    {
                        return NotFound();
                    }

                    equipeExistente.Nome = equipe.Nome;
                    equipeExistente.Status = equipe.Status;
                    equipeExistente.Tipo = equipe.Tipo;

                    _context.Update(equipeExistente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EquipeExists(equipe.Id))
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
            return View(equipe);
        }
        private bool EquipeExists(int id)
        {
            return _context.Equipes.Any(e => e.Id == id);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipe = await _context.Equipes
                .Include(g => g.Treinador).FirstOrDefaultAsync(m => m.Id == id);

            if (equipe == null)
            {
                return NotFound();
            }

            return View(equipe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var equipe = await _context.Equipes.FindAsync(id);
            if (equipe == null)
            {
                return NotFound();
            }

            var players = _context.Players.Where(p => p.EquipeId == id).ToList();
            foreach (var player in players)
            {
                player.EquipeId = null; _context.Update(player);
            }

            _context.Equipes.Remove(equipe);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public class PontuacaoViewModel
        {
            public int PartidaId { get; set; }
            public List<PontuacaoEquipeViewModel> Pontuacoes { get; set; }
        }

        public class PontuacaoEquipeViewModel
        {
            public int EquipeId { get; set; }
            public int Pontuacao { get; set; }
        }

        [HttpGet]
        [Authorize(Roles = "Treinador")]
        public async Task<IActionResult> RemoverJogador()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var treinadorid = await _context.Treinadores.Where(t => t.UserId == userId).Select(t => t.Id).FirstOrDefaultAsync();

            var jogadores = await _context.Players.Include(p => p.Equipe).Where(p => p.TreinadorId == treinadorid && p.EquipeId != null && p.Equipe.Tipo == "Seleção").ToListAsync();
            ViewBag.Jogadores = new SelectList(jogadores, "Nome", "Nome");



            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Treinador")]
        public async Task<IActionResult> RemoverJogador(string playerToRemoveNome)
        {
            var playerToRemove = await _context.Players.FirstOrDefaultAsync(p => p.Nome == playerToRemoveNome);


            if (playerToRemove == null)
            {
                return NotFound("Um ou ambos os jogadores não foram encontrados.");
            }

            playerToRemove.EquipeId = null;

            _context.Update(playerToRemove);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
