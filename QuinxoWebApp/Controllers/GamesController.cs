using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuinxoWebApp.Data;
using QuinxoWebApp.Models;
using System.Text;
using System.Xml.Linq;

namespace QuinxoWebApp.Controllers
{
    public class GamesController : Controller
    {
        private readonly AppDbContext _context;

        public GamesController(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // LISTADO DE PARTIDAS FINALIZADAS
        // ============================================================

        public async Task<IActionResult> Finished()
        {
            var list = await _context.Games
                .Include(g => g.WinnerPlayer)
                .Where(g => g.FinishedAt != null)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();

            return View(list);
        }

        // ============================================================
        // CREAR NUEVA PARTIDA (vista)
        // ============================================================

        public IActionResult New()
        {
            return View();
        }

        // ============================================================
        // CREAR PARTIDA (POST)
        // ============================================================

        [HttpPost]
        public async Task<IActionResult> New(int mode, List<string> players)
        {
            // ELIMINAR VACÍOS
            players = players.Where(p => !string.IsNullOrWhiteSpace(p)).ToList();

            var game = new Game
            {
                Mode = mode,
                CreatedAt = DateTime.UtcNow
            };

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            int order = 1;
            foreach (var playerName in players)
            {
                var existing = await _context.Players
                    .FirstOrDefaultAsync(p => p.Name == playerName);

                if (existing == null)
                {
                    existing = new Player { Name = playerName };
                    _context.Players.Add(existing);
                    await _context.SaveChangesAsync();
                }

                _context.GamePlayers.Add(new GamePlayer
                {
                    PlayerId = existing.Id,
                    GameId = game.Id,
                    Order = order,
                    Team = mode == 2
                        ? order % 2 == 0 ? "B" : "A"
                        : order == 1 || order == 3 ? "A" : "B"
                });

                order++;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Play", new { id = game.Id });
        }


        // ============================================================
        // PLAY: MOSTRAR TABLERO
        // ============================================================

        public async Task<IActionResult> Play(int id)
        {
            var game = await _context.Games
                .Include(g => g.GamePlayers)
                .ThenInclude(gp => gp.Player)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null) return NotFound();

            return View(game);
        }


        // ============================================================
        // GUARDAR PARTIDA (POST desde JS)
        // ============================================================

        [HttpPost]
        public async Task<IActionResult> SaveResult([FromBody] SaveGameRequest request)
        {
            var game = await _context.Games.FindAsync(request.GameId);

            if (game == null)
                return NotFound();

            game.FinishedAt = DateTime.UtcNow;
            game.DurationSeconds = request.DurationSeconds;
            game.FinalState = request.FinalState;
            game.WinnerPlayerId = request.WinnerPlayerId;
            game.WinnerTeam = request.WinnerTeam;

            foreach (var mv in request.Moves)
            {
                _context.Moves.Add(new Move
                {
                    GameId = game.Id,
                    MoveNumber = mv.MoveNumber,
                    PlayerId = mv.PlayerId,
                    FromRow = mv.FromRow,
                    FromCol = mv.FromCol,
                    ToRow = mv.ToRow,
                    ToCol = mv.ToCol,
                    Symbol = mv.Symbol,
                    PointOrientation = mv.PointOrientation,
                    Timestamp = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();

            return Ok();
        }


        // ============================================================
        // VER DETALLES DE UNA PARTIDA
        // ============================================================

        public async Task<IActionResult> Details(int id)
        {
            var game = await _context.Games
                .Include(g => g.GamePlayers).ThenInclude(gp => gp.Player)
                .Include(g => g.Moves)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null) return NotFound();

            var moves = game.Moves
                .OrderBy(m => m.MoveNumber)
                .ToList();

            ViewBag.Moves = moves;

            return View(game);
        }


        // ============================================================
        // EXPORTAR A XML
        // ============================================================

        public async Task<IActionResult> ExportXml(int id)
        {
            var game = await _context.Games
                .Include(g => g.GamePlayers).ThenInclude(gp => gp.Player)
                .Include(g => g.Moves)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null) return NotFound();

            var xml = new XElement("Game",
                new XAttribute("id", game.Id),
                new XElement("Mode", game.Mode),
                new XElement("CreatedAt", game.CreatedAt),
                new XElement("FinishedAt", game.FinishedAt),
                new XElement("DurationSeconds", game.DurationSeconds),
                new XElement("WinnerPlayer", game.WinnerPlayer?.Name ?? ""),
                new XElement("WinnerTeam", game.WinnerTeam),
                new XElement("FinalState", game.FinalState),
                new XElement("Players",
                    game.GamePlayers.Select(gp =>
                        new XElement("Player",
                            new XAttribute("order", gp.Order),
                            new XAttribute("team", gp.Team ?? ""),
                            gp.Player!.Name
                        )
                    )
                ),
                new XElement("Moves",
                    game.Moves
                        .OrderBy(m => m.MoveNumber)
                        .Select(m =>
                            new XElement("Move",
                                new XAttribute("number", m.MoveNumber),
                                new XElement("Player", m.Player!.Name),
                                new XElement("Symbol", m.Symbol),
                                new XElement("PointOrientation", m.PointOrientation ?? ""),
                                new XElement("From",
                                    new XAttribute("row", m.FromRow),
                                    new XAttribute("col", m.FromCol)
                                ),
                                new XElement("To",
                                    new XAttribute("row", m.ToRow),
                                    new XAttribute("col", m.ToCol)
                                )
                            )
                        )
                )
            );

            var bytes = Encoding.UTF8.GetBytes(xml.ToString());
            var fileName = $"quixo-game-{id}.xml";

            return File(bytes, "application/xml", fileName);
        }
    }
}
