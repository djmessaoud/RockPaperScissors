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
            var fromUser = await _context.users.FindAsync(request.fromUserId);
            var toUser = await _context.users.FindAsync(request.toUserID);
            // если игрок не найден, то возвращаем 404
            if (fromUser == null || toUser == null) return NotFound("игрок не найден");
            // если у игрока недостаточно средств, то возвращаем 400 и сообщение
            if (fromUser.balance < request.amount)
            {
                return BadRequest("Недостаточно средств.");
            }
            // перевод денег между игроками если все условия выполнены
            fromUser.balance -= request.amount;
            toUser.balance += request.amount;
            // обект транзакции для записи в БД
            var transaction = new GameTransactions
            {
                fromUserId = request.fromUserId,
                toUserId = request.toUserID,
                amount = request.amount,
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
        
        public int fromUserId { get; set; }
        public int toUserID { get; set; }
        public decimal amount { get; set; }
    }
}

