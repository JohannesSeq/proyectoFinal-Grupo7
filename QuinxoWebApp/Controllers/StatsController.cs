using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuinxoWebApp.Data;
using QuinxoWebApp.Models;

namespace QuinxoWebApp.Controllers
{
    public class StatsController : Controller
    {
        private readonly AppDbContext _context;

        public StatsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // ================================
            // ESTADÍSTICAS MODO 2 JUGADORES
            // ================================
            var mode2Games = await _context.Games
                .Where(g => g.Mode == 1 && g.FinishedAt != null)
                .ToListAsync();

            var mode2Stats = mode2Games
                .SelectMany(g => _context.GamePlayers.Where(gp => gp.GameId == g.Id))
                .GroupBy(gp => gp.PlayerId)
                .Select(g =>
                {
                    var player = _context.Players.First(p => p.Id == g.Key);
                    var total = g.Count();
                    var won = mode2Games.Count(x => x.WinnerPlayerId == g.Key);

                    return new PlayerStats
                    {
                        PlayerName = player.Name,
                        Total = total,
                        Won = won,
                        Effectiveness = total == 0 ? 0 : (won * 100) / total
                    };
                })
                .OrderByDescending(s => s.Effectiveness)
                .ToList();

            // ================================
            // ESTADÍSTICAS MODO 4 JUGADORES
            // ================================
            var mode4Games = await _context.Games
                .Where(g => g.Mode == 2 && g.FinishedAt != null)
                .ToListAsync();

            var teamStats = mode4Games
                .GroupBy(g => g.WinnerTeam)
                .Select(g => new TeamStats
                {
                    Team = g.Key ?? "",
                    Won = g.Count(),
                    Total = mode4Games.Count(),
                    Effectiveness = mode4Games.Count() == 0
                        ? 0
                        : (g.Count() * 100) / mode4Games.Count()
                })
                .OrderByDescending(s => s.Effectiveness)
                .ToList();

            ViewBag.Mode2 = mode2Stats;
            ViewBag.Mode4 = teamStats;

            return View();
        }
    }

    public class PlayerStats
    {
        public string PlayerName { get; set; } = "";
        public int Total { get; set; }
        public int Won { get; set; }
        public int Effectiveness { get; set; }
    }

    public class TeamStats
    {
        public string Team { get; set; } = "";
        public int Total { get; set; }
        public int Won { get; set; }
        public int Effectiveness { get; set; }
    }
}
