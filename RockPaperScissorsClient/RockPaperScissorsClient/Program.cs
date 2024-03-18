using Grpc.Net.Client;
using RockPaperScissorsAPI.GrpcServices;
using System.Text.RegularExpressions;



using var channel = GrpcChannel.ForAddress("http://localhost:5132"); //адрес сервера
var client = new GameService.GameServiceClient(channel);

// Меню для выбора действия
while (true)
{
    Console.WriteLine("\nВыберите действие:");
    Console.WriteLine("1 - Получить баланс");
    Console.WriteLine("2 - Список игр");
    Console.WriteLine("3 - Присоединяйтесь к игре");
    Console.WriteLine("0 - Выход");
    Console.Write("Введите выбор: ");

    var option = Console.ReadLine();

    switch (option)
    {
        case "1":
            await GetBalance(client);
            break;
        case "2":
            await ListGames(client);
            break;
        case "3":
            await JoinGameAsync(client);
            break;
        case "0":
            return;
        default:
            Console.WriteLine("Неверный вариант.");
            break;
    }
}
// функция для получения баланса
static async Task GetBalance(GameService.GameServiceClient client)
{
    Console.Write("Введите свой токен: "); //uID
    var userId = int.Parse(Console.ReadLine());
    var reply = await client.GetBalanceAsync(new BalanceRequest { UserId = userId });
    Console.WriteLine($"Баланс: {reply.Balance}");
}

// функция для получения списка игр
static async Task ListGames(GameService.GameServiceClient client)
{
    var reply = await client.ListGamesAsync(new ListGamesRequest());
    foreach (var game in reply.Games)
    {
        Console.WriteLine($"ID игры: {game.GameId}, Сумма ставки: {game.Stake}, Статус: {game.Status}");
    }
}
// функция для присоединения к игре
 async Task JoinGameAsync(GameService.GameServiceClient client)
{
    // Получение токен игрока (userID) и идентификатора игры от пользователя
    Console.WriteLine("Введите свой токен: ");
    var userId = int.Parse(Console.ReadLine());
    Console.WriteLine("Введите идентификатор игры, к которой вы хотите присоединиться: ");
    var gameId = int.Parse(Console.ReadLine());
    // Отправка запроса на присоединение к игре
    var response = await client.JoinGameAsync(new JoinGameRequest 
    {
        UserId = userId,
        GameId = gameId
    });

    Console.WriteLine(response.Message);
    if (response.Success) // если присоединение прошло успешно - ожидание хода другого игрока 
    {
        bool isMatchReady = false; // флаг для проверки готовности игры
        while (!isMatchReady) // Проверка статуса игры каждые 2 секунды
        {
            var statusResponse = await client.CheckGameStatusAsync(new CheckGameStatusRequest { MatchId = gameId }); 
            if (statusResponse.State == GameStates.InProgress) // Если игра в процессе, то значит к игре присоединился еще один игрок
            {
                isMatchReady = true;
                Console.WriteLine("К вам присоединился еще один игрок. Пожалуйста, сделайте свой ход. (КАМЕНЬ, БУМАГА, НОЖНИЦЫ):");
                await MakeMoveAsync(client, gameId, userId);
            }
            else //статус игры не изменился, ждем
            {
                Console.WriteLine("Ждем, когда присоединится еще один игрок...");
                await Task.Delay(2000); // ждем 2 секунды и проверяем статус игры снова
            }
        }
        
    }
}

// функция для совершения хода
 async Task MakeMoveAsync(GameService.GameServiceClient client, int gameID, int uID)
{
    // Получение выбора игрока
    Console.WriteLine("Выберите свой ход (Камень = 0, Бумага = 1, Ножницы = 2):");
    // Проверка ввода пользователя на корректность
    if (!Enum.TryParse<GameChoice>(Console.ReadLine(), out var choice))
    {
        Console.WriteLine("Неверный выбор.");
        return;
    }
    // Отправка запроса на совершение хода
    try
    {
        var response = await client.MakeMoveAsync(new MakeMoveRequest
        {
            MatchId = gameID,
            UserId = uID,
            Choice = choice
        });
        // Вывод сообщения от сервера
        Console.WriteLine(response.Message);

        // Проверка результата игры
        bool isGameResolved = false; // флаг для проверки окончания игры
        while (!isGameResolved) // Проверка статус резултата игры каждые 2 секунды
        {
            var resultResponse = await client.CheckGameResultAsync(new CheckGameResultRequest { MatchId = gameID });
            // Если игра окончена, то выводим сообщение о результате
            if (resultResponse.State == GameStates.Completed)
            {
                isGameResolved = true;
                Console.WriteLine($"Игра окончена. {resultResponse.Message}");
            }
            // Если игра не окончена,значит 2ой игрок еще не сделал ход, мы ждем 2 секунды и проверяем статус результата игры снова
            else
            {
                Console.WriteLine("Ждем, когда другой игрок сделает ход...");
                await Task.Delay(2000);
            }
        }

    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
    }
