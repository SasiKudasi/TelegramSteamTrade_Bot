using LinqToDB;
using SteamWebAPI2.Interfaces;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramSteamTrade_Bot.Models;

namespace TelegramSteamTrade_Bot.Data
{
    public class ItemsData : BaseData, IWorkWhithEntity
    {
        private SteamMethod _steam = new();
        private GamesData _gamesData = new();
        private TracksData _tracksData = new();
        private UsersData _userData = new();


        public async Task CreateNewEntity<T>(T entity) where T : class
        {
            await _db.InsertWithIdentityAsync(entity);
            _db.Close();
        }
        public async Task<T> GetEntity<T>(string name) where T : class
        {
            var item = await _db.Items.FirstOrDefaultAsync(item => item.Name == name) as T;           
            _db.Close();
            return item;
        }
        public async Task ItemMenuAsync(ITelegramBotClient client, Update update, CancellationToken token)
        {

            var msg = update.Message!.Text;
            var userChatId = update.Message!.Chat.Id;
            var user = await _userData.GetEntity<UserModel>(userChatId.ToString());
            var game = await GetMode(user);
            var gameMode = game.ModeGame;
            if (msg == "/check_item_price")
                return;
            else
            {
                if (gameMode == ModeGame.Initial)
                {
                    switch (msg)
                    {
                        case "/cs2":
                            await SetState(userChatId, ModeGame.GetCSItems);
                            await client.SendTextMessageAsync(userChatId,
                                "Пожалуйста, введите название предмета, цену которого хотите посмотреть" +
                                "\nприм. AK-47 | Redline (Minimal Wear)",
                        cancellationToken: token);
                            break;
                        case "/dota2":
                            await SetState(userChatId, ModeGame.GetDotaItems);
                            await client.SendTextMessageAsync(userChatId,
                               "Пожалуйста, введите название предмета, цену которого хотите посмотреть" +
                               "\nприм. Totem of Deep Magma",
                       cancellationToken: token);
                            break;
                        case "/yes":
                            await _tracksData.AddItemAsync(client, update, token, user);
                            break;
                    }
                }
                else
                {
                    switch (gameMode)
                    {
                        case ModeGame.GetCSItems:
                            await GetItems(client, update, token, _gamesData.GetGameAppId("/cs2"));
                            break;
                        case ModeGame.GetDotaItems:
                            await GetItems(client, update, token, _gamesData.GetGameAppId("/dota2"));
                            break;
                    }
                }
            }
        }

        private async Task GetItems(ITelegramBotClient client, Update update, CancellationToken token, int gameID)
        {
            var person = update.Message!.Chat.Id;
            var msg = update.Message!.Text;
            if (msg == "/cs2" || msg == "/dota2")
                return;
            else
            {
                var items = await GetEntity<ItemModel>(msg);
                if (items == null)
                {
                    await CreateNewEntity(new ItemModel
                    {
                        Name = msg,
                        GameId = gameID
                    });
                };

                items = await GetEntity<ItemModel>(msg);
                await GetItemWhithSteamPrice(client, update, token, items);
                await SetState(person, items.Id);
                await SetState(person, ModeGame.Initial);

            }
        }    

        private async Task GetItemWhithSteamPrice(ITelegramBotClient client, Update update, CancellationToken token, ItemModel? item)
        {
            if (item == null)
            {
                return;
            }
            var person = update.Message!.Chat.Id;
            var price = 0.0;
            price = await _steam.SearchItemPriceAsync(item.GameId, item.Name);
            if (_steam.ItemLowestPrice == 0.0)
            {
                await client.SendTextMessageAsync(person,
                    "Похоже такой предмет на торговой площадке Steam отсутствует.\nПопробуйте еще раз.",
                    cancellationToken: token);
                _db.Items.Delete(n => n.Name == item.Name);
                await SetState(person, ModeGame.Initial);
                await SetState(person, 0);
                _db.Close();
            }
            else
            {
                await client.SendTextMessageAsync(person,
                    $"Актуальная цена {item.Name} на данный момент составляет {price}\n" +
                    $"Хотите ли вы добавить этот предмет в отслеживаемые предметы?\n" +
                    $"/yes - что бы добавить данный предмет в отслеживаемые.\n" +
                    $"/start - что бы вернуться в меню."
                    , cancellationToken: token);
                _db.Items.Where(p => p.Name == item.Name)
              .Set(m => m.ItemPrice, price).Update();
                _db.Close();
            }

        }
    }
}
