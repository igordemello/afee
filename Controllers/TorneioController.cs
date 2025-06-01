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
using Microsoft.VisualStudio.Web.CodeGeneration.Utils;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Microsoft.AspNetCore.Mvc.Filters;

namespace tcc_in305b.Controllers
{
    public class TorneioController : Controller
    {
        private readonly ILogger<TorneioController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;
        public TorneioController(IConfiguration configuration, UserManager<IdentityUser> userManager, ApplicationDbContext context, ILogger<TorneioController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }



        public async Task<IActionResult> Index()
        {
            var torneios = await _context.Torneios.Include(t => t.EquipeTorneios).ToListAsync();
            return View(torneios);
        }


        public IActionResult Criar()
        {
            var userId = _userManager.GetUserId(User);
            var treinador = _context.Treinadores.FirstOrDefault(t => t.UserId == userId);



            ViewBag.Equipes = new SelectList(_context.Equipes.Where(e => e.SelecaoId == null), "Id", "Nome");

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(Torneio torneio)
        {


            if (ModelState.IsValid)
            {

                var userId = _userManager.GetUserId(User);
                var treinador = _context.Treinadores.FirstOrDefault(t => t.UserId == userId);

                torneio.TreinadorId = treinador.Id;

                _context.Torneios.Add(torneio);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(torneio);
        }


        public async Task<IActionResult> Detalhes(int id)
        {
            var torneio = await _context.Torneios
                .Include(t => t.EquipeTorneios)
                    .ThenInclude(et => et.Equipe)
                .Include(t => t.Chaveamentos)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (torneio == null)
            {
                return NotFound("Torneio não encontrado.");
            }

            var equipes = await _context.Equipes
            .Where(e => e.EquipeTorneios.Any(et => et.TorneioId == id))
            .Distinct()
            .ToListAsync();

            ViewBag.Equipes = equipes;

            var playersEquipe = await _context.Players
                .ToListAsync();

            if (playersEquipe == null || !playersEquipe.Any())
            {
                _logger.LogWarning("Nenhum jogador encontrado");
            }

            ViewBag.PlayersEquipe = playersEquipe;

            return View(torneio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Excluir(int id)
        {
            var torneio = await _context.Torneios
                .Include(t => t.Chaveamentos)
                .ThenInclude(c => c.Partidas)
                .ThenInclude(p => p.EquipePartidas)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (torneio == null)
            {
                return NotFound("Torneio não encontrado.");
            }

            // Remover as entidades relacionadas primeiro
            foreach (var chaveamento in torneio.Chaveamentos)
            {
                foreach (var partida in chaveamento.Partidas)
                {
                    _context.EquipePartidas.RemoveRange(partida.EquipePartidas);
                }
                _context.Partidas.RemoveRange(chaveamento.Partidas);
            }
            _context.Chaveamentos.RemoveRange(torneio.Chaveamentos);

            // Remover o torneio
            _context.Torneios.Remove(torneio);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        //
        // CHAVEAMENTO
        //

        public async Task<IActionResult> Chaveamento(int id)
        {
            var chaveamentos = await _context.Chaveamentos
                .Include(c => c.Partidas)
                .ThenInclude(p => p.EquipePartidas)
                .ThenInclude(ep => ep.Equipe)
                .Where(c => c.TorneioId == id)
                .OrderBy(c => c.Data)
                .ToListAsync();

            if (!chaveamentos.Any())
            {
                return NotFound("Nenhum chaveamento encontrado para este torneio.");
            }

            return View(chaveamentos);
        }

        private string ObterNomeDaRodada(int numeroDeEquipes)
        {
            return numeroDeEquipes switch
            {
                64 => "32 avos de final.",
                32 => "16 avos de final",
                16 => "Oitavos de final",
                8 => "Quartas de final",
                4 => "Semi-final",
                2 => "Final",
                _ => "Rodada"
            };
        }

        public async Task<IActionResult> GerarChaveamento(int id, DateTime dataPrimeiraRodada)
        {
            // Obter o torneio com as equipes inscritas
            var torneio = await _context.Torneios
                .Include(t => t.EquipeTorneios)
                .ThenInclude(et => et.Equipe).ThenInclude(e => e.Players)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (torneio == null)
            {
                return NotFound("Torneio não encontrado.");
            }

            // Verificar se a data da primeira rodada é anterior à data de início do torneio
            if (dataPrimeiraRodada < torneio.DataInicio.Date)
            {
                return BadRequest($"A data da primeira rodada não pode ser anterior à data de início do torneio ({torneio.DataInicio:dd/MM/yyyy}).");
            }

            // Verificar se as equipes são suficientes
            var equipes = torneio.EquipeTorneios.Select(et => et.Equipe).ToList();
            if (equipes.Count == 0)
            {
                return BadRequest("Nenhuma equipe inscrita no torneio.");
            }

            if (!IsPotenciaDeDois(equipes.Count))
            {
                return BadRequest("O número de equipes deve ser uma potência de dois (2, 4, 8, 16, 32, etc.).");
            }

            //foreach (var equipe in equipes)
            //{
            //    var playerCount = equipe.Players.Count; 
            //    if (playerCount != 2)
            //    {
            //        return BadRequest($"A equipe '{equipe.Nome}' não possui exatamente 5 jogadores (possui {playerCount}).");
            //    }
            //}


            // Embaralhar as equipes para o sorteio
            equipes = equipes.OrderBy(e => Guid.NewGuid()).ToList();

            // Criar a primeira rodada
            var chaveamento = new Chaveamento
            {
                TorneioId = id,
                Nome = ObterNomeDaRodada(equipes.Count),
                Data = dataPrimeiraRodada
            };

            _context.Chaveamentos.Add(chaveamento);
            await _context.SaveChangesAsync();

            for (int i = 0; i < equipes.Count; i += 2)
            {
                var partida = new Partida
                {
                    ChaveamentoId = chaveamento.Id,
                    Data = dataPrimeiraRodada,
                    Tipo = "Eliminatória",
                    EquipePartidas = new List<EquipePartida>
            {
                new EquipePartida { EquipeId = equipes[i].Id },
                new EquipePartida { EquipeId = equipes[i + 1].Id }
            }
                };

                _context.Partidas.Add(partida);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Chaveamento", new { id });
        }

        [HttpPost]
        public async Task<IActionResult> InserirHorario([FromBody] HorarioViewModel model)
        {
            if (model == null || model.PartidaId <= 0 || string.IsNullOrEmpty(model.Horario))
            {
                return BadRequest("Dados inválidos.");
            }

            var partida = await _context.Partidas.FindAsync(model.PartidaId);
            if (partida == null)
            {
                return NotFound("Partida não encontrada.");
            }

            // Atualizar o horário da partida
            if (TimeSpan.TryParse(model.Horario, out var horario))
            {
                partida.Data = new DateTime(partida.Data.Year, partida.Data.Month, partida.Data.Day, horario.Hours, horario.Minutes, 0);
                await _context.SaveChangesAsync();
                return Ok();
            }

            return BadRequest("Horário inválido.");
        }

        public class HorarioViewModel
        {
            public int PartidaId { get; set; }
            public string Horario { get; set; }
        }


        // Helper: Valida potência de dois
        private bool IsPotenciaDeDois(int numero)
        {
            return (numero > 0) && ((numero & (numero - 1)) == 0);
        }



        [HttpPost]
        public async Task<IActionResult> AvancarRodada([FromBody] RodadaViewModel model)
        {
            if (model == null || model.ChaveamentoId <= 0 || model.DataProximaRodada == null)
            {
                return BadRequest("Dados inválidos.");
            }

            var chaveamentoAtual = await _context.Chaveamentos
                .Include(c => c.Partidas)
                .ThenInclude(p => p.EquipePartidas)
                .ThenInclude(ep => ep.Equipe)
                .FirstOrDefaultAsync(c => c.Id == model.ChaveamentoId);

            if (chaveamentoAtual == null)
            {
                return NotFound("Chaveamento atual não encontrado.");
            }

            // Garantir que todas as partidas tenham resultados
            if (chaveamentoAtual.Partidas.Any(p => p.EquipePartidas.All(ep => ep.Pontuacao == null)))
            {
                return BadRequest("Nem todas as partidas da rodada atual têm vencedores definidos.");
            }


            // Identificar os vencedores
            var vencedores = chaveamentoAtual.Partidas.Select(p =>
                p.EquipePartidas.OrderByDescending(ep => ep.Pontuacao).First().Equipe).ToList();

            // Verificar se resta apenas um vencedor (torneio finalizado)
            if (vencedores.Count == 1)
            {
                return RedirectToAction("FinalizarTorneio", new { torneioId = chaveamentoAtual.TorneioId });
            }

            // Criar nova rodada (próximo chaveamento)
            var novaRodada = new Chaveamento
            {
                TorneioId = chaveamentoAtual.TorneioId,
                AnteriorId = chaveamentoAtual.Id,
                Nome = ObterNomeDaRodada(vencedores.Count),
                Data = model.DataProximaRodada // Usar a data fornecida pelo usuário
            };

            _context.Chaveamentos.Add(novaRodada);
            await _context.SaveChangesAsync();

            // Criar partidas para a nova rodada
            for (int i = 0; i < vencedores.Count; i += 2)
            {
                var novaPartida = new Partida
                {
                    ChaveamentoId = novaRodada.Id,
                    Data = model.DataProximaRodada, // Data sem horário
                    Tipo = "Eliminatória",
                    EquipePartidas = new List<EquipePartida>
            {
                new EquipePartida { EquipeId = vencedores[i].Id },
                new EquipePartida { EquipeId = vencedores[i + 1].Id }
            }
                };

                _context.Partidas.Add(novaPartida);
            }

            await _context.SaveChangesAsync();

            return Ok();
        }


        public class RodadaViewModel
        {
            public int ChaveamentoId { get; set; }
            public DateTime DataProximaRodada { get; set; }
        }



        public async Task<IActionResult> InserirPontuacao(int partidaId)
        {
            var partida = await _context.Partidas
                .Include(p => p.EquipePartidas)
                .ThenInclude(ep => ep.Equipe)
                .FirstOrDefaultAsync(p => p.Id == partidaId);

            if (partida == null)
            {
                return NotFound("Partida não encontrada.");
            }

            return View(partida);
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



        public async Task<IActionResult> FinalizarTorneio(int torneioId)
        {
            var torneio = await _context.Torneios
                .Include(t => t.Chaveamentos)
                    .ThenInclude(c => c.Partidas)
                    .ThenInclude(p => p.EquipePartidas)
                    .ThenInclude(ep => ep.Equipe)
                .FirstOrDefaultAsync(t => t.Id == torneioId);

            if (torneio == null)
            {
                return NotFound("Torneio não encontrado.");
            }

            var ultimaRodada = torneio.Chaveamentos
                .OrderByDescending(c => c.Data)
                .FirstOrDefault();

            if (ultimaRodada == null || ultimaRodada.Partidas.Count != 1)
            {
                return BadRequest("Torneio ainda não está na última rodada.");
            }

            var partidaFinal = ultimaRodada.Partidas.First();
            var vencedor = partidaFinal.EquipePartidas
                .OrderByDescending(ep => ep.Pontuacao)
                .FirstOrDefault()?.Equipe;

            if (vencedor == null)
            {
                return BadRequest("Não foi possível determinar o vencedor.");
            }

            if (torneio.DataFim == null)
            {
                torneio.DataFim = DateTime.Now;
                await _context.SaveChangesAsync();
            }


            ViewBag.Vencedor = vencedor.Nome;

            //GRÁFICO 1:

            ViewBag.Diferencas = torneio.Chaveamentos
                .SelectMany(c => c.Partidas)
                .SelectMany(p => p.EquipePartidas.Select(ep => new
                {
                    Equipe = ep.Equipe.Nome,
                    Diferenca = (ep.Pontuacao ?? 0) - (p.EquipePartidas.First(e => e.EquipeId != ep.EquipeId).Pontuacao ?? 0)
                    // Pontuação da equipe - Pontuação do adversário
                }))
                .GroupBy(x => x.Equipe)
                .Select(g => new
                {
                    Equipe = g.Key,
                    SomaDiferenca = g.Sum(x => x.Diferenca) // Soma das diferenças com sinal
                })
                .OrderByDescending(x => x.SomaDiferenca)
                .ToList();

            foreach (var chaveamento in torneio.Chaveamentos)
            {
                foreach (var partida in chaveamento.Partidas)
                {
                    Console.WriteLine($"Partida: {partida.Id}");
                    foreach (var equipePartida in partida.EquipePartidas)
                    {
                        Console.WriteLine($"Equipe: {equipePartida.Equipe.Nome}, Pontuação: {equipePartida.Pontuacao}");
                    }
                }
            }

            foreach (var diferenca in ViewBag.Diferencas)
            {
                Console.WriteLine($"Equipe: {diferenca.Equipe}, Soma da Diferença: {diferenca.SomaDiferenca}");
            }

            //ATÉ AQUI


            //GRÁFICO 2:


            var rodadas = torneio.Chaveamentos
                .OrderBy(c => c.Data)
                .Select(c => new
                {
                    Rodada = c.Nome,
                    Dados = c.Partidas.SelectMany(p => p.EquipePartidas
                        .Select(ep => new
                        {
                            Equipe = ep.Equipe.Nome,
                            Rodada = c.Nome,
                            Diferenca = (ep.Pontuacao ?? 0) - (p.EquipePartidas.First(e => e.EquipeId != ep.EquipeId).Pontuacao ?? 0)
                        }))
                });

            ViewBag.Rodadas = rodadas.Select(r => r.Rodada).ToList(); // Ex: ["Quartas de Final", "Semi-Final", "Final"]
            ViewBag.Equipes = rodadas.SelectMany(r => r.Dados.Select(d => d.Equipe)).Distinct().ToList(); // Ex: ["A", "B", ...]
            ViewBag.Valores = rodadas.SelectMany(r => r.Dados).GroupBy(d => new { d.Equipe, d.Rodada })
                .Select(g => new
                {
                    Equipe = g.Key.Equipe,
                    Rodada = g.Key.Rodada,
                    Diferenca = g.Sum(x => x.Diferenca)
                }).ToList(); // Lista de valores



            //ATÉ AQUI



            return View("Finalizado", torneio);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEquipe([Bind("Nome")] Equipe equipe, int torneioId)
        {
            var userId = _userManager.GetUserId(User);
            var torneio = await _context.Torneios
                .FirstOrDefaultAsync(t => t.Id == torneioId);
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Usuário não está logado.";
                return RedirectToAction(nameof(Detalhes), new { id = torneioId });
            }

            var treinador = await _context.Treinadores.FirstOrDefaultAsync(t => t.UserId == userId);

            if (treinador == null)
            {
                TempData["ErrorMessage"] = "Nenhum treinador encontrado para o usuário logado.";
                return RedirectToAction(nameof(Detalhes), new { id = torneioId });
            }

            equipe.TreinadorId = treinador.Id;
            equipe.Tipo = "Torneio";
            equipe.Status = "Ativo";
            equipe.DtCriacao = DateTime.Now;

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                Console.WriteLine("Erros de validação: " + string.Join(", ", errors));
                TempData["ErrorMessage"] = "Erro de validação ao criar equipe: " + string.Join(", ", errors);
                return RedirectToAction(nameof(Detalhes), new { id = torneioId });
            }

            try
            {
                _context.Equipes.Add(equipe);
                await _context.SaveChangesAsync();
                var equipeTorneio = new EquipeTorneio
                {
                    TorneioId = torneioId,
                    EquipeId = equipe.Id,

                };
                _context.EquipeTorneios.Add(equipeTorneio);
                await _context.SaveChangesAsync();


            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao salvar equipe: " + ex.Message);
                TempData["ErrorMessage"] = "Erro ao salvar equipe. Tente novamente.";
            }




            return RedirectToAction(nameof(Detalhes), new { id = torneioId });
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

        [HttpGet]
        [Authorize(Roles = "Treinador")]
        public async Task<IActionResult> RemoverJogador()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var treinadorid = await _context.Treinadores.Where(t => t.UserId == userId).Select(t => t.Id).FirstOrDefaultAsync();

            var jogadores = await _context.Players.Include(p => p.Equipe).Where(p => p.TreinadorId == treinadorid && p.EquipeId != null && p.Equipe.Tipo == "Torneio").ToListAsync();
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


        [HttpGet]
        [Authorize(Roles = "Treinador")]
        public async Task<IActionResult> AdicionarJogadorLivre(int torneioId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var treinadorid = await _context.Treinadores.Where(t => t.UserId == userId).Select(t => t.Id).FirstOrDefaultAsync();
            var jogadoresLivres = await _context.Players
                                     .Where(p => p.EquipeId == null && p.TreinadorId == treinadorid && p.FgCandidato == false)
                                     .ToListAsync();

            ViewBag.JogadoresLivres = new SelectList(jogadoresLivres, "Id", "Nome");

            var equipes = await _context.Equipes
                .Where(e => e.TreinadorId == treinadorid && e.EquipeTorneios.Any(et => et.TorneioId == torneioId))
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

        [HttpGet]
        [Authorize(Roles = "Treinador")]
        public async Task<IActionResult> TrocarJogadores(int torneioId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var treinadorid = await _context.Treinadores.Where(t => t.UserId == userId).Select(t => t.Id).FirstOrDefaultAsync();

            var jogadores = await _context.Players
                .Include(p => p.Equipe).ThenInclude(e => e.EquipeTorneios)
                .Where(p => p.TreinadorId == treinadorid && p.FgCandidato == false && p.Equipe.EquipeTorneios.Any(et => et.TorneioId == torneioId))
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
        public async Task<IActionResult> AlterarJogador(int torneioId)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var treinadorid = await _context.Treinadores.Where(t => t.UserId == userId).Select(t => t.Id).FirstOrDefaultAsync();

            var jogadores = await _context.Players
                .Include(p => p.Equipe).ThenInclude(e => e.EquipeTorneios)
                .Where(p => p.TreinadorId == treinadorid && p.FgCandidato == false && p.Equipe.EquipeTorneios.Any(et => et.TorneioId == torneioId))
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




    }
}
