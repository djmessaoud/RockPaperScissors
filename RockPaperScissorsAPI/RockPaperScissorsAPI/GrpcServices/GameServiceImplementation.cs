using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using RockPaperScissorsAPI.Controllers;
using RockPaperScissorsAPI.Data;
using RockPaperScissorsAPI.Models;
using static RockPaperScissorsAPI.Models.MatchHistory;

namespace RockPaperScissorsAPI.GrpcServices
{
    public class GameServiceImplementation : GameService.GameServiceBase
    {
        private readonly ApplicationDbContext _context;

        public GameServiceImplementation(ApplicationDbContext context) // конструктор сервиса ( с БД)
        {
            _context = context;
        }

        // функция получения баланса игрока с БД
        public override async Task<BalanceReply> GetBalance(BalanceRequest request, ServerCallContext context)
        {
            var user = await _context.users.FindAsync(request.UserId);
            return new BalanceReply
            {
                Balance = (double)(user?.balance ?? 0)
            };
        }
        // функция получения списка матчей с БД
        public override async Task<ListGamesReply> ListGames(ListGamesRequest request, ServerCallContext context)
        {
            
            var games = await _context.matchhistory
                .Select(m => new GameInfo
                {
                    GameId = m.matchId,
                    Stake = (double)m.stake,
                    Status = m.playerTwoId == null ? "waiting" : "full"
                }).ToListAsync();

            return new ListGamesReply
            {
                Games = { games }
            };
        }
        // функция вступления в матч 
        public async override Task<JoinGameReply> JoinGame(JoinGameRequest request, ServerCallContext context)
        {
            //найти матч и игрока в БД
            var match = await _context.matchhistory.FindAsync(request.GameId);
            var user = await _context.users.FindAsync(request.UserId);
            // если матч или игрок не найдены, то возвращаем сообщение об ошибке
            if (match == null || user == null)
            {
                return new JoinGameReply { Success = false, Message = "Матч или пользователь не найден." };
            }
            // если матч не в ожидании игрока, то возвращаем сообщение об ошибкеы (Full/InProgress)
            if (match.GameState !=(MatchHistory.GameStates.WaitingForPlayer))
            {
                return new JoinGameReply { Success = false, Message = "Невозможно присоединиться к матчу." };
            }
            // если у игрока недостаточно средств, то возвращаем сообщение об ошибке
            if (user.balance < match.stake)
            {
                return new JoinGameReply { Success = false, Message = "Недостаточно средств." };
            }
            // вступление в матч и изменение статуса матча 
            if (!match.playerOneId.HasValue) // если первого игрока нет, то добавляем как первого
            {
                match.playerOneId = request.UserId;
            }
            else if (match.playerOneId == request.UserId) // если игрок уже в матче, то возвращаем сообщение об ошибке
            {
                return new JoinGameReply { Success = false, Message = "Вы уже в матче." };
            }
            else if (!match.playerTwoId.HasValue) // если первый игрок ест а второго игрока нету, то добавляем как второго
            {
                match.playerTwoId = request.UserId;
                match.GameState = MatchHistory.GameStates.InProgress; // меняем статус матча на "in progress"
            }
            else // если второго игрока уже есть, то возвращаем сообщение что матч полон 
            {
                return new JoinGameReply { Success = false, Message = "Матч уже заполнен." };
            }
            await _context.SaveChangesAsync(); // сохраняем изменения в БД
            // если все успешно, то возвращаем сообщение об успешном вступлении в матч
            return new JoinGameReply { Success = true, Message = "Успешно присоединились к матчу!" };
        }
        // функция совершения хода в матче
        public override async Task<MakeMoveReply> MakeMove(MakeMoveRequest request, ServerCallContext context)
        {
            var match = await _context.matchhistory.FindAsync(request.MatchId); // найти матч в БД

            // создание хода для игрока
            if (request.UserId == match.playerOneId)
            {
                match.playerOneChoice = (MatchHistory.GameChoice)request.Choice;
            }
            else if (request.UserId == match.playerTwoId)
            {
                match.playerTwoChoice = (MatchHistory.GameChoice)request.Choice;
            }
            else // если игрок не участвует в матче, то возвращаем сообщение об ошибке
            { 
                return new MakeMoveReply { Success = false, Message = "Вы не являетесь участником этого матча." };
            }

            // если оба игрока сделали ход, то решаем игру
            if (match.playerOneChoice.HasValue && match.playerTwoChoice.HasValue)
            {
                ResolveGame(match); // решение игры
                match.GameState = MatchHistory.GameStates.Completed; // меняем статус матча на "completed"
            }
                // сохраняем изменения в БД
            await _context.SaveChangesAsync();
            // возвращаем сообщение об успешном ходе
            return new MakeMoveReply { Success = true, Message = "Ход успешно завершен." };
        }
      

