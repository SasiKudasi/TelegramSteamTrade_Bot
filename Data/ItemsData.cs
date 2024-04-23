using LinqToDB;
using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramSteamTrade_Bot.Models;

namespace TelegramSteamTrade_Bot.Data
{
    public class ItemsData : BaseData, IWorkWhithEntity
    {
        private ITelegramBotClient _client;
        private CancellationToken _token;
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
        public async Task ItemMenuAsync(ITelegramBotClient client, string msg, long userChatId, CancellationToken token)
        {
            _client = client;
            _token = token;
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
                                replyMarkup: new ReplyKeyboardRemove(),
                        cancellationToken: _token);
                            break;
                        case "/dota2":
                            await SetState(userChatId, ModeGame.GetDotaItems);
                            await client.SendTextMessageAsync(userChatId,
                               "Пожалуйста, введите название предмета, цену которого хотите посмотреть" +
                               "\nприм. Totem of Deep Magma",
                               replyMarkup: new ReplyKeyboardRemove(),
                       cancellationToken: _token);
                            break;
                        case "/yes":
                            await _tracksData.AddItemAsync(client, userChatId, _token, user);
                            break;
                    }
                }
                else
                {

                    switch (gameMode)
                    {
                        case ModeGame.GetCSItems:
                            await GetItems(userChatId, msg, _gamesData.GetGameAppId("/cs2"));
                            break;
                        case ModeGame.GetDotaItems:
                            await GetItems(userChatId, msg, _gamesData.GetGameAppId("/dota2"));
                            break;
                    }
                }
            }
        }

        private async Task GetItems(long userChatId, string msg, int gameID)
        {
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
                await GetItemWhithSteamPrice(userChatId, items);
                await SetState(userChatId, items.Id);
                await SetState(userChatId, ModeGame.Initial);

            }
        }

        private async Task GetItemWhithSteamPrice(long userChatId, ItemModel? item)
        {
            if (item == null)
            {
                return;
            }
            var price = 0.0;
            price = await _steam.SearchItemPriceAsync(item.GameId, item.Name);
            if (_steam.ItemLowestPrice == 0.0)
            {
                await _client.SendTextMessageAsync(userChatId,
                    "Похоже такой предмет на торговой площадке Steam отсутствует.\n",
                    replyMarkup: Keyboards.InlineKeyboard(Keyboards.GoToStartBtn),
                    cancellationToken: _token);
                _db.Items.Delete(n => n.Name == item.Name);
                await SetState(userChatId, 0);
                _db.Close();
            }
            else
            {
                _db.Items.Where(p => p.Name == item.Name)
                 .Set(m => m.ItemPrice, price).Update();
                _db.Close();
                await _client.SendTextMessageAsync(userChatId,
                    $"Актуальная цена {item.Name} на данный момент составляет {price}\n" +
                    $"Хотите ли вы добавить этот предмет в отслеживаемые предметы?",
                    replyMarkup: Keyboards.KonfirmKeyboard(),
                    cancellationToken: _token);
            }
        }
    }
}
