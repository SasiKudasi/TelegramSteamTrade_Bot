using Telegram.Bot;
using Telegram.Bot.Polling;
using TelegramSteamTrade_Bot.Models;
using TelegramSteamTrade_Bot.Data;
using TelegramSteamTrade_Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;

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
            AllowedUpdates = new UpdateType[] { },
        };
        bot.StartReceiving(updateHandler: Handler, pollingErrorHandler: ErrorHandler, receiverOptions: receiver);
        Console.ReadLine();
    }
    private static async Task Handler(ITelegramBotClient client, Update update, CancellationToken token)
    {
        switch (update.Type)
        {
            case UpdateType.Message:
                await OnMessageText(client, update, token);
                break;
            case UpdateType.CallbackQuery:
                await OnMessageQuery(client, update, token);
                break;
        }
    }

    private static async Task OnMessageQuery(ITelegramBotClient client, Update update, CancellationToken token)
    {
        var user = await _userData.GetEntity<UserModel>(update.CallbackQuery.From.Id.ToString());
        await _userData.SwitchStateAsync(client, update.CallbackQuery.Data, update.CallbackQuery.From.Id, token);
        var userMode = await _userData.GetMode(user);       
        var init = new Initial(update.CallbackQuery.From.Id, update.CallbackQuery.Data, userMode.ModeMain);

        switch (init.State)
        {
            case ModeMain.Start:
                await SendMenu(client, init.UserChatId, token);
                break;
            case ModeMain.GetItem:
                await _itemsData.ItemMenuAsync(client, init.Message, init.UserChatId, token);
                await _userData.SetState(init.UserChatId, ModeMain.GetItem);
                break;
        }
    }

    private static async Task OnMessageText(ITelegramBotClient client, Telegram.Bot.Types.Update update, CancellationToken token)
    {
        var user = await _userData.GetEntity<UserModel>(update.Message.Chat.Id.ToString());
        await _userData.SwitchStateAsync(client, update.Message.Text, update.Message.Chat.Id, token);
        var userMode = await _userData.GetMode(user);
        var init = new Initial(update.Message.Chat.Id, update.Message.Text, userMode.ModeMain);

        switch (init.State)
        {
            case ModeMain.Start:
                await SendMenu(client, init.UserChatId, token);
                break;
            case ModeMain.GetItem:
                await _itemsData.ItemMenuAsync(client, update.Message.Text, init.UserChatId, token);
                await _userData.SetState(init.UserChatId, ModeMain.GetItem);
                break;
            case ModeMain.DeleteItem:
                await _tracksData.DeliteTrackingItem(user, client, update, token);
                await _userData.SetState(init.UserChatId, ModeMain.DeleteItem);
                break;
            case ModeMain.GetAllItem:
                await _tracksData.GetAllItemAsync(user, client, update, token);
                await _userData.SetState(init.UserChatId, ModeMain.Start);
                break;
        }

    }

    public static async Task SendMenu(ITelegramBotClient client, long userChatId, CancellationToken token)
    {
        await client.SendTextMessageAsync(userChatId,
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

