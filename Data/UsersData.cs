using LinqToDB;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramSteamTrade_Bot.Models;
using Update = Telegram.Bot.Types.Update;

namespace TelegramSteamTrade_Bot.Data
{
    public class UsersData : BaseData, IWorkWhithEntity
    {
        private GamesData _gamesData = new();
        private StateData _stateData = new();
        public async Task CreateNewEntity<T>(T entity) where T : class
        {
            await _db.InsertWithIdentityAsync(entity);

            var userModel = entity as UserModel;

            if (userModel != null)
            {
                userModel = await GetEntity<UserModel>(userModel.ChatId.ToString());
                await _stateData.CreateNewEntity(new StateModel()
                {
                    UserId = userModel.Id,
                    ModeMain = ModeMain.Start,
                    ModeGame = ModeGame.Initial,
                    LastItemState = 0
                });
            }
        }

        public async Task<T> GetEntity<T>(string name) where T : class
        {
            long person = 0;
            var id = long.TryParse(name, out person);
            if (id)
            {
                var user = await _db.Users.FirstOrDefaultAsync(x => x.ChatId == person) as T;

                if (user == null)
                {
                    await CreateNewEntity(new UserModel()
                    {
                        ChatId = person,
                    });
                    user = await _db.Users.FirstOrDefaultAsync(x => x.ChatId == person) as T;
                }
                _db.Close();
                return user!;
            }
            else
                return null;
        }

        public async Task SwitchStateAsync(ITelegramBotClient client, string msg, long person, CancellationToken token)
        {

            switch (msg)
            {
                case "Старт":
                    await SetState(person, ModeMain.Start);
                    await SetState(person, ModeGame.Initial);
                    break;
                case "Удалить предмет из списка":
                    await client.SendTextMessageAsync(person,
                        "Введите номер предмета, который хотите удалить",
                        cancellationToken: token);
                    await SetState(person, ModeMain.DeleteItem);
                    break;
                case "Посмотреть цены на предметы из личного списка":
                    await SetState(person, ModeMain.GetAllItem);
                    break;
                case "Посмотреть актуальную цену на предмет":
                    var game = new GamesData();

                    await client.SendTextMessageAsync(person,
                        "Выберите игру, предметы которой хотите посмотерть",
                        replyMarkup: Keyboards.Keyboard(GamesData.GameKeyboard),
                        cancellationToken: token);
                    await SetState(person, ModeMain.GetItem);
                    break;
            }
        }
        public static InlineKeyboardButton[][] GoToStartBtn()
        {
            var btns = new InlineKeyboardButton[1][];
            int i = 0;
            btns[0] = new[]
            {
                    InlineKeyboardButton.WithCallbackData("Старт", "Старт")
                };
            i++;

            return btns;
        }

    }
}
