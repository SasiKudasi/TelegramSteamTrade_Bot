using LinqToDB;
using Telegram.Bot;
using TelegramSteamTrade_Bot.Models;
using static LinqToDB.Reflection.Methods.LinqToDB;
using Update = Telegram.Bot.Types.Update;

namespace TelegramSteamTrade_Bot.Data
{
    public class UsersData : BaseData
    {
        
        private GamesData _gamesData = new();
        private StateData _stateData = new();
        public UserModel GetUser(long person)
        {
            var user = _db.Users.FirstOrDefault(x => x.ChatId == person);

            if (user == null)
            {
                user = CreateNewUser(person);
            }
            return user!;
        }

        private UserModel CreateNewUser(long person)
        {
            var newUser = new UserModel()
            {
                ChatId = person,
            };
            _db.InsertWithIdentity(newUser);
            _stateData.CreateStateForNewUser(newUser);
            return newUser;
        }
        public async Task SwitchStateAsync(ITelegramBotClient client, Update update, CancellationToken token)
        {
            var msg = update.Message!.Text;
            long person = update.Message.Chat.Id;
            switch (msg)
            {

                case "/start":
                    SetState(person, ModeMain.Start);
                    break;
                case "/add_item_to_track":
                    SetState(person, ModeMain.Start);
                    break;
                case "/check_tracking_item":
                    SetState(person, ModeMain.GetAllItem);
                    break;
                case "/check_item_price":
                    await client.SendTextMessageAsync(update.Message!.Chat.Id, "Выберите игру, предметы которой хотите посмотерть", cancellationToken: token);
                    await _gamesData.GetAllGamesName(client, update, token);
                    SetState(person, ModeMain.GetItem);
                    break;

            }
        }
    }
}
