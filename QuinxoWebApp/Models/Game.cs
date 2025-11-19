namespace QuinxoWebApp.Models
{
    public class Game
    {
        public int Id { get; set; }

        // 1 = modo 2 jugadores, 2 = modo 4 jugadores
        public int Mode { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? FinishedAt { get; set; }

        public int DurationSeconds { get; set; }

        public int? WinnerPlayerId { get; set; }
        public Player? WinnerPlayer { get; set; }

        public string? WinnerTeam { get; set; }  // "A" o "B"

        // Guarda el estado final del tablero (JSON o texto 25 posiciones)
        public string FinalState { get; set; } = string.Empty;

        public ICollection<GamePlayer>? GamePlayers { get; set; }
        public ICollection<Move>? Moves { get; set; }
    }
}
