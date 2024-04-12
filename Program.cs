using TelegramSteamTrade_Bot;
using Telegram.Bot;
using Telegram.Bot.Polling;
using TelegramSteamTrade_Bot.Models;

class Program
{
    static async Task Main(string[] args)
    {
        var token = System.IO.File.ReadAllText("token.txt");
        var bot = new TelegramBotClient(token);

        var receiver = new ReceiverOptions
        {
            AllowedUpdates = new Telegram.Bot.Types.Enums.UpdateType[] { },
        };
        bot.StartReceiving(updateHandler: Handler, pollingErrorHandler: ErrorHandler, receiverOptions: receiver);
        Console.ReadLine();
        //  string cmd = "";
        //List<SteamMethod> items = new List<SteamMethod>();
        //while (true)
        //{
        //    int gameId = ChoseTheGame();
        //    string gameItem = ChoseGameItem();
        //    SteamMethod steam = new();
        //    await steam.SearchItemPriceAsync(gameId, gameItem);
        //    Console.WriteLine($"Предмет {steam.ItemName} стоит: {steam.ItemLowestPrice} руб\nХотите ли вы добавить этот предмет для отслеживания его цены?");
        //    cmd = Console.ReadLine();
        //    if (cmd == "Да")
        //    {
        //        items.Add(new SteamMethod(steam.GameID, steam.ItemName, steam.ItemLowestPrice));
        //        foreach (SteamMethod item in items)
        //        {
        //            double newPrice = await steam.SearchItemPriceAsync(item.GameID, item.ItemName);
        //            Console.WriteLine($"{item.GameID} Предмет {item.ItemName} стоил на момент добавления: {item.ItemLowestPrice} \nЕго актуальная цена {newPrice}");
        //        }
        //    }



    }
    private static async Task Handler(ITelegramBotClient client, Telegram.Bot.Types.Update update, CancellationToken token)
    {
        long person = update.Id;
        Mode mode = Data.GetMode(person);
        if (mode == null)
        {
            Data.SetState(person, Mode.Start);
        }
        else
        {
            switch (mode)
            {

                case Mode.Start:
                    await client.SendTextMessageAsync(update.Message!.Chat.Id, "Привет, я Бот, который поможет тебе с отслеживанием цен на внутриигровые предметы.\n" +
                        "Пожалуйста, выбери игру, цены на предметы которой ты хочешь посмотреть (/chouse_game)");
                    Data.SetState(person, Mode.ChouseGame);
                    break;
                case Mode.ChouseGame:
                    break;
                case Mode.AddItem:
                    break;
                case Mode.GetItem:
                    break;
                case Mode.GetAllItem:
                    break;
            }
        }
    }

    private static Task ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        throw new NotImplementedException();
    }



    static int ChoseTheGame()
    {
        Console.WriteLine("Выберите игру");
        string game = Console.ReadLine();
        int gameId = 0;
        switch (game)
        {
            case "CS2":
                gameId = 730;
                break;
            case "Dota2":
                gameId = 570;
                break;
            default:
                break;
        }
        return gameId;
    }
    static string ChoseGameItem()
    {
        Console.WriteLine("Введите название предмета\nприм. AK-47 | Redline (Minimal Wear)");
        string item = Console.ReadLine();
        return item;
    }
}

