using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using tcc_in305b.Data;
using tcc_in305b.Models;

public class NomeFaseViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public NomeFaseViewComponent(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userId = _userManager.GetUserId(UserClaimsPrincipal);

        // Verificar se o usuário é Treinador
        if (User.IsInRole("Treinador"))
        {
            var treinador = await _context.Treinadores
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (treinador == null)
            {
                return View("Default", "Treinador não encontrado");
            }

            var fases = await _context.FaseTreinadores
                .Where(ft => ft.TreinadorId == treinador.Id)
                .Select(ft => ft.Fase.Nome)
                .ToListAsync();

            if (fases == null || !fases.Any())
            {
                return View("Default", "Sem Fase");
            }

            return View("Default", fases);
        }

        // Verificar se o usuário é Player
        if (User.IsInRole("Player"))
        {
            var player = await _context.Players
                .Include(p => p.Fase) // Incluir a fase associada
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (player == null)
            {
                return View("Default", "Player não encontrado");
            }

            var fase = player.Fase?.Nome ?? "Sem Fase";

            return View("Default", new List<string> { fase });
        }

        // Caso nenhum dos papéis seja encontrado
        return View("Default", "Fase não encontrada");
    }
}
