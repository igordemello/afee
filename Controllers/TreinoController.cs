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
    public class TreinoController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;
        public TreinoController(IConfiguration configuration, UserManager<IdentityUser> userManager, ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }


        [Authorize(Roles = "Treinador")]
        public IActionResult TreinadorTreinos()
        {
            var userId = _userManager.GetUserId(User);
            var treinador = _context.Treinadores.FirstOrDefault(t => t.UserId == userId);
            if (treinador == null)
            {
                return Unauthorized();
            }

            ViewBag.Players = _context.Players.Where(p => p.TreinadorId == treinador.Id).Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Nome
            }).ToList();

            ViewBag.Grupos = _context.Grupos.Where(g => g.TreinadorId == treinador.Id).Select(g => new SelectListItem
            {
                Value = g.Id.ToString(),
                Text = g.Nome
            }).ToList();

            ViewBag.Estrategias = _context.Estrategias.Where(e => e.TreinadorId == treinador.Id).Select(e => new SelectListItem
            {
                Value = e.Id.ToString(),
                Text = e.Nome
            }).ToList();

            ViewBag.TreinoTipos = _context.TreinoTipos.Where(at => at.TreinadorId == treinador.Id).Select(at => new SelectListItem
            {
                Value = at.Id.ToString(),
                Text = at.Nome
            }).ToList();

            ViewBag.TreinoTiposLegenda = _context.TreinoTipos
                .Where(at => at.TreinadorId == treinador.Id)
                .Select(at => new { at.Nome, at.CorHex })
                .ToList();

            return View();
        }


        [Authorize(Roles = "Player")]
        public IActionResult PlayerTreinos()
        {
            var userId = _userManager.GetUserId(User);
            var player = _context.Players
            .Include(p => p.Treinador)
            .FirstOrDefault(p => p.UserId == userId);
            ViewBag.TreinoTiposLegenda = _context.TreinoTipos
                .Where(at => at.TreinadorId == player.TreinadorId)
                .Select(at => new { at.Nome, at.CorHex })
                .ToList();
            return View();
        }


        public async Task<JsonResult> GetEvents()
        {
            var userId = _userManager.GetUserId(User);

            var events = new List<object>();

            if (User.IsInRole("Treinador"))
            {
                var treinador = await _context.Treinadores
                    .FirstOrDefaultAsync(t => t.UserId == userId);

                if (treinador != null)
                {
                    events = _context.Treinos
                        .Where(a => a.TreinadorId == treinador.Id)
                        .Select(a => new
                        {
                            title = a.Nome,
                            start = a.Data.ToString("yyyy-MM-dd") + "T" + a.HorarioInicio.ToString(@"hh\:mm\:ss"),
                            end = a.Data.ToString("yyyy-MM-dd") + "T" + a.HorarioFim.ToString(@"hh\:mm\:ss"),
                            id = a.Id,
                            type = a.TreinoTipo.Nome,
                            typecode = a.TreinoTipo.Codigo,
                            backgroundColorHex = a.TreinoTipo.CorHex,
                            textColor = "#FFFFFF"
                        }).ToList<object>();
                }
            }
            else if (User.IsInRole("Player"))
            {
                var player = await _context.Players
                    .FirstOrDefaultAsync(p => p.UserId == userId);

                if (player != null)
                {
                    events = _context.TreinoPlayers
                        .Where(ap => ap.PlayerId == player.Id)
                        .Select(ap => new
                        {
                            title = ap.Treino.Nome,
                            start = ap.Treino.Data.ToString("yyyy-MM-dd") + "T" + ap.Treino.HorarioInicio.ToString(@"hh\:mm\:ss"),
                            end = ap.Treino.Data.ToString("yyyy-MM-dd") + "T" + ap.Treino.HorarioFim.ToString(@"hh\:mm\:ss"),
                            id = ap.TreinoId,
                            type = ap.Treino.TreinoTipo.Nome,
                            typecode = ap.Treino.TreinoTipo.Codigo,
                            backgroundColorHex = ap.Treino.TreinoTipo.CorHex,
                            textColor = "#FFFFFF"
                        }).ToList<object>();
                }
            }

            return new JsonResult(events);
        }




        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var treino = await _context.Treinos
                .Include(a => a.Estrategia)
                .Include(a => a.TreinoTipo)
                .Include(a => a.TreinoPlayers)
                    .ThenInclude(ap => ap.Player)
                .Include(a => a.TreinoGrupos)
                    .ThenInclude(ag => ag.Grupo)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (treino == null)
            {
                return NotFound();
            }


            var grupoPlayers = new Dictionary<int, List<Player>>();


            foreach (var treinoGrupo in treino.TreinoGrupos)
            {
                var players = await _context.Players
                    .Where(p => p.GrupoId == treinoGrupo.GrupoId)
                    .ToListAsync();
                grupoPlayers.Add(treinoGrupo.GrupoId, players);
            }


            ViewData["GrupoPlayers"] = grupoPlayers;

            return PartialView("_DetailsModal", treino);
        }


        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Treino treino, List<int> Jogadores, List<int> Grupos, int? EstrategiaId, int? TreinoTipoId)
        {
            if (treino.HorarioInicio >= treino.HorarioFim)
            {
                return Json(new { success = false, message = "O horário final deve ser posterior ao horário inicial." });
            }

            if (treino.Data < DateTime.Now)
            {
                return Json(new { success = false, message = "Não se pode fazer um treinomento em uma data que já passou." });
            }

            var userId = _userManager.GetUserId(User);
            var treinador = await _context.Treinadores.FirstOrDefaultAsync(t => t.UserId == userId);
            if (treinador == null)
            {
                return Json(new { success = false, message = "Treinador não encontrado." });
            }

            var treinoTipo = await _context.TreinoTipos.FindAsync(TreinoTipoId);
            if (treinoTipo == null)
            {
                return Json(new { success = false, message = "Tipo de treinomento inválido." });
            }
            int codigoTipo = treinoTipo.Codigo;

            using (IDbConnection db = new SqliteConnection(_connectionString))
            {
                try
                {
                    db.Open();

                    var checkConflictQuery = @"SELECT COUNT(*) FROM Treinos 
                        WHERE TreinadorId = @TreinadorId 
                        AND Data = @Data
                        AND ((@HorarioInicio BETWEEN HorarioInicio AND HorarioFim) 
                        OR (@HorarioFim BETWEEN HorarioInicio AND HorarioFim) 
                        OR (HorarioInicio BETWEEN @HorarioInicio AND @HorarioFim)
                        OR (HorarioFim BETWEEN @HorarioInicio AND @HorarioFim))";

                    var conflictCount = await db.ExecuteScalarAsync<int>(checkConflictQuery, new
                    {
                        treino.Data,
                        treino.HorarioInicio,
                        HorarioFim = treino.HorarioFim,
                        TreinadorId = treinador.Id
                    });

                    if (conflictCount > 0)
                    {
                        return Json(new { success = false, message = "Já existe um treinomento neste horário." });
                    }

                    treino.EstrategiaId = EstrategiaId.HasValue ? EstrategiaId.Value : (int?)null;

                    if (codigoTipo == 0 && EstrategiaId.HasValue)
                    {
                        foreach (var playerId in Jogadores)
                        {
                            int? grupoIdNullable = _context.Players
                                .Where(p => p.Id == playerId)
                                .Select(p => p.GrupoId)
                                .FirstOrDefault();

                            if (grupoIdNullable.HasValue)
                            {
                                int grupoId = grupoIdNullable.Value;

                                bool estrategiaAssociada = _context.EstrategiaGrupos
                                    .Any(eg => eg.GrupoId == grupoId && eg.EstrategiaId == EstrategiaId);

                                if (!estrategiaAssociada)
                                {
                                    _context.EstrategiaGrupos.Add(new EstrategiaGrupo
                                    {
                                        GrupoId = grupoId,
                                        EstrategiaId = EstrategiaId.Value
                                    });
                                }
                            }
                        }
                    }
                    else if (codigoTipo == 1 && EstrategiaId.HasValue)
                    {
                        foreach (var grupoId in Grupos)
                        {
                            bool estrategiaAssociada = _context.EstrategiaGrupos
                                .Any(eg => eg.GrupoId == grupoId && eg.EstrategiaId == EstrategiaId);

                            if (!estrategiaAssociada)
                            {
                                _context.EstrategiaGrupos.Add(new EstrategiaGrupo
                                {
                                    GrupoId = grupoId,
                                    EstrategiaId = EstrategiaId.Value
                                });
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                    var insertTreinoQuery = @"INSERT INTO Treinos (TreinadorId, TreinoTipoId, Data, HorarioInicio, HorarioFim, Nome, EstrategiaId) 
                        VALUES (@TreinadorId, @TreinoTipoId, @Data, @HorarioInicio, @HorarioFim, @Nome, @EstrategiaId);
                        SELECT last_insert_rowid();";
                    var treinoId = await db.ExecuteScalarAsync<int>(insertTreinoQuery, new
                    {
                        TreinadorId = treinador.Id,
                        TreinoTipoId = treinoTipo.Id,
                        treino.Data,
                        treino.HorarioInicio,
                        treino.HorarioFim,
                        treino.Nome,
                        EstrategiaId = treino.EstrategiaId
                    });

                    if (codigoTipo == 0)
                    {
                        foreach (var playerId in Jogadores)
                        {
                            var insertTreinoPlayerQuery = @"INSERT INTO TreinoPlayers (TreinoId, PlayerId) 
                    VALUES (@TreinoId, @PlayerId);";
                            await db.ExecuteAsync(insertTreinoPlayerQuery, new { TreinoId = treinoId, PlayerId = playerId });
                        }
                    }
                    else if (codigoTipo == 1)
                    {
                        foreach (var grupoId in Grupos)
                        {
                            var insertTreinoGrupoQuery = @"INSERT INTO TreinoGrupos (TreinoId, GrupoId) 
                    VALUES (@TreinoId, @GrupoId);";
                            await db.ExecuteAsync(insertTreinoGrupoQuery, new { TreinoId = treinoId, GrupoId = grupoId });

                            var playerIds = _context.Players.Where(p => p.GrupoId == grupoId).Select(p => p.Id).ToList();
                            foreach (var playerId in playerIds)
                            {
                                var insertTreinoPlayerQuery = @"INSERT INTO TreinoPlayers (TreinoId, PlayerId) 
                        VALUES (@TreinoId, @PlayerId);";
                                await db.ExecuteAsync(insertTreinoPlayerQuery, new { TreinoId = treinoId, PlayerId = playerId });
                            }
                        }
                    }

                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro ao salvar o treinomento: " + ex.Message);
                    return Json(new { success = false, message = "Erro durante o processo de criação: " + ex.Message });
                }
            }
        }


        [HttpPost]
        [Authorize(Roles = "Treinador")]
        public async Task<JsonResult> DeleteTreino(int id)
        {
            using (IDbConnection db = new SqliteConnection(_connectionString))
            {
                try
                {
                    db.Open();


                    using (var transaction = db.BeginTransaction())
                    {

                        var deleteTreinoPlayersQuery = @"DELETE FROM TreinoPlayers WHERE TreinoId = @TreinoId";
                        await db.ExecuteAsync(deleteTreinoPlayersQuery, new { TreinoId = id }, transaction);


                        var deleteTreinoQuery = @"DELETE FROM Treinos WHERE Id = @Id";
                        await db.ExecuteAsync(deleteTreinoQuery, new { Id = id }, transaction);


                        transaction.Commit();
                    }

                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro ao excluir o treinomento: " + ex.Message);
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }
        [HttpPost]
        [Authorize(Roles = "Treinador")]
        public async Task<IActionResult> ReplicateEvent(int eventId, DateTime originalDate, DateTime newDate)
        {
            try
            {
                var originalTreino = await _context.Treinos
                    .Include(a => a.TreinoTipo).Include(a => a.TreinoPlayers).Include(a => a.TreinoGrupos).FirstOrDefaultAsync(a => a.Id == eventId);

                if (originalTreino == null)
                {
                    return Json(new { success = false, message = "Evento original não encontrado." });
                }

                var userId = _userManager.GetUserId(User);
                var treinador = await _context.Treinadores.FirstOrDefaultAsync(t => t.UserId == userId);
                if (treinador == null)
                {
                    return Unauthorized();
                }

                var existingEvents = _context.Treinos
                    .Where(a => a.TreinadorId == treinador.Id && a.Data == newDate)
                    .AsEnumerable().Where(a => a.HorarioInicio <= originalTreino.HorarioFim && a.HorarioFim >= originalTreino.HorarioInicio)
                    .Count();

                if (existingEvents > 0)
                {
                    return Json(new { success = false, message = "Já existe um evento nesta data e horário." });
                }

                var newTreino = new Treino
                {
                    Nome = originalTreino.Nome,
                    TreinoTipoId = originalTreino.TreinoTipoId,
                    Data = newDate,
                    HorarioInicio = originalTreino.HorarioInicio,
                    HorarioFim = originalTreino.HorarioFim,
                    TreinadorId = originalTreino.TreinadorId,
                    EstrategiaId = originalTreino.EstrategiaId
                };

                _context.Treinos.Add(newTreino);
                await _context.SaveChangesAsync();

                if (originalTreino.TreinoTipo?.Codigo == 0)
                {
                    var jogadores = _context.TreinoPlayers.Where(ap => ap.TreinoId == originalTreino.Id);
                    foreach (var jogador in jogadores)
                    {
                        _context.TreinoPlayers.Add(new TreinoPlayer
                        {
                            TreinoId = newTreino.Id,
                            PlayerId = jogador.PlayerId
                        });
                    }
                }
                else if (originalTreino.TreinoTipo?.Codigo == 1)
                {
                    var grupos = _context.TreinoGrupos.Where(ag => ag.TreinoId == originalTreino.Id);
                    foreach (var grupo in grupos)
                    {
                        _context.TreinoGrupos.Add(new TreinoGrupo
                        {
                            TreinoId = newTreino.Id,
                            GrupoId = grupo.GrupoId
                        });
                    }
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao replicar o evento: " + ex.Message });
            }
        }





        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var treino = await _context.Treinos
                .Include(a => a.TreinoTipo)
                .Include(a => a.TreinoPlayers)
                .Include(a => a.TreinoGrupos)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (treino == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var treinador = await _context.Treinadores.FirstOrDefaultAsync(t => t.UserId == userId);
            if (treinador == null)
            {
                return Unauthorized();
            }

            ViewBag.Players = _context.Players
                .Where(p => p.TreinadorId == treinador.Id)
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Nome })
                .ToList();

            ViewBag.Grupos = _context.Grupos
                .Where(g => g.TreinadorId == treinador.Id)
                .Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Nome })
                .ToList();

            ViewBag.Estrategias = _context.Estrategias
                .Where(e => e.TreinadorId == treinador.Id)
                .Select(e => new SelectListItem { Value = e.Id.ToString(), Text = e.Nome })
                .ToList();
            ViewBag.TreinoTipos = _context.TreinoTipos
                 .Select(at => new SelectListItem
                 {
                     Value = at.Id.ToString(),
                     Text = at.Nome
                 })
                 .ToList();

            return PartialView("_EditModal", treino);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Treino treino, List<int> Jogadores, List<int> Grupos, int? EstrategiaId, int TreinoTipoId)
        {
            var userId = _userManager.GetUserId(User);
            var treinador = await _context.Treinadores.FirstOrDefaultAsync(t => t.UserId == userId);

            if (treinador == null)
            {
                _logger.LogError("Treinador not found for user Id: {UserId}", userId);
                return Unauthorized();
            }

            var treinoToUpdate = await _context.Treinos
                .Include(a => a.TreinoTipo)
                .Include(a => a.TreinoPlayers)
                .Include(a => a.TreinoGrupos)
                .FirstOrDefaultAsync(a => a.Id == treino.Id);

            if (treinoToUpdate == null)
            {
                _logger.LogError("Treino with Id {Id} not found.", treino.Id);
                return NotFound();
            }

            treinoToUpdate.Nome = treino.Nome;
            treinoToUpdate.Data = treino.Data;
            treinoToUpdate.HorarioInicio = treino.HorarioInicio;
            treinoToUpdate.HorarioFim = treino.HorarioFim;
            treinoToUpdate.TreinoTipoId = treino.TreinoTipoId;

            treinoToUpdate.EstrategiaId = EstrategiaId.HasValue ? EstrategiaId.Value : (int?)null;

            _context.TreinoPlayers.RemoveRange(treinoToUpdate.TreinoPlayers);
            _context.TreinoGrupos.RemoveRange(treinoToUpdate.TreinoGrupos);

            if (treinoToUpdate.TreinoTipo == null)
            {
                _logger.LogError("TreinoTipo is null for Treino with Id {Id}.", treino.Id);
                return BadRequest("Tipo de treinomento inválido.");
            }

            if (treinoToUpdate.TreinoTipo.Codigo == 0)
            {
                _logger.LogInformation("Updating players for individual event.");
                foreach (var playerId in Jogadores)
                {
                    _context.TreinoPlayers.Add(new TreinoPlayer { TreinoId = treino.Id, PlayerId = playerId });
                }
            }
            else if (treinoToUpdate.TreinoTipo.Codigo == 1)
            {
                _logger.LogInformation("Updating groups and players for collective event.");
                foreach (var grupoId in Grupos)
                {
                    _context.TreinoGrupos.Add(new TreinoGrupo { TreinoId = treino.Id, GrupoId = grupoId });

                    var playerIds = _context.Players.Where(p => p.GrupoId == grupoId).Select(p => p.Id).ToList();
                    foreach (var playerId in playerIds)
                    {
                        _context.TreinoPlayers.Add(new TreinoPlayer { TreinoId = treino.Id, PlayerId = playerId });
                    }
                }
            }

            try
            {
                _logger.LogInformation("Saving changes to the database.");
                await _context.SaveChangesAsync();
                _logger.LogInformation("Changes saved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while saving changes.");
                ModelState.AddModelError(string.Empty, "Ocorreu um erro ao salvar as alterações.");
                return View(treino);
            }

            return RedirectToAction(nameof(TreinadorTreinos));
        }

        [HttpGet]
        public async Task<JsonResult> GetTreinoTipoCodigo(int treinoTipoId)
        {
            var treinoTipo = await _context.TreinoTipos.FindAsync(treinoTipoId);
            if (treinoTipo != null)
            {
                return Json(treinoTipo.Codigo);
            }
            return Json(null);
        }



        [HttpGet]
        public IActionResult ListTreinoTipos()
        {
            var userId = _userManager.GetUserId(User);
            var treinador = _context.Treinadores.FirstOrDefault(t => t.UserId == userId);
            if (treinador == null)
            {
                return Unauthorized();
            }

            var treinoTipos = _context.TreinoTipos
                .Where(at => at.TreinadorId == treinador.Id)
                .ToList();

            return PartialView("_TiposModal", treinoTipos);
        }


        [HttpGet]
        public IActionResult CreateTreinoTipo()
        {
            var userId = _userManager.GetUserId(User);
            var treinador = _context.Treinadores.FirstOrDefault(t => t.UserId == userId);

            if (treinador == null)
            {
                return Json(new { success = false, message = "Treinador não encontrado." });
            }

            var treinoTipo = new TreinoTipo
            {
                TreinadorId = treinador.Id
            };

            return PartialView("_TreinoTipoForm", treinoTipo);
        }


        [HttpPost]
        public async Task<IActionResult> CreateTreinoTipo(TreinoTipo treinoTipo)
        {
            ModelState.Remove("Treinador");

            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                Console.WriteLine("Erros de validação no ModelState:");
                foreach (var error in errorMessages)
                {
                    Console.WriteLine(error);
                }

                return Json(new { success = false, message = "Invalid data provided.", errors = errorMessages });
            }

            var userId = _userManager.GetUserId(User);
            var treinador = await _context.Treinadores.FirstOrDefaultAsync(t => t.UserId == userId);

            if (treinador == null)
            {
                return Json(new { success = false, message = "Treinador not found." });
            }

            if (treinoTipo.TreinadorId == 0)
            {
                treinoTipo.TreinadorId = treinador.Id;
            }

            _context.TreinoTipos.Add(treinoTipo);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }









        [HttpGet]
        public async Task<IActionResult> EditTreinoTipo(int id)
        {
            var treinoTipo = await _context.TreinoTipos.FindAsync(id);
            if (treinoTipo == null)
            {
                return NotFound();
            }
            return PartialView("_TreinoTipoForm", treinoTipo);
        }

        [HttpPost]
        public async Task<IActionResult> EditTreinoTipo(TreinoTipo treinoTipo)
        {
            ModelState.Remove("Treinador");

            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                Console.WriteLine("Erros de validação no ModelState:");
                foreach (var error in errorMessages)
                {
                    Console.WriteLine(error);
                }

                return Json(new { success = false, message = "Invalid data provided.", errors = errorMessages });
            }

            var treinoTipoExistente = await _context.TreinoTipos.FindAsync(treinoTipo.Id);
            if (treinoTipoExistente == null)
            {
                Console.WriteLine("Tipo de Treinomento não encontrado.");
                return Json(new { success = false, message = "Tipo de Treinomento não encontrado." });
            }

            treinoTipoExistente.Nome = treinoTipo.Nome;
            treinoTipoExistente.CorHex = treinoTipo.CorHex;
            treinoTipoExistente.Codigo = treinoTipo.Codigo;
            treinoTipoExistente.TreinadorId = treinoTipo.TreinadorId;

            try
            {
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao salvar o tipo de treinomento: " + ex.Message);
                Console.WriteLine("Stack Trace: " + ex.StackTrace); return Json(new { success = false, message = "Erro ao salvar o tipo de treinomento.", exception = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTreinoTipo(int id)
        {
            var treinoTipo = await _context.TreinoTipos.FindAsync(id);
            if (treinoTipo == null)
            {
                return Json(new { success = false, message = "Tipo de Treinomento não encontrado." });
            }

            _context.TreinoTipos.Remove(treinoTipo);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }



    }
}