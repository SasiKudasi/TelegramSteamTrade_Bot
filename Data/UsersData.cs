using LinqToDB;
using System;
using Telegram.Bot;
using TelegramSteamTrade_Bot.Models;
using static LinqToDB.Reflection.Methods.LinqToDB;
using static LinqToDB.Reflection.Methods.LinqToDB.Insert;
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
                }
                return user!;
            }
            else
                return null;
        }

        public async Task SwitchStateAsync(ITelegramBotClient client, Update update, CancellationToken token)
        {
            var msg = update.Message!.Text;
            long person = update.Message.Chat.Id;
            switch (msg)
            {
                case "/start":
                    await SetState(person, ModeMain.Start);
                    break;
                case "/delete_tracking_item":
                    await client.SendTextMessageAsync(update.Message!.Chat.Id, "Введите номер предмета, который хотите удалить", cancellationToken: token);
                    await SetState(person, ModeMain.DeleteItem);
                    break;
                case "/check_tracking_items":
                    await SetState(person, ModeMain.GetAllItem);
                    break;
                case "/check_item_price":
                    await client.SendTextMessageAsync(update.Message!.Chat.Id, "Выберите игру, предметы которой хотите посмотерть", cancellationToken: token);
                    await _gamesData.GetAllGamesName(client, update, token);
                    await SetState(person, ModeMain.GetItem);
                    break;
            }
        }


    }
}