        // функция решения игры
        private MatchHistory ResolveGame(MatchHistory game)
        {
            // если игроки не сделали ход, то возвращаем сообщение об ошибке
            if (game.playerOneChoice == null || game.playerTwoChoice == null)
            {
                throw new InvalidOperationException("Невозможно разрешить игру без выбора обоих игроков.");
            }
            // если игроки выбрали одинаковые ходы то ничья
            if (game.playerOneChoice == game.playerTwoChoice)
            {
                game.winnerId = 0;
            }
            // если игроки выбрали разные ходы, то определяем победителя
            else if ((game.playerOneChoice == MatchHistory.GameChoice.Rock && game.playerTwoChoice == MatchHistory.GameChoice.Scissors) ||
                     (game.playerOneChoice == MatchHistory.GameChoice.Scissors && game.playerTwoChoice == MatchHistory.GameChoice.Paper) ||
                     (game.playerOneChoice == MatchHistory.GameChoice.Paper && game.playerTwoChoice == MatchHistory.GameChoice.Rock))
            {
               // первый игрок победил
                game.winnerId = game.playerOneId;
                // перевод денег от второго игрока к первому
                var transferRequest = new TransferMoneyRequest
                {
                    fromUserId = game.playerTwoId.Value,
                    toUserID = (int)game.playerOneId,
                    amount = game.stake
                };
                var result = new UsersController(_context).TransferMoney(transferRequest).Result;
                Console.WriteLine(result);
            }
            else
            {
                // второй игрок победил
                game.winnerId = game.playerTwoId.HasValue ? game.playerTwoId.Value : throw new InvalidOperationException("Невозможно определить победителя в игре с одним игроком.");
                // перевод денег от первого игрока ко второму
                var transferRequest = new TransferMoneyRequest
                {
                    fromUserId = (int)game.playerOneId,
                    toUserID = game.playerTwoId.Value,
                    amount = game.stake
                };
                var result = new UsersController(_context).TransferMoney(transferRequest).Result;
                Console.WriteLine(result);

            }

            return game;
        }
        // функция проверки статуса матча чтоб перевести игрок в следующий шаг игры или ждать еще одного игрока
        public override async Task<CheckGameStatusReply> CheckGameStatus(CheckGameStatusRequest request, ServerCallContext context)
        {
            var match = await _context.matchhistory.FindAsync(request.MatchId);

            if (match == null)
            {
                return new CheckGameStatusReply { State = GameStates.WaitingForPlayer };
            }

            var status = match.GameState;
            GameStates gRPC = (GameStates)status;
            return new CheckGameStatusReply { State =  gRPC };
        }

        // функция проверки результата игры (победа, поражение, ничья) или ожидание хода другого игрока
        public override async Task<CheckGameResultReply> CheckGameResult(CheckGameResultRequest request, ServerCallContext context)
        {
            var match = await _context.matchhistory.FindAsync(request.MatchId);

            if (match == null || match.GameState != MatchHistory.GameStates.Completed)
            {
                return new CheckGameResultReply { State = GameStates.InProgress, Message= "Игра еще не закончена! Ожидание хода!" };
            }
            var theWinner= await _context.users.FindAsync(match.winnerId); // найти победителя в БД 
            // возвращаем сообщение о победе, поражении или ничьей
            if (match.winnerId==0)
            {
                return new CheckGameResultReply { State = GameStates.Completed, Message = "Это ничья!" };
            }
            else if (match.winnerId.HasValue)
            {
                return new CheckGameResultReply { State = GameStates.Completed, Message = $"Победитель - ID {match.winnerId} [ {theWinner.username} ] || {match.stake}$" };
            }
            else
            {
                return new CheckGameResultReply { State = GameStates.Completed, Message = "Игра еще не закончена! Ждем хода!" };
            }
        }
    }
}
  
