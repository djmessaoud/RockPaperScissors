using System.ComponentModel.DataAnnotations;

namespace RockPaperScissorsAPI.Models
{
    public class User
    {
        [Key]
        public int userid { get; set; } // Primary key
        public string username { get; set; }
        public decimal balance { get; set; }
    }
}
