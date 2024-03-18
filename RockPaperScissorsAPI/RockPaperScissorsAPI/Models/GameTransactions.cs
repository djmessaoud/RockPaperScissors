using System.ComponentModel.DataAnnotations;

namespace RockPaperScissorsAPI.Models
{
    public class GameTransactions
    {
        [Key]
        public int transactionId { get; set; } //Primary key
        public int fromUserId { get; set; }
        public int toUserId { get; set; }
        public decimal amount { get; set; }
        public DateTime timestamp { get; set; }
    }
}
