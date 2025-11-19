namespace QuinxoWebApp.Models
{
    public class GamePlayer
    {
        public int Id { get; set; }

        public int GameId { get; set; }
        public Game? Game { get; set; }

        public int PlayerId { get; set; }
        public Player? Player { get; set; }

        public int Order { get; set; }   // 1-2 ó 1-4

        public string? Team { get; set; } // null en modo 2 jugadores, "A" o "B" en modo 4
    }
}
