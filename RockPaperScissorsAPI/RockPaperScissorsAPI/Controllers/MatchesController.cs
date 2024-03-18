using Microsoft.AspNetCore.Mvc;
using RockPaperScissorsAPI.Data;
using RockPaperScissorsAPI.Models;

namespace RockPaperScissorsAPI.Controllers
{
    public class MatchesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MatchesController(ApplicationDbContext context) // конструктор контроллера ( с БД)
        {
            _context = context;
        }

        [HttpPost("create")] // функция создания матча (POST)
        public async Task<IActionResult> CreateMatch([FromBody] CreateMatchRequest request)
        {
            // создание объекта матча для добавления в БД
            var match = new MatchHistory
            {
                stake = request.Stake,
                GameState = MatchHistory.GameStates.WaitingForPlayer,
                timestamp = DateTime.UtcNow
            };
            // добавление матча в БД
            _context.matchhistory.Add(match);
            await _context.SaveChangesAsync();
            // возвращаем 200 и сообщение об успешном создании матча
            return Ok($"Матч создан! ID матча : {match.matchId}");
        }
    }
    // класс запроса на создание матча
    public class CreateMatchRequest
    {
        public decimal Stake { get; set; }
    }
}
