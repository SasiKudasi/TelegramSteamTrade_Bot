using LinqToDB;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramSteamTrade_Bot.Models;
using static LinqToDB.SqlQuery.SqlPredicate;

namespace TelegramSteamTrade_Bot.Data
{
    internal class TracksData : BaseData
    {
        private SteamMethod _steam = new();
        public async Task AddItemAsync(ITelegramBotClient client, Update update, CancellationToken token, UserModel user)
        {

            var userParams = GetMode(user);
            var item = _db.Items.FirstOrDefault(n => n.Id == userParams.LastItemState);
            if (GetTrack(user, userParams.LastItemState))
            {
                var track = new TrackModel()
                {
                    UserId = userParams.UserId,
                    ItemId = userParams.LastItemState,
                    LastActualPrice = item.ItemPrice
                };
                _db.Insert(track);
                await client.SendTextMessageAsync(update.Message!.Chat.Id,
                $"Предмет успешно добавлен.",
                cancellationToken: token);
                SetState(user.ChatId, 0);
            }
            else
            {
                await client.SendTextMessageAsync(update.Message!.Chat.Id,
                $"Вы уже отслеживаете данный предмет.",
                cancellationToken: token);
                SetState(user.ChatId, 0);
            }
        }

        private bool GetTrack(UserModel user, int lastItemState)
        {
            var track = _db.Track.Where(u => u.UserId == user.Id).
                FirstOrDefault(x => x.ItemId == lastItemState);
            if (track == null)
                return true;
            else
                return false;
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
                            select new
                            {
                                t.Id,
                                t.LastActualPrice,
                                i.Name,
                                i.ItemPrice,
                                i.GameId,
                                u.ChatId
                            };
            foreach (var item in userItems)
            {
                price = await _steam.SearchItemPriceAsync(item.GameId, item.Name);
                distinct = PercentProfit(item.LastActualPrice, price);
                await client.SendTextMessageAsync(update.Message!.Chat.Id,
                   $"track id {item.Id} itemname {item.Name} lastprice {item.LastActualPrice}, actualprice {price}, изменение в цене {distinct}%",
                   cancellationToken: token);
            }
        }
        double PercentProfit(double lastPrice, double actualPrice)
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
