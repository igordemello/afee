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
    public class AnaliseController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;
        public AnaliseController(IConfiguration configuration, UserManager<IdentityUser> userManager, ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }
        public IActionResult Index(FiltroAnaliseViewModel filtro)
        {
            var userId = _userManager.GetUserId(User);
            var treinador = _context.Treinadores.FirstOrDefault(t => t.UserId == userId);

            if (filtro.DataInicio.HasValue && filtro.DataFim.HasValue && filtro.DataInicio > filtro.DataFim)
            {
                ModelState.AddModelError("DataInvalida", "A data de início não pode ser maior que a data de fim.");
            }

            

            // Base de dados para cada tipo de análise
            var analisesGrupo = _context.AnaliseGrupos
                .Include(ag => ag.Grupo)
                .Include(ag => ag.Analise)
                .Include(ag => ag.Analise.AnaliseNotas)
                    .ThenInclude(an => an.Nota)
                .Where(ag => ag.Analise.TreinadorId == treinador.Id);

            var analisesPlayer = _context.AnalisePlayers
                .Include(ap => ap.Player)
                .Include(ap => ap.Analise)
                .Include(ap => ap.Analise.AnaliseNotas)
                    .ThenInclude(an => an.Nota)
                .Where(ap => ap.Analise.TreinadorId == treinador.Id);

            var analisesPlayerGrupo = _context.AnalisePlayerGrupos
                .Include(apg => apg.Player)
                .Include(apg => apg.Grupo)
                .Include(apg => apg.Analise)
                .Include(apg => apg.Analise.AnaliseNotas)
                    .ThenInclude(an => an.Nota)
                .Where(apg => apg.Analise.TreinadorId == treinador.Id);

            if (!ModelState.IsValid)
            {
                ViewBag.AnalisesGrupo = analisesGrupo.OrderByDescending(ag => ag.Analise.Data).ToList();
                ViewBag.AnalisesPlayer = analisesPlayer.OrderByDescending(ap => ap.Analise.Data).ToList();
                ViewBag.AnalisesPlayerGrupo = analisesPlayerGrupo.OrderByDescending(apg => apg.Analise.Data).ToList();

                
                ViewBag.Players = _context.Players.Where(p => p.TreinadorId == treinador.Id).ToList();
                ViewBag.Grupos = _context.Grupos.Where(g => g.TreinadorId == treinador.Id).ToList();
                return View(filtro);
            }

            // Aplicar filtros
            if (filtro.PlayerId.HasValue)
            {
                analisesPlayer = analisesPlayer.Where(ap => ap.PlayerId == filtro.PlayerId.Value);
                analisesGrupo = analisesGrupo.Where(ag => ag.Grupo.Players.Any(p => p.Id == filtro.PlayerId.Value));
                analisesPlayerGrupo = analisesPlayerGrupo.Where(apg => apg.PlayerId == filtro.PlayerId.Value);
            }

            if (filtro.GrupoId.HasValue)
            {
                analisesPlayer = analisesPlayer.Where(ap => ap.Player.GrupoId == filtro.GrupoId.Value);
                analisesGrupo = analisesGrupo.Where(ag => ag.GrupoId == filtro.GrupoId.Value);
                analisesPlayerGrupo = analisesPlayerGrupo.Where(apg => apg.GrupoId == filtro.GrupoId.Value);
            }

            if (filtro.DataInicio.HasValue && filtro.DataFim.HasValue)
            {
                // Expandir DataFim para incluir todo o último dia
                var dataFim = filtro.DataFim.Value.Date.AddDays(1).AddTicks(-1);

                analisesPlayer = analisesPlayer.Where(ap => ap.Analise.Data >= filtro.DataInicio.Value.Date && ap.Analise.Data <= dataFim);
                analisesGrupo = analisesGrupo.Where(ag => ag.Analise.Data >= filtro.DataInicio.Value.Date && ag.Analise.Data <= dataFim);
                analisesPlayerGrupo = analisesPlayerGrupo.Where(apg => apg.Analise.Data >= filtro.DataInicio.Value.Date && apg.Analise.Data <= dataFim);
            }

            // Ordenar e retornar as listas filtradas
            ViewBag.AnalisesGrupo = analisesGrupo.OrderByDescending(ag => ag.Analise.Data).ToList();
            ViewBag.AnalisesPlayer = analisesPlayer.OrderByDescending(ap => ap.Analise.Data).ToList();
            ViewBag.AnalisesPlayerGrupo = analisesPlayerGrupo.OrderByDescending(apg => apg.Analise.Data).ToList();

            // Popular os dropdowns
            ViewBag.Players = _context.Players.Where(p => p.TreinadorId == treinador.Id).ToList();
            ViewBag.Grupos = _context.Grupos.Where(g => g.TreinadorId == treinador.Id).ToList();

            return View(filtro); // Retornar o ViewModel para repopular o formulário na View
        }

        [HttpGet]
        public IActionResult Criar()
        {
            var userId = _userManager.GetUserId(User);
            var treinador = _context.Treinadores.FirstOrDefault(t => t.UserId == userId);

            if (treinador == null)
            {
                return NotFound("Treinador não encontrado.");
            }

            // Seleciona todos os grupos e jogadores associados
            ViewBag.Grupos = _context.Grupos
                .Where(g => g.TreinadorId == treinador.Id)
                .Select(g => new { g.Id, g.Nome })
                .ToList();

            ViewBag.Players = _context.Players
                .Where(p => p.TreinadorId == treinador.Id)
                .Select(p => new { p.Id, p.Nome, p.GrupoId }) // Incluímos o GrupoId
                .ToList();

            ViewBag.Notas = _context.Notas
                .Where(n => n.TreinadorId == treinador.Id)
                .Select(n => new { n.Id, n.Nome })
                .ToList();

            return View();
        }


        [HttpPost]
        public IActionResult Criar(CriarAnaliseViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            var treinador = _context.Treinadores.FirstOrDefault(t => t.UserId == userId);
            if (!ModelState.IsValid)
            {
                

                ViewBag.Grupos = _context.Grupos
                    .Where(g => g.TreinadorId == treinador.Id)
                    .Select(g => new { g.Id, g.Nome })
                    .ToList();

                ViewBag.Players = _context.Players
                    .Where(p => p.TreinadorId == treinador.Id)
                    .Select(p => new { p.Id, p.Nome, p.GrupoId }) 
                    .ToList();

                ViewBag.Notas = _context.Notas
                    .Where(n => n.TreinadorId == treinador.Id)
                    .Select(n => new { n.Id, n.Nome })
                    .ToList();

                return View(model);
            }

            

            if (treinador == null)
            {
                return NotFound("Treinador não encontrado.");
            }

            // Criar a análise
            var analise = new Analise
            {
                Descricao = model.Descricao,
                TreinadorId = treinador.Id,
                Data = DateTime.Now
            };

            _context.Analises.Add(analise);
            _context.SaveChanges();

            // Salvar as notas
            if (model.Notas != null)
            {
                Console.WriteLine("Nota é diferente de null");
                foreach (var nota in model.Notas)
                {
                    if (nota.Valor.HasValue)
                    {
                        Console.WriteLine("Nota HAS VALUE");
                        var analiseNota = new AnaliseNota
                        {
                            AnaliseId = analise.Id,
                            NotaId = nota.NotaId,
                            Valor = nota.Valor.Value
                        };

                        Console.WriteLine("AnaliseId: " + analise.Id + "\nNotaId: " + nota.NotaId + "\nValor: " + nota.Valor.Value);
                        _context.AnaliseNotas.Add(analiseNota);
                    }
                }
            }

            // Processar o tipo da análise
            if (model.Tipo == "Player")
            {
                _context.AnalisePlayers.Add(new AnalisePlayer
                {
                    AnaliseId = analise.Id,
                    PlayerId = model.AlvoId
                });
            }
            else if (model.Tipo == "Grupo")
            {
                var grupo = _context.Grupos.Include(g => g.Players).FirstOrDefault(g => g.Id == model.AlvoId);

                if (grupo != null)
                {
                    var formacao = string.Join(", ", grupo.Players.Select(p => p.Nome));

                    _context.AnaliseGrupos.Add(new AnaliseGrupo
                    {
                        AnaliseId = analise.Id,
                        GrupoId = grupo.Id,
                        Formacao = formacao
                    });
                }
            }
            else if (model.Tipo == "PlayerEmGrupo")
            {
                // Obter o grupo associado ao jogador
                var grupoDoPlayer = _context.Grupos
                    .Include(g => g.Players)
                    .FirstOrDefault(g => g.Players.Any(p => p.Id == model.AlvoId));

                if (grupoDoPlayer != null)
                {
                    var formacao = string.Join(", ", grupoDoPlayer.Players.Select(p => p.Nome));

                    _context.AnalisePlayerGrupos.Add(new AnalisePlayerGrupo
                    {
                        AnaliseId = analise.Id,
                        PlayerId = model.AlvoId,
                        GrupoId = grupoDoPlayer.Id,
                        Formacao = formacao
                    });
                }
                else
                {
                    // Caso não seja encontrado um grupo para o player
                    ModelState.AddModelError("", "Nenhum grupo associado encontrado para o jogador selecionado.");
                    return View(model);
                }
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var analise = _context.Analises.FirstOrDefault(a => a.Id == id);
            if (analise == null)
            {
                return NotFound("Análise não encontrada.");
            }

            _context.Analises.Remove(analise);
            _context.SaveChanges();

            return Ok(new { message = "Análise excluída com sucesso!" });
        }


        [HttpPost]
        public IActionResult Edit(int AnaliseId, string Descricao, List<AnaliseNota> Notas)
        {
            // Buscar a análise no banco de dados
            var analise = _context.Analises.FirstOrDefault(a => a.Id == AnaliseId);
            if (analise == null)
            {
                return NotFound("Análise não encontrada.");
            }

            // Atualizar a descrição da análise
            analise.Descricao = Descricao;

            // Carregar as notas existentes da análise no banco de dados
            var notasExistentes = _context.AnaliseNotas
                .Where(an => an.AnaliseId == AnaliseId)
                .ToList();

            // Processar as notas enviadas no formulário
            if (Notas != null && Notas.Any())
            {
                foreach (var nota in Notas)
                {
                    if (nota.Valor != null) // Verifica se o valor existe
                    {
                        // Tentar encontrar a nota existente
                        var notaExistente = notasExistentes.FirstOrDefault(n => n.NotaId == nota.NotaId);

                        if (notaExistente != null)
                        {
                            // Atualizar o valor da nota existente
                            notaExistente.Valor = nota.Valor.Value; // Use .Value porque sabemos que não é nulo
                        }
                        else
                        {
                            // Adicionar nova nota, se não existir
                            _context.AnaliseNotas.Add(new AnaliseNota
                            {
                                AnaliseId = AnaliseId,
                                NotaId = nota.NotaId,
                                Valor = nota.Valor.Value // Use .Value porque sabemos que não é nulo
                            });
                        }
                    }
                }
            }

            // Salvar alterações no banco de dados
            _context.SaveChanges();

            return Ok(new { message = "Análise e notas atualizadas com sucesso!" });
        }


        [HttpGet]
        public IActionResult VerificarAnalise(int selecaoId)
        {
            var existeAnalise = _context.AnaliseSelecoes.Any(a => a.SelecaoId == selecaoId);
            return Json(new { temAnalise = existeAnalise });
        }

        [HttpPost]
        public IActionResult CriarAnaliseSelecao([FromBody] CriarAnaliseSelecaoViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            var treinador = _context.Treinadores.FirstOrDefault(t => t.UserId == userId);

            if (model == null || string.IsNullOrWhiteSpace(model.Descricao))
            {
                return BadRequest("Dados inválidos.");
            }

            var analise = new Analise
            {
                Descricao = model.Descricao,
                TreinadorId = treinador.Id,
                Data = DateTime.Now
            };

            _context.Analises.Add(analise);
            _context.SaveChanges();

            var analiseSelecao = new AnaliseSelecao
            {
                AnaliseId = analise.Id,
                SelecaoId = model.SelecaoId
            };

            _context.AnaliseSelecoes.Add(analiseSelecao);
            _context.SaveChanges();

            return Ok(new { message = "Análise criada com sucesso!" });
        }

        public class CriarAnaliseSelecaoViewModel
        {
            public int SelecaoId { get; set; }
            public string Descricao { get; set; }
        }

        public IActionResult Selecao(int selecaoid)
        {
            var analiseSelecao = _context.AnaliseSelecoes
                .Include(a => a.Selecao)
                .Include(a => a.Analise)
                .FirstOrDefault(a => a.SelecaoId == selecaoid);

            if (analiseSelecao == null)
            {
                return NotFound("Análise não encontrada para esta seleção.");
            }

            ViewBag.Data = analiseSelecao.Analise.Data;

            ViewBag.NomeSelecao = analiseSelecao.Selecao.Nome;

            ViewBag.Descricao = analiseSelecao.Analise.Descricao;

            return View(analiseSelecao);

            

        }

        [HttpPost]
        public IActionResult EditarAnaliseSelecao([FromBody] EditarAnaliseSelecaoVM model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.NovaDescricao))
            {
                return BadRequest("Dados inválidos.");
            }

            var analise = _context.Analises.FirstOrDefault(a => a.Id == model.AnaliseId);
            if (analise == null)
            {
                return NotFound("Análise não encontrada.");
            }

            analise.Descricao = model.NovaDescricao;
            analise.Data = DateTime.Now;
            _context.SaveChanges();

            return Ok(new { message = "Descrição atualizada com sucesso!" });
        }


        public class EditarAnaliseSelecaoVM
        {
            public int AnaliseId { get; set; }
            public string NovaDescricao { get; set; }
        }



    }
}
