using LinqToDB;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramSteamTrade_Bot.Models;

namespace TelegramSteamTrade_Bot.Data
{
    internal class TracksData
    {

        private DbContext _db = new();
        private SteamMethod _steam = new();
        internal void AddItem(ITelegramBotClient client, Update update, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        internal async Task GetAllItemAsync(UserModel userModel, ITelegramBotClient client, Update update, CancellationToken token)
        {
            double price = 0.0;
            double distinct = 0.0;
            var userItems = from t in _db.Track
                            join u in _db.Users
                            on t.UserId equals u.Id
                            join i in _db.Items
                            on t.ItemId equals i.Id
                            where u.ChatId == userModel.ChatId
                            select new { t.Id, i.Name, i.ItemPrice, i.GameId, t.LastActualPrice, u.ChatId};
          foreach (var item in userItems)
            {
                price=  await _steam.SearchItemPriceAsync(item.GameId, item.Name);
                distinct = PercentProfit(item.LastActualPrice, price);
                await client.SendTextMessageAsync(update.Message!.Chat.Id,
                   $"track id {item.Id} itemname {item.Name} lastprice {item.LastActualPrice}, actualprice {price}, изменение в цене {distinct}%",
                   cancellationToken: token);
            }
        }
        double PercentProfit (double lastPrice, double actualPrice)
        {
            var result = ((actualPrice - lastPrice) / lastPrice) * 100;
            return Math.Round(result, 3);
        }
        public void NewTrack(ItemModel? items, UserModel? userModel)
        {
            var track = new TrackModel()
            {
                ItemId = items!.Id,
                UserId = userModel!.Id,
                LastActualPrice = items.ItemPrice
            };
             _db.Insert(track);
        }
    }
}
