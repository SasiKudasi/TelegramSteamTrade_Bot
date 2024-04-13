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
        long person = update.Message.Chat.Id;
        if (!Data.GetUser(person))
        {
            await client.SendTextMessageAsync(update.Message!.Chat.Id, "Произошла непредвиденная ошибка, пожалуйста попробуйте позднее");
        }
        else
        {
            await Data.SetStateAsync(client, update, token);
            var mode = Data.GetModeMain(person);
            switch (mode)
            {
                case ModeMain.Start:
                    await SendMenu(client, update);
                    break;
                case ModeMain.GetItem:
                    await Data.ItemMenuAsync(client, update, token);
                    Data.SetState(person, ModeMain.GetItem);
                    break;
                case ModeMain.AddItem:
                    Data.SetState(person, ModeMain.Start);
                    break;
                case ModeMain.GetAllItem:
                    Data.SetState(person, ModeMain.Start);
                    break;
            }
        }
    }

    private static async Task SendMenu(ITelegramBotClient client, Telegram.Bot.Types.Update update)
    {
        await client.SendTextMessageAsync(update.Message!.Chat.Id, "Привет, я Бот, который поможет тебе с отслеживанием цен на внутриигровые предметы.\n" +
            "/check_item_price - если ты просто хочешь посмотреть цену на определенный предмет.\n" +
            "/add_item_to_track - если ты хочешь отслеживать его цену.\n" +
            "/check_tracking_item - если ты хочешь посмотреть актуальные цены на все предметы, что ты добавил.\n" +
            "/start - для возврата в главное меню.");
    }

    private static Task ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    static string ChoseGameItem()
    {
        Console.WriteLine("Введите название предмета\nприм. AK-47 | Redline (Minimal Wear)");
        string item = Console.ReadLine();
        return item;
    }
}

