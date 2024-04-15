using Telegram.Bot;
using Telegram.Bot.Polling;
using TelegramSteamTrade_Bot.Models;
using TelegramSteamTrade_Bot.Data;

class Program
{
    private static UsersData _userData = new UsersData();
    private static ItemsData _itemsData = new ItemsData();
    private static TracksData _tracksData = new TracksData();
    static void Main(string[] args)
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
        var userChatId = update.Message!.Chat.Id;
        var user = _userData.GetUser(userChatId);
        if (user == null)
        {
            await client.SendTextMessageAsync(update.Message!.Chat.Id, "Произошла непредвиденная ошибка, пожалуйста попробуйте позднее", cancellationToken: token);
        }
        else
        {
           
            await _userData.SwitchStateAsync(client, update, token);
            var mode = _userData.GetMode(user).ModeMain;
            switch (mode)
            {
                case ModeMain.Start:
                    await _userData.SendMenu(client, update, token);
                    break;
                case ModeMain.GetItem:
                    await _itemsData.ItemMenuAsync(client, update, token);
                    _userData.SetState(userChatId, ModeMain.GetItem);
                    break;
                case ModeMain.AddItem:
                    _tracksData.AddItem(client, update, token);
                    _userData.SetState(userChatId, ModeMain.Start);
                    break;
                case ModeMain.GetAllItem:
                   await _tracksData.GetAllItemAsync(_userData.GetUser(userChatId), client, update, token);
                    break;
            }
        }
    }



    private static Task ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        Console.WriteLine($"{exception.Message}");
        return Task.CompletedTask;
    }
}

