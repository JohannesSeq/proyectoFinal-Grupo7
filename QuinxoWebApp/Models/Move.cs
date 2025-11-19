namespace QuinxoWebApp.Models
{
    public class Move
    {
        public int Id { get; set; }

        public int GameId { get; set; }
        public Game? Game { get; set; }

        public int MoveNumber { get; set; }

        public int PlayerId { get; set; }
        public Player? Player { get; set; }

        public int FromRow { get; set; }
        public int FromCol { get; set; }

        public int ToRow { get; set; }
        public int ToCol { get; set; }

        public string Symbol { get; set; } = string.Empty;  // "O", "X", "N"

        public string? PointOrientation { get; set; }        // "Top", "Right", "Bottom", "Left"

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
