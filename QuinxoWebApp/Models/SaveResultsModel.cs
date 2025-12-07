namespace QuinxoWebApp.Models
{
    // ============================================================
    // DTOs PARA SaveResult()
    // ============================================================

    public class SaveGameRequest
    {
        public int GameId { get; set; }
        public int DurationSeconds { get; set; }
        public string FinalState { get; set; } = "";
        public int? WinnerPlayerId { get; set; }
        public string? WinnerTeam { get; set; }
        public List<MoveDTO> Moves { get; set; } = new();
    }

    public class MoveDTO
    {
        public int MoveNumber { get; set; }
        public int PlayerId { get; set; }
        public int FromRow { get; set; }
        public int FromCol { get; set; }
        public int ToRow { get; set; }
        public int ToCol { get; set; }
        public string Symbol { get; set; } = "";
        public string? PointOrientation { get; set; }
    }
}