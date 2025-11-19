namespace QuinxoWebApp.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<GamePlayer>? GamePlayers { get; set; }
        public ICollection<Game>? GamesWon { get; set; }
    }
}
