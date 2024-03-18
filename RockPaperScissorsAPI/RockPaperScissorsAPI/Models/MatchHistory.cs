using System.ComponentModel.DataAnnotations;

namespace RockPaperScissorsAPI.Models
{
    public class MatchHistory
    {
        public enum GameChoice // (чтоб знать что выбрал игрок и сравнить с выбором другого игрока и тоже сохранить в БД)
        {
            Rock = 0,
            Paper = 1,
            Scissors = 2
        }
        public enum GameStates //To be used in checking the state of the game in gRPC (чтоб знать что делать дальше и орнагизовать игру для игроков)
        {
            WaitingForPlayer, InProgress, Completed // 0,1,2
        }

        [Key]
        public int matchId { get; set; } // Primary key
        public int? playerOneId { get; set; }
        public int? playerTwoId { get; set; }
        public int? winnerId { get; set; }
        public decimal stake { get; set; }
        public DateTime timestamp { get; set; }
        public GameChoice? playerOneChoice { get; set; }
        public GameChoice? playerTwoChoice { get; set; }

        public GameStates GameState { get; set; }
    }
}
