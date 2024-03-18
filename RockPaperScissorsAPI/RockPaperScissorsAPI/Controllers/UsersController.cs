using Microsoft.AspNetCore.Mvc;
using RockPaperScissorsAPI.Data;
using RockPaperScissorsAPI.Models;

namespace RockPaperScissorsAPI.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context) // конструктор контроллера ( с БД)
        {
            _context = context;
        }

        [HttpPost("transfer")] // функция перевода денег между игроками (POST) 
        public async Task<IActionResult> TransferMoney([FromBody] TransferMoneyRequest request)
        {
            // поиск игроков в БД по ID 
            var fromUser = await _context.users.FindAsync(request.FromUserId);
            var toUser = await _context.users.FindAsync(request.ToUserId);
            // если игрок не найден, то возвращаем 404
            if (fromUser == null || toUser == null) return NotFound("игрок не найден");
            // если у игрока недостаточно средств, то возвращаем 400 и сообщение
            if (fromUser.balance < request.Amount)
            {
                return BadRequest("Недостаточно средств.");
            }
            // перевод денег между игроками если все условия выполнены
            fromUser.balance -= request.Amount;
            toUser.balance += request.Amount;
            // обект транзакции для записи в БД
            var transaction = new GameTransactions
            {
                fromUserId = request.FromUserId,
                toUserId = request.ToUserId,
                amount = request.Amount,
                timestamp = DateTime.UtcNow
            };
            // добавление транзакции в БД
            _context.gametransactions.Add(transaction);
            await _context.SaveChangesAsync();
            // возвращаем 200 и сообщение об успешном переводе
            return Ok("Транзакция успешно завершена");
        }
    }
    // класс запроса на перевод денег
    public class TransferMoneyRequest
    {
        
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
        public decimal Amount { get; set; }
    }
}

