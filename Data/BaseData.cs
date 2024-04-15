using LinqToDB;
using LinqToDB.Data;
using Telegram.Bot;
using TelegramSteamTrade_Bot.Models;
using static LinqToDB.DataProvider.Access.AccessHints;
using static LinqToDB.DataProvider.ClickHouse.ClickHouseHints;

namespace TelegramSteamTrade_Bot.Data
{
    public class BaseData
    {
        private DbContext _db = new();


        public void SetState(long person, object mode)
        {
            var userData = new UsersData();
            var user = userData.GetUser(person);

            if (mode is (ModeMain))
            {
                var modeMain = (ModeMain)mode;
                var a = _db.State.Where(u => u.UserId == user.Id).
                     Set(m => m.ModeMain, modeMain).
                     Update();

            }
            if (mode is (ModeGame))
            {
                var modeGame = (ModeGame)mode;
                _db.State.Where(u => u.UserId == user.Id).
                    Set(m => m.ModeGame, modeGame).
                    Update();
            }
            if (mode is (int))
            {
                var lastItem = (int)mode;
                _db.State.Where(u => u.UserId == user.Id).
                    Set(m => m.LastItemState, lastItem).
                    Update();
            }
        }
        public StateModel GetMode(UserModel user)
        {
            var userState = _db.State.FirstOrDefault(x => x.UserId == user.Id);
            if (userState == null)
            {
                var state = new StateData();
                userState = state.CreateStateForNewUser(user);
            }
            return userState!;
        }
        public async Task SendMenu(ITelegramBotClient client, Telegram.Bot.Types.Update update, CancellationToken token)
        {
            await client.SendTextMessageAsync(update.Message!.Chat.Id, "Привет, я Бот, который поможет тебе с отслеживанием цен на внутриигровые предметы.\n" +
                "/check_item_price - если ты просто хочешь посмотреть цену на определенный предмет.\n" +
                "/add_item_to_track - если ты хочешь отслеживать его цену.\n" +
                "/check_tracking_item - если ты хочешь посмотреть актуальные цены на все предметы, что ты добавил.\n" +
                "/start - для возврата в главное меню.", cancellationToken: token);
        }
    }
}
