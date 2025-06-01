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
using tcc_in305b.Services;

namespace tcc_in305b.Controllers
{
    public class TreinadorController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;
        private readonly ValorantApiService _valorantApiService;
        public TreinadorController(IConfiguration configuration, UserManager<IdentityUser> userManager, ApplicationDbContext context, ILogger<HomeController> logger, ValorantApiService valorantApiService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _userManager = userManager;
            _context = context;
            _logger = logger;
            _valorantApiService = valorantApiService;
        }
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);


            var treinador = await _context.Treinadores
                .Where(t => t.UserId == userId)
                .Include(t => t.ToDoTreinadores)
                .FirstOrDefaultAsync();

            if (treinador == null)
            {
                return RedirectToAction("Error", "Home");
            }

            ViewBag.Players = await _context.Players
                .Where(p => p.TreinadorId == treinador.Id)
                .ToListAsync();


            ViewBag.ToDoList = treinador.ToDoTreinadores
                 .OrderBy(t => t.Feito)
                 .ToList();


            return View();
        }



        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CrudTreinador()
        {
            var treinadores = await _context.Treinadores
                .Include(p => p.User)
                .ToListAsync();

            return View(treinadores);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditTreinador(int id)
        {
            using (IDbConnection db = new SqliteConnection(_connectionString))
            {

                var query = @"SELECT t.*, u.Email 
                      FROM Treinadores t 
                      JOIN AspNetUsers u ON t.UserId = u.Id 
                      WHERE t.Id = @Id";

                var treinador = db.Query<Treinador, IdentityUser, Treinador>(query,
                    (treinador, user) =>
                    {
                        treinador.User = user;
                        return treinador;
                    },
                    new { Id = id }, splitOn: "Email").FirstOrDefault();

                if (treinador == null)
                {
                    return NotFound();
                }

                return View(treinador);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult EditTreinador(Treinador treinador)
        {
            using (IDbConnection db = new SqliteConnection(_connectionString))
            {


                db.Open();


                var currentEmailQuery = "SELECT Email FROM AspNetUsers WHERE Id = @UserId";
                var currentEmail = db.ExecuteScalar<string>(currentEmailQuery, new { UserId = treinador.UserId });

                Console.WriteLine("Email atual no banco de dados: " + currentEmail);
                Console.WriteLine("Email novo enviado: " + treinador.User.Email);

                Console.WriteLine("UserId: " + treinador.UserId);
                Console.WriteLine("Email enviado para a atualização: " + treinador.User.Email);


                var updateUserQuery = "UPDATE AspNetUsers SET Email = @Email, NormalizedEmail = @NormalizedEmail WHERE Id = @UserId";
                var updateTreinadorQuery = "UPDATE Treinadores SET Nome = @Nome WHERE Id = @Id";


                var normalizedEmail = treinador.User.Email.ToUpperInvariant();

                using (var transaction = db.BeginTransaction())
                {
                    try
                    {

                        var userResult = db.Execute(updateUserQuery, new { Email = treinador.User.Email, NormalizedEmail = normalizedEmail, UserId = treinador.UserId }, transaction);

                        Console.WriteLine("Resultado da atualização do usuário (userResult): " + userResult);


                        var treinadorResult = db.Execute(updateTreinadorQuery, new { treinador.Nome, treinador.Id }, transaction);

                        Console.WriteLine("Resultado da atualização do treinador (treinadorResult): " + treinadorResult);

                        transaction.Commit();

                        if (treinadorResult != 0 || userResult != 0)
                        {
                            Console.WriteLine("Redirecionando para CrudTreinador...");
                            return RedirectToAction("CrudTreinador", "Treinador");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Erro ao atualizar os dados");
                            Console.WriteLine("Erro ao atualizar os dados");
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ModelState.AddModelError(string.Empty, "Erro durante a atualização: " + ex.Message);
                        Console.WriteLine("Erro durante a atualização: " + ex.Message);
                    }
                }
            }
            Console.WriteLine("Retornando para a View EditTreinador...");
            return View(treinador);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTreinador(int id)
        {
            using (IDbConnection db = new SqliteConnection(_connectionString))
            {

                var query = @"SELECT t.*, u.Email 
                      FROM Treinadores t 
                      JOIN AspNetUsers u ON t.UserId = u.Id 
                      WHERE t.Id = @Id";

                var treinador = db.Query<Treinador, IdentityUser, Treinador>(query,
                    (treinador, user) =>
                    {
                        treinador.User = user;
                        return treinador;
                    },
                    new { Id = id }, splitOn: "Email").FirstOrDefault();

                if (treinador == null)
                {
                    return NotFound();
                }

                return View(treinador);
            }
        }

        [HttpPost, ActionName("DeleteTreinador")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTreinadorConfirmed(int id)
        {
            using (IDbConnection db = new SqliteConnection(_connectionString))
            {

                db.Open();


                var userIdQuery = "SELECT UserId FROM Treinadores WHERE Id = @Id";
                var userId = db.ExecuteScalar<string>(userIdQuery, new { Id = id });


                var deleteTreinadorQuery = "DELETE FROM Treinadores WHERE Id = @Id";
                var deleteUserQuery = "DELETE FROM AspNetUsers WHERE Id = @UserId";

                using (var transaction = db.BeginTransaction())
                {
                    try
                    {

                        var treinadorResult = db.Execute(deleteTreinadorQuery, new { Id = id }, transaction);


                        var userResult = db.Execute(deleteUserQuery, new { UserId = userId }, transaction);

                        transaction.Commit();

                        if (treinadorResult != 0 && userResult != 0)
                        {
                            return RedirectToAction("CrudTreinador", "Treinador");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Erro ao excluir os dados");
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ModelState.AddModelError(string.Empty, "Erro durante a exclusão: " + ex.Message);
                    }
                }
            }

            return RedirectToAction("CrudTreinador", "Treinador");
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateTreinador()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTreinador(CreateTreinadorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var user = new IdentityUser
            {
                UserName = model.UserName,
                Email = model.Email,
                EmailConfirmed = true
            };



            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {

                var roleResult = await _userManager.AddToRoleAsync(user, "Treinador");

                if (!roleResult.Succeeded)
                {

                    foreach (var error in roleResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }


                var treinador = new Treinador
                {
                    Nome = model.Nome,
                    UserId = user.Id
                };

                _context.Treinadores.Add(treinador);
                await _context.SaveChangesAsync();

                return RedirectToAction("CrudTreinador", "Treinador");
            }
            else
            {

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }



        [HttpGet]
        public JsonResult GetEvents()
        {
            var userid = _userManager.GetUserId(User);

            var treinadorid = _context.Treinadores
                .Where(t => t.UserId == userid)
                .Select(t => t.Id)
                .FirstOrDefault();

            var treinos = _context.Treinos.Where(a => a.TreinadorId == treinadorid).Select(a => new
            {
                id = a.Id,
                title = a.Nome,
                start = a.Data.ToString("yyyy-MM-dd"),
                type = "Treino"
            }).ToList();

            var torneios = _context.Torneios.Where(t => t.TreinadorId == treinadorid).Select(t => new
            {
                id = t.Id,
                title = t.Nome,
                start = t.DataInicio.ToString("yyyy-MM-dd"),
                type = "Torneio"
            }).ToList();

            var selecoes = _context.Selecoes.Where(s => s.TreinadorId == treinadorid).Select(s => new
            {
                id = s.Id,
                title = s.Nome,
                start = s.DataInicio.ToString("yyyy-MM-dd"),
                type = "Seleção"
            }).ToList();
            

            var events = treinos.Concat(selecoes).Concat(torneios).OrderBy(e => e.start).ToList();

            return Json(events);
        }


        [HttpPost]
        public async Task<IActionResult> CreateTodo(int treinadorId, string nome, string descricao)
        {
            var newTodo = new ToDoTreinador
            {
                TreinadorId = treinadorId,
                Nome = nome,
                Descricao = descricao,
                Feito = false,
                DtCriacao = DateTime.Now
            };

            _context.ToDoTreinadores.Add(newTodo);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> ToggleComplete(int id, bool feito)
        {
            var todo = await _context.ToDoTreinadores.FindAsync(id);

            if (todo != null)
            {
                todo.Feito = feito;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            var todo = await _context.ToDoTreinadores.FindAsync(id);

            if (todo != null)
            {
                _context.ToDoTreinadores.Remove(todo);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> GetPuuid(string playerName, string tag)
        {
            var playerInfo = await _valorantApiService.GetPuuid(playerName, tag);

            if (playerInfo != null)
            {
                return View(playerInfo);
            }

            ViewBag.ErrorMessage = "Erro ao buscar informações do jogador.";
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> GetEloAtual(string puuid)
        {
            Console.WriteLine($"Puuid recebido: {puuid}");

            var EloAtual = await _valorantApiService.GetElo(puuid);

            if (EloAtual != null)
            {
                Console.WriteLine($"Elo Atual retornado: {EloAtual.Elo}");
                return Json(new { success = true, elo = EloAtual.Elo });
            }

            Console.WriteLine("Erro: Elo Atual não encontrado para o jogador.");
            return Json(new { success = false, message = "Erro ao buscar informações do jogador." });
        }

        [HttpPost]
        public async Task<IActionResult> GetPlayerStats(string puuid)
        {
            try
            {
                Console.WriteLine($"PUUID recebido para calcular estatísticas: {puuid}");

                var (kills, deaths, assists, hsPercentage, winRate, damage, firstBloods) = await _valorantApiService.GetPlayerStats(puuid);

                if (kills > 0 || deaths > 0 || assists > 0)
                {
                    return Json(new
                    {
                        success = true,
                        kills,
                        deaths,
                        assists,
                        hsPercentage = Math.Round(hsPercentage, 2),
                        winRate = Math.Round(winRate, 2),
                        damage,
                        firstBloods
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Nenhuma partida competitiva encontrada para este jogador."
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar estatísticas: {ex.Message}");
                return Json(new { success = false, message = $"Erro ao buscar estatísticas: {ex.Message}" });
            }
        }

    }
}
