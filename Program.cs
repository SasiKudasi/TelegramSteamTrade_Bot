using Telegram.Bot;
using Telegram.Bot.Polling;
using TelegramSteamTrade_Bot.Models;
using TelegramSteamTrade_Bot.Data;
using TelegramSteamTrade_Bot;
using Telegram.Bot.Types.Enums;

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
        
        switch (update.Type)
        {
            case UpdateType.Message:
                await OnMessageText(client, update, token);
                break;
            case UpdateType.CallbackQuery:
                var userId = update.CallbackQuery.From.Id;
                var msg = update.CallbackQuery.Data;
                var user = await _userData.GetEntity<UserModel>(userId.ToString());
                await _itemsData.ItemMenuAsync(client, msg, userId, token);
                break;
        }       
    }

    private static async Task OnMessageText(ITelegramBotClient client, Telegram.Bot.Types.Update update, CancellationToken token)
    {
        long userChatId = update.Message.Chat.Id;

        var user = await _userData.GetEntity<UserModel>(userChatId.ToString());
        if (user == null)
        {
            await client.SendTextMessageAsync(update.Message!.Chat.Id, "Произошла непредвиденная ошибка, пожалуйста попробуйте позднее", cancellationToken: token);
        }
        else
        {
            await _userData.SwitchStateAsync(client, update, token);
            var userMode = await _userData.GetMode(user);
            var mode = userMode.ModeMain;
            switch (mode)
            {
                case ModeMain.Start:
                    await SendMenu(client, update, token);
                    break;
                case ModeMain.GetItem:
                    await _itemsData.ItemMenuAsync(client, update.Message.Text, userChatId, token);
                    await _userData.SetState(userChatId, ModeMain.GetItem);
                    break;
                case ModeMain.DeleteItem:
                    await _tracksData.DeliteTrackingItem(user, client, update, token);
                    await _userData.SetState(userChatId, ModeMain.DeleteItem);
                    break;
                case ModeMain.GetAllItem:
                    await _tracksData.GetAllItemAsync(user, client, update, token);
                    await _userData.SetState(userChatId, ModeMain.Start);
                    break;
            }
        }
    }

    public static async Task SendMenu(ITelegramBotClient client, Telegram.Bot.Types.Update update, CancellationToken token)
    {
        await client.SendTextMessageAsync(update.Message!.Chat.Id,
            "Привет, я Бот, который поможет тебе с отслеживанием цен на внутриигровые предметы.\n" +
            "В данный момент я нахожусь в разработке, но я уже кое что умею!\n" +
            "Пожалуйста, выберите действие",
            replyMarkup: Keyboards.MainKeyboard(),
            cancellationToken: token);
    }

    private static Task ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        Console.WriteLine($"{exception.Message}");
        return Task.CompletedTask;
    }
}

