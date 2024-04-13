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
        private DbContext _db = new();
        private GamesData _gamesData = new();
        private void CreateNewItem(int v, string? msg)
        {
            var item = new ItemModel()
            {
                Name = msg!,
                GameId = v,
            };
            _db.InsertWithIdentity(item);
        }
        public async Task ItemMenuAsync(ITelegramBotClient client, Update update, CancellationToken token)
        {
            var gameMode = GetModeGame(update.Message.Chat.Id);
            var msg = update.Message!.Text;
            if (msg == "/check_item_price")
                return;
            else
            {
                if (gameMode == ModeGame.Initial)
                {
                    switch (msg)
                    {
                        case "/cs2":
                            SetState(update.Message.Chat.Id, ModeGame.GetCSItems);
                            break;
                        case "/dota2":
                            SetState(update.Message.Chat.Id, ModeGame.GetDotaItems);
                            break;
                    }
                    await client.SendTextMessageAsync(update.Message!.Chat.Id, "Пожалуйста, введите название предмета, цену которого хотите посмотреть" +
                        "\nприм. AK-47 | Redline (Minimal Wear)", cancellationToken: token);
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

        private async void GetItems(ITelegramBotClient client, Update update, CancellationToken token, int v)
        {
            var msg = update.Message!.Text;
            if (msg == "/cs2" || msg == "/dota2")
                return;
            else
            {
                var price = 0.0;
                var items = _db.Items.FirstOrDefault(n => n.Name == msg);
                if (items == null)
                {
                    CreateNewItem(v, msg);
                    price = await CheckItem(client, update, token, v);
                    SetState(update.Message.Chat.Id, ModeGame.Initial);
                }
                else
                {
                    price = await CheckItem(client, update, token, v);
                    SetState(update.Message.Chat.Id, ModeGame.Initial);
                }
            }
        }

        private async Task<double> CheckItem(ITelegramBotClient client, Update update, CancellationToken token, int v)
        {
            var msg = update.Message!.Text;
            var price = 0.0;
            price = await _steam.SearchItemPriceAsync(v, msg!);
            if (_steam.ItemLowestPrice == 0.0)
            {
                await client.SendTextMessageAsync(update.Message!.Chat.Id, "Похоже такого предмета не существует", cancellationToken: token);
                _db.Items.Delete(n => n.Name == msg);
            }
            else
            {
                await client.SendTextMessageAsync(update.Message!.Chat.Id, $"Актуальная цена {msg} на данный момент составляет {price}", cancellationToken: token);
                _db.Items.Where(p => p.Name == msg)
              .Set(m => m.ItemPrice, price).Update();
            }
            return price;
        }
    }
}
