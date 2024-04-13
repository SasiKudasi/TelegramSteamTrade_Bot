using LinqToDB;
using Telegram.Bot;
using TelegramSteamTrade_Bot.Models;
using static LinqToDB.Reflection.Methods.LinqToDB;
using Update = Telegram.Bot.Types.Update;

namespace TelegramSteamTrade_Bot.Data
{
    public class UsersData : BaseData
    {
        private DbContext _db = new();
        private SteamMethod _steam = new();

        private GamesData _gamesData = new();
        public bool GetUser(long person)
        {
            var user = _db.Users.FirstOrDefault(x => x.ChatId == person);

            if (user == null)
            {
                CreateNewUser(person);
            }
            return true;
        }

        private void CreateNewUser(long person)
        {
            var newUser = new UserModel()
            {
                ChatId = person,
                ModeMain = ModeMain.Start,
                ModeGame = ModeGame.Initial
            };
            _db.InsertWithIdentity(newUser);
        }
        public async Task SetStateAsync(ITelegramBotClient client, Update update, CancellationToken token)
        {
            var msg = update.Message.Text;
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
                    SetState(person, ModeMain.Start);
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
