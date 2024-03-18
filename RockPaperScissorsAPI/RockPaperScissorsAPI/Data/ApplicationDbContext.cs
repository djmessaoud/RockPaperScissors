using Microsoft.EntityFrameworkCore;
using RockPaperScissorsAPI.Models;

namespace RockPaperScissorsAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        //setting up the database tables (таблицы базы данных)
        public DbSet<User> users { get; set; }
        public DbSet<MatchHistory> matchhistory { get; set; }
        public DbSet<GameTransactions> gametransactions { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
