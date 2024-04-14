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
        internal void AddItem(ITelegramBotClient client, Update update, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        internal async Task GetAllItemAsync(UserModel userModel, ITelegramBotClient client, Update update, CancellationToken token)
        {

            var userItems = from t in _db.Track
                            join u in _db.Users
                            on t.UserId equals u.Id
                            join i in _db.Items
                            on t.ItemId equals i.Id
                            where u.ChatId == userModel.ChatId
                            select new { t.Id, i.Name, i.ItemPrice, u.ChatId};
          foreach (var item in userItems)
            {
                await client.SendTextMessageAsync(update.Message.Chat.Id,
                   $"track id {item.Id} itemname {item.Name} price {item.ItemPrice}, mychat {item.ChatId}\n",
                   cancellationToken: token);
            }
        }

        public void NewTrack(ItemModel? items, UserModel? userModel)
        {
            var track = new TrackModel()
            {
                ItemId = items.Id,
                UserId = userModel.Id,
                LastActualPrice = items.ItemPrice
            };
             _db.Insert(track);
        }
    }
}
