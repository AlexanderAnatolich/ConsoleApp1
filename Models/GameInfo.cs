using System;

namespace ConsoleApp1.Models
{
    public class GameInfo
    {
        public Guid GameId { get; set; } = Guid.NewGuid();
        public string HomeTeam { get; set; }
        public int HomeTeamScore { get; set; }
        public string AwayTeam { get; set; }
        public int AwayTeamScore { get; set; }
        public bool IsFinished { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    }
}
