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
    internal class TracksData : BaseData, IWorkerWhithEntity
    {
        private SteamMethod _steam = new();
        public async Task CreateNewEntity<T>(T entity) where T : class
        {
            await _db.InsertWithIdentityAsync(entity);
        }
        public Task<T> GetEntity<T>(string name) where T : class
        {
            throw new NotImplementedException();
        }
        public async Task AddItemAsync(ITelegramBotClient client, Update update, CancellationToken token, UserModel user)
        {
            var userParams = await GetMode(user);
            var item = await _db.Items.FirstOrDefaultAsync(n => n.Id == userParams.LastItemState);
            if (GetTrack(user, userParams.LastItemState))
            {
                await CreateNewEntity(new TrackModel()
                {
                    UserId = userParams.UserId,
                    ItemId = userParams.LastItemState,
                    LastActualPrice = item.ItemPrice
                });
                await client.SendTextMessageAsync(update.Message!.Chat.Id,
                $"Предмет успешно добавлен.",
                cancellationToken: token);
                await SetState(user.ChatId, 0);
                await SetState(user.ChatId, ModeGame.Initial);
                await SetState(user.ChatId, ModeMain.Start);
            }
            else
            {
                await client.SendTextMessageAsync(update.Message!.Chat.Id,
                $"Вы уже отслеживаете данный предмет.",
                cancellationToken: token);
                await SetState(user.ChatId, 0);
                await SetState(user.ChatId, ModeGame.Initial);
                await SetState(user.ChatId, ModeMain.Start);
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
                   $"Предмет {item.Name}:\n" +
                   $"Цена на момент добавления: {item.LastActualPrice},\n" +
                   $"Актуальная цена: {price}\n" +
                   $"Изменение в цене: {distinct}%\n",
                   cancellationToken: token);
            }
        }
        double PercentProfit(double lastPrice, double actualPrice)
        {
            var result = ((actualPrice - lastPrice) / lastPrice) * 100;
            return Math.Round(result, 3);
        }
    }
}
