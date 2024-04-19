using LinqToDB;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramSteamTrade_Bot.Models;

namespace TelegramSteamTrade_Bot.Data
{
    internal class TracksData : BaseData, IWorkWhithEntity
    {
        private SteamMethod _steam = new();
        public async Task CreateNewEntity<T>(T entity) where T : class
        {
            await _db.InsertWithIdentityAsync(entity);
        }
        public async Task<T> GetEntity<T>(string name) where T : class
        {
            long id = 0;
            if (ParsStringIntoLong(name, out id))
            {
                var personTracks = await _db.Track.Where(x => x.UserId == id).ToListAsync();
                return personTracks as T;
            }
            return null;
        }
        private async Task<bool> CheckItemInTracking(UserModel user, int lastItemState)
        {
            var allTrackingItems = await GetEntity<List<TrackModel>>(user.Id.ToString());
            var res = false;
            allTrackingItems.ForEach(x =>
            {
                if (x.ItemId == lastItemState)
                    res = true;
            });
            return res;
        }
        public async Task AddItemAsync(ITelegramBotClient client, Update update, CancellationToken token, UserModel user)
        {
            var userParams = await GetMode(user);
            var item = await _db.Items.FirstOrDefaultAsync(n => n.Id == userParams.LastItemState);
            if (!await CheckItemInTracking(user, userParams.LastItemState))
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
            int itemNumber = 0;
            var total = 0.0;
            foreach (var item in userItems)
            {
                price = await _steam.SearchItemPriceAsync(item.GameId, item.Name);
                distinct = PercentProfit(item.LastActualPrice, price);
                await client.SendTextMessageAsync(update.Message!.Chat.Id,
                   $"{itemNumber}. Предмет {item.Name}:\n" +
                   $"Цена на момент добавления: {item.LastActualPrice},\n" +
                   $"Актуальная цена: {price}\n" +
                   $"Изменение в цене: {distinct}%\n",
                   cancellationToken: token);
                itemNumber++;
                total += distinct;
            }
            await client.SendTextMessageAsync(update.Message!.Chat.Id,
                  $"Ваш инвентарь изменился на {Math.Round(total, 3)}%",
                  cancellationToken: token);
        }
        double PercentProfit(double lastPrice, double actualPrice)
        {
            var result = ((actualPrice - lastPrice) / lastPrice) * 100;
            return Math.Round(result, 3);
        }
        public async Task DeliteTrackingItem(UserModel user, ITelegramBotClient client, Update update, CancellationToken token)
        {
            var msg = update.Message.Text;

            if (msg == "/delete_tracking_item")
                return;
            int id = 0;
            var pars = int.TryParse(msg, out id);
            if (pars)
            {
                var allTrackingItems = await GetEntity<List<TrackModel>>(user.Id.ToString());
                if (id >= allTrackingItems.Count)
                {
                    await client.SendTextMessageAsync(update.Message!.Chat.Id,
                     "Вы ввели значение которое превышает колличество ваших отслеживаемых предметов",
                      cancellationToken: token);
                }
                else
                {
                    var item = allTrackingItems[id];
                    if (item != null)
                    {
                        await _db.DeleteAsync(item);
                        await client.SendTextMessageAsync(update.Message!.Chat.Id,
                         $"Предмет был успешно удален из вашего списка отслеживаемых предметов.",
                         cancellationToken: token);
                    }
                    else
                    {
                        await client.SendTextMessageAsync(update.Message!.Chat.Id,
                         "Такого предмета нет в вашем списке остлеживаемых предметов.",
                          cancellationToken: token);
                    }
                }
            }
            else
                await client.SendTextMessageAsync(update.Message!.Chat.Id,
                     "Вы ввели что то некорректное.",
                      cancellationToken: token);

        }
    }
}
