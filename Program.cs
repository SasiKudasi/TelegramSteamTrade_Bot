using Telegram.Bot;
using Telegram.Bot.Polling;
using TelegramSteamTrade_Bot.Models;
using TelegramSteamTrade_Bot.Data;

class Program
{
    private static UsersData _userData = new UsersData();
    private static ItemsData _itemsData = new ItemsData();
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
    }
    private static async Task Handler(ITelegramBotClient client, Telegram.Bot.Types.Update update, CancellationToken token)
    {
        long person = update.Message.Chat.Id;
        if (!_userData.GetUser(person))
        {
            await client.SendTextMessageAsync(update.Message!.Chat.Id, "Произошла непредвиденная ошибка, пожалуйста попробуйте позднее");
        }
        else
        {
            await _userData.SetStateAsync(client, update, token);
            var mode = _userData.GetModeMain(person);
            switch (mode)
            {
                case ModeMain.Start:
                    await SendMenu(client, update);
                    break;
                case ModeMain.GetItem:
                   await _itemsData.ItemMenuAsync(client, update,token);
                    _userData.SetState(person, ModeMain.GetItem);
                    break;
                case ModeMain.AddItem:
                    _userData.SetState(person, ModeMain.Start);
                    break;
                case ModeMain.GetAllItem:
                    _userData.SetState(person, ModeMain.Start);
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
}

