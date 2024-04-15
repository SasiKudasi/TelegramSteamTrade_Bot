using LinqToDB;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramSteamTrade_Bot.Models;

namespace TelegramSteamTrade_Bot.Data
{
    public class ItemsData : BaseData
    {
        private SteamMethod _steam = new();
        private GamesData _gamesData = new();
        private TracksData _tracksData = new();
        private UsersData _userData = new();
        private async Task<ItemModel> CreateNewItem(int v, string? msg)
        {
            var item = new ItemModel()
            {
                Name = msg!,
                GameId = v,
            };
           await _db.InsertWithIdentityAsync(item);
            item = await _db.Items.FirstOrDefaultAsync(item => item.Name == msg);
            return item;
        }
        public async Task ItemMenuAsync(ITelegramBotClient client, Update update, CancellationToken token)
        {

            var msg = update.Message!.Text;
            var userChatId = update.Message!.Chat.Id;
            var user = _userData.GetUser(userChatId);
            var gameMode = GetMode(user).ModeGame;
            if (msg == "/check_item_price")
                return;
            else
            {
                if (gameMode == ModeGame.Initial)
                {
                    switch (msg)
                    {
                        case "/cs2":
                            SetState(userChatId, ModeGame.GetCSItems);
                            await client.SendTextMessageAsync(userChatId,
                                "Пожалуйста, введите название предмета, цену которого хотите посмотреть" +
                                "\nприм. AK-47 | Redline (Minimal Wear)",
                        cancellationToken: token);
                            break;
                        case "/dota2":
                            SetState(userChatId, ModeGame.GetDotaItems);
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
                            GetItems(client, update, token, _gamesData.GetGameAppId("/cs2"));
                            break;
                        case ModeGame.GetDotaItems:
                            GetItems(client, update, token, _gamesData.GetGameAppId("/dota2"));
                            break;
                    }
                }
            }
        }

        private async void GetItems(ITelegramBotClient client, Update update, CancellationToken token, int gameID)
        {
            var person = update.Message!.Chat.Id;
            var msg = update.Message!.Text;
            if (msg == "/cs2" || msg == "/dota2")
                return;
            else
            {
                var items = _db.Items.FirstOrDefault(n => n.Name == msg);
                items = await IsItemExists(client, update, gameID, items, token);
                await CheckItem(client, update, token, gameID);
                SetState(person, items.Id);
                SetState(person, ModeGame.Initial);
            }
        }

        private async Task<ItemModel?> IsItemExists(ITelegramBotClient client, Update update, int gameId, ItemModel? items, CancellationToken token)
        {
            var person = update.Message!.Chat.Id;
            var msg = update.Message!.Text;
            if (items == null)
            {
                items = await CreateNewItem(gameId, msg);
            }
            return items;
        }

        private async Task<bool> CheckItem(ITelegramBotClient client, Update update, CancellationToken token, int v)
        {
            var person = update.Message!.Chat.Id;
            var msg = update.Message!.Text;
            var price = 0.0;
            price = await _steam.SearchItemPriceAsync(v, msg!);
            if (_steam.ItemLowestPrice == 0.0)
            {
                await client.SendTextMessageAsync(person,
                    "Похоже такой предмет на торговой площадке Steam отсутствует.\nПопробуйте еще раз.",
                    cancellationToken: token);
                _db.Items.Delete(n => n.Name == msg);
                return false;
            }
            else
            {
                await client.SendTextMessageAsync(person,
                    $"Актуальная цена {msg} на данный момент составляет {price}\n" +
                    $"Хотите ли вы добавить этот предмет в отслеживаемые предметы?\n" +
                    $"/yes - что бы добавить данный предмет в отслеживаемые.\n" +
                    $"/start - что бы вернуться в меню."
                    , cancellationToken: token);
                _db.Items.Where(p => p.Name == msg)
              .Set(m => m.ItemPrice, price).Update();
                return true;
            }

        }
    }
}
