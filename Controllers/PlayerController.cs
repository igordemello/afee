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
using System.Numerics;

namespace tcc_in305b.Controllers
{
    public class PlayerController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;
        private readonly ValorantApiService _valorantApiService;
        public PlayerController(IConfiguration configuration, UserManager<IdentityUser> userManager, ApplicationDbContext context, ILogger<HomeController> logger, ValorantApiService valorantApiService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _userManager = userManager;
            _context = context;
            _logger = logger;
            _valorantApiService = valorantApiService;
        }

        [Authorize(Roles = "Player")]
        public async Task<IActionResult> PlayerIndex()
        {
            var userId = _userManager.GetUserId(User);

            var player = await _context.Players
                .Include(p => p.Grupo)
                .FirstOrDefaultAsync(p => p.UserId == userId);
            var playerequipe = await _context.Players
                .Include(p => p.Equipe)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (player != null)
            {
                ViewBag.PlayerName = player.Nickname + "#" + player.Tag;
                ViewBag.GrupoNome = player.Grupo?.Nome ?? "Sem Grupo";
                ViewBag.EquipeNome = player.Equipe?.Nome ?? "Sem Equipe";
            }
            else
            {
                ViewBag.PlayerName = "Player não encontrado";
            }

            var playersInSameGroup = await _context.Players
                .Include(p => p.Grupo)
                .Where(p => p.GrupoId == player.GrupoId && p.Id != player.Id)
                .OrderBy(p => p.Nickname.Trim().ToLower())
                .ToListAsync();

            var playersInSameEquip = await _context.Players
                .Include(p => p.Equipe)
                .Where(p => p.EquipeId == player.EquipeId && p.Id != player.Id)
                .OrderBy(p => p.Nickname.Trim().ToLower())
                .ToListAsync();

            var idLogado = await _context.Players
                .FirstOrDefaultAsync(p => p.UserId == userId);

            ViewBag.PlayerIdLogado = idLogado.Id;

            var viewModel = new PlayersEquipGroupViewModel
            {
                PlayersInSameGroup = playersInSameGroup,
                PlayersInSameEquip = playersInSameEquip
            };

            return View(viewModel);

        }
        [Authorize(Roles = "Admin, Treinador")]
        public async Task<IActionResult> CrudPlayer()
        {
            if (User.IsInRole("Treinador"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var treinadorid = await _context.Treinadores.Where(t => t.UserId == userId).Select(t => t.Id).FirstOrDefaultAsync();
                var players = await _context.Players
                .Where(p => p.TreinadorId == treinadorid)
                .Include(p => p.User)
                .OrderBy(p => p.Nome.Trim().ToLower())
                .ThenBy(p => p.Status.Trim().ToLower())
                .ToListAsync();
                return View(players);
            }
            else
            {
                var players = await _context.Players
                    .Include(p => p.User)
                    .ToListAsync();
                return View(players);
            }

        }

        [HttpGet]
        [Authorize(Roles = "Admin, Treinador, Player")]
        public async Task<IActionResult> EditPlayer(int id)
        {
            using (IDbConnection db = new SqliteConnection(_connectionString))
            {

                var query = @"SELECT p.*, u.Email 
                      FROM Players p 
                      JOIN AspNetUsers u ON p.UserId = u.Id 
                      WHERE p.Id = @Id";

                var player = db.Query<Player, IdentityUser, Player>(query,
                    (player, user) =>
                    {
                        player.User = user;
                        return player;
                    },
                    new { Id = id }, splitOn: "Email").FirstOrDefault();

                if (player == null)
                {
                    return NotFound();
                }

                return View(player);
            }
        }
      

        private void RegistrarHistorico(Player player)
        {
            // Obtém o jogador atual do banco
            var existingPlayer = _context.Players
                .Include(p => p.User)
                .FirstOrDefault(p => p.Id == player.Id);

            if (existingPlayer == null)
            {
                throw new Exception("Jogador não encontrado para registrar histórico.");
            }

            // Lista para armazenar dados alterados
            var dadosAlterados = new List<string>();

            // Compara os campos e registra as mudanças
            if (existingPlayer.Nome != player.Nome)
            {
                dadosAlterados.Add($"Nome alterado de '{existingPlayer.Nome}' para '{player.Nome}'");
            }

            if (existingPlayer.Status != player.Status)
            {
                dadosAlterados.Add($"Status alterado de '{existingPlayer.Status}' para '{player.Status}'");
            }

            if (existingPlayer.Nickname != player.Nickname)
            {
                dadosAlterados.Add($"Nickname alterado de '{existingPlayer.Nickname}' para '{player.Nickname}'");
            }

            if (existingPlayer.Tag != player.Tag)
            {
                dadosAlterados.Add($"Tag alterada de '{existingPlayer.Tag}' para '{player.Tag}'");
            }

            if (existingPlayer.Classe != player.Classe)
            {
                dadosAlterados.Add($"Classe alterada de '{existingPlayer.Classe}' para '{player.Classe}'");
            }

            if (existingPlayer.IGL != player.IGL)
            {
                dadosAlterados.Add($"IGL alterado de '{existingPlayer.IGL}' para '{player.IGL}'");
            }

            if (existingPlayer.User.Email != player.User.Email)
            {
                dadosAlterados.Add($"E-mail alterado de '{existingPlayer.User.Email}' para '{player.User.Email}'");
            }

            string descri = "";

            if (User.IsInRole("Treinador")){

                var userlogado = _userManager.GetUserId(User);
                var treinador = _context.Treinadores
                    .Where(t => t.UserId == userlogado)
                    .FirstOrDefault();

                descri = $"Alterações realizadas no perfil do player por {treinador.Nome}";
            }

            if (User.IsInRole("Admin"))
            {

                var userlogado = _userManager.GetUserId(User);
                var admin = _context.Admins
                    .Where(t => t.UserId == userlogado)
                    .FirstOrDefault();

                descri = $"Alterações realizadas no perfil do player por {admin.Nome}";
            }


            // Verifica se há alterações
            if (dadosAlterados.Any())
            {
                // Cria um novo registro de histórico
                var historico = new PlayerHistorico
                {
                    PlayerId = player.Id,
                    DataEvento = DateTime.Now,
                    TipoEvento = "Atualização de Dados",
                    Descricao = descri,
                    DadosAlterados = string.Join("; ", dadosAlterados)
                };

                // Adiciona ao contexto e salva
                _context.PlayerHistoricos.Add(historico);
                _context.SaveChanges();
            }


        }


        [HttpPost]
        [Authorize(Roles = "Admin, Treinador, Player")]
        public IActionResult EditPlayer(Player player)
        {
            try
            {
                // Obtém o jogador atual do banco
                var existingPlayer = _context.Players
                    .Include(p => p.User) // Inclui a entidade User associada
                    .FirstOrDefault(p => p.Id == player.Id);

                if (existingPlayer == null)
                {
                    ModelState.AddModelError(string.Empty, "Jogador não encontrado.");
                    return View(player);
                }

                RegistrarHistorico(player);

                // Atualiza os campos do jogador
                existingPlayer.Nome = player.Nome;
                existingPlayer.Status = player.Status;
                existingPlayer.Nickname = player.Nickname;
                existingPlayer.Tag = player.Tag;
                existingPlayer.FgCandidato = player.Status == "Candidato";
                existingPlayer.Classe = player.Classe;
                existingPlayer.IGL = player.IGL;

                // Atualiza o e-mail do usuário associado
                if (!string.IsNullOrEmpty(player.User.Email) && existingPlayer.User.Email != player.User.Email)
                {
                    existingPlayer.User.Email = player.User.Email;
                    existingPlayer.User.NormalizedEmail = player.User.Email.ToUpperInvariant();
                }

                // Salva as alterações
                _context.SaveChanges();


                // Redireciona após sucesso
                if (User.IsInRole("Treinador") || User.IsInRole("Admin"))
                {
                    return RedirectToAction("CrudPlayer", "Player");
                }
                else if (User.IsInRole("Player"))
                {
                    return RedirectToAction("PlayerIndex", "Player");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Erro durante a atualização: " + ex.Message);
                Console.WriteLine("Erro durante a atualização: " + ex.Message);
            }

            return View(player);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Treinador")]
        public async Task<IActionResult> DeletePlayer(int id)
        {
            using (IDbConnection db = new SqliteConnection(_connectionString))
            {

                var query = @"SELECT p.*, u.Email 
                      FROM Players p 
                      JOIN AspNetUsers u ON p.UserId = u.Id 
                      WHERE p.Id = @Id";

                var player = db.Query<Player, IdentityUser, Player>(query,
                    (player, user) =>
                    {
                        player.User = user;
                        return player;
                    },
                    new { Id = id }, splitOn: "Email").FirstOrDefault();

                if (player == null)
                {
                    return NotFound();
                }

                return View(player);
            }
        }

        [HttpPost, ActionName("DeletePlayer")]
        [Authorize(Roles = "Admin, Treinador")]
        public async Task<IActionResult> DeletePlayerConfirmed(int id)
        {
            using (IDbConnection db = new SqliteConnection(_connectionString))
            {

                db.Open();


                var userIdQuery = "SELECT UserId FROM Players WHERE Id = @Id";
                var userId = db.ExecuteScalar<string>(userIdQuery, new { Id = id });


                var deletePlayerQuery = "DELETE FROM Players WHERE Id = @Id";
                var deleteUserQuery = "DELETE FROM AspNetUsers WHERE Id = @UserId";

                using (var transaction = db.BeginTransaction())
                {
                    try
                    {

                        var playerResult = db.Execute(deletePlayerQuery, new { Id = id }, transaction);


                        var userResult = db.Execute(deleteUserQuery, new { UserId = userId }, transaction);

                        transaction.Commit();

                        if (playerResult != 0 && userResult != 0)
                        {
                            return RedirectToAction("CrudPlayer", "Player");
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

            return RedirectToAction("CrudPlayer", "Player");
        }


        [HttpGet]
        [Authorize(Roles = "Admin, Treinador")]
        public IActionResult CreatePlayer()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Treinador")]
        public async Task<IActionResult> CreatePlayer(CreatePlayerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingPlayer = await _context.Players
                    .FirstOrDefaultAsync(p => p.Nickname == model.Nickname && p.Tag == model.Tag);

            if (existingPlayer != null)
            {
                ModelState.AddModelError(string.Empty, "Já existe um jogador com essa combinação de Nickname e Tag.");
                return View(model);
            }

            var playerPuuid = await _valorantApiService.GetPuuid(model.Nickname, model.Tag);

            if (playerPuuid == null)
            {
                ModelState.AddModelError(string.Empty, "Conta não encontrada. Verifique o Nickname e Tag e tente novamente.");
                return View(model);
            }

            var user = new IdentityUser
            {
                UserName = model.UserName,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "Player");

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);

                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            var userlogado = _userManager.GetUserId(User);
            var treinadorlogado = 1;
            if (User.IsInRole("Treinador"))
            {
                treinadorlogado = _context.Treinadores
                    .Where(t => t.UserId == userlogado)
                    .Select(t => t.Id)
                    .FirstOrDefault();
            }

            var elo = await _valorantApiService.GetElo(playerPuuid.Puuid);

            if (elo == null)
            {
                ModelState.AddModelError(string.Empty, "Erro: O jogador tem que ter elo.");
                return View(model);
            }

            var TFgcandidato = true;
            var player = new Player
            {
                Nome = model.Nome,
                Nickname = model.Nickname,
                Tag = model.Tag,
                Status = model.Status,
                UserId = user.Id,
                TreinadorId = treinadorlogado,
                Puuid = playerPuuid.Puuid,
                EloCriacao = elo.Elo,
                DtCriacao = DateTime.Now,
                FgCandidato = TFgcandidato,
                DataNascimento = model.DataNascimento,
                Classe = model.Classe,
            };

            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            return RedirectToAction("CrudPlayer", "Player");
        }


        [Authorize(Roles = "Admin, Treinador")]
        public async Task<IActionResult> DesativarPlayer(int playerId)
        {
            var player = await _context.Players.SingleOrDefaultAsync(p => p.Id == playerId);

            if (player == null)
            {
                return NotFound();
            }

            player.Status = "Desativado";
            await _context.SaveChangesAsync();

            return RedirectToAction("CrudPlayer", "Player");
        }

        [Authorize(Roles = "Admin, Treinador")]
        public async Task<IActionResult> AtivarPlayer(int playerId)
        {
            var player = await _context.Players.SingleOrDefaultAsync(p => p.Id == playerId);

            if (player == null)
            {
                return NotFound();
            }

            player.Status = "Ativo";
            await _context.SaveChangesAsync();

            return RedirectToAction("CrudPlayer", "Player");
        }
        [HttpPost]
        public async Task<IActionResult> GetEloAtual()
        {
            var userId = _userManager.GetUserId(User);
            var puuid = _context.Players.Where(p => p.UserId == userId).Select(p => p.Puuid).FirstOrDefault();

            if (string.IsNullOrEmpty(puuid))
            {
                return Json(new { success = false, message = "Jogador não encontrado." });
            }

            var EloAtual = await _valorantApiService.GetElo(puuid);

            if (EloAtual != null)
            {
                return Json(new { success = true, elo = EloAtual.Elo });
            }
            else
            {
                return Json(new { success = false, message = "Erro ao buscar informações do jogador." });
            }
        }


        [HttpPost]
        public async Task<IActionResult> GetPlayerStats()
        {
            var userId = _userManager.GetUserId(User);
            var puuid = _context.Players.Where(p => p.UserId == userId).Select(p => p.Puuid).FirstOrDefault();

            if (string.IsNullOrEmpty(puuid))
            {
                return Json(new { success = false, message = "Jogador não encontrado." });
            }

            try
            {
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
                        winRate,
                        damage,
                        firstBloods
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Nenhuma partida competitiva encontrada para este jogador." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erro ao buscar estatísticas: {ex.Message}" });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Treinador, Player")]
        public async Task<IActionResult> Historico(int id)
        {
                var player = _context.Players.Where(p =>p.Id == id).FirstOrDefault();

                if (player == null)
                {
                    return NotFound();
                }

                ViewBag.Historico = _context.PlayerHistoricos.Where(h => h.PlayerId == id).OrderByDescending(h => h.DataEvento).ToList();

                ViewBag.Player = player;

                return View(player);
            
        }
    }
}
