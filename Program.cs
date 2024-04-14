using Telegram.Bot;
using Telegram.Bot.Polling;
using TelegramSteamTrade_Bot.Models;
using TelegramSteamTrade_Bot.Data;

class Program
{
    private static UsersData _userData = new UsersData();
    private static ItemsData _itemsData = new ItemsData();
    private static TracksData _tracksData = new TracksData();
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
        if (_userData.GetUser(person) == null)
        {
            await client.SendTextMessageAsync(update.Message!.Chat.Id, "Произошла непредвиденная ошибка, пожалуйста попробуйте позднее", cancellationToken: token);
        }
        else
        {
            await _userData.SetStateAsync(client, update, token);
            var mode = _userData.GetModeMain(person);
            switch (mode)
            {
                case ModeMain.Start:
                    await _userData.SendMenu(client, update, token);
                    break;
                case ModeMain.GetItem:
                    await _itemsData.ItemMenuAsync(client, update, token);
                    _userData.SetState(person, ModeMain.GetItem);
                    break;
                case ModeMain.AddItem:
                    _tracksData.AddItem(client, update, token);
                    _userData.SetState(person, ModeMain.Start);
                    break;
                case ModeMain.GetAllItem:
                   await _tracksData.GetAllItemAsync(_userData.GetUser(person), client, update, token);
                    break;
            }
        }
    }



    private static Task ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}

