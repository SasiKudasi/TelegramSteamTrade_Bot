using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using TelegramSteamTrade_Bot.Models;

namespace TelegramSteamTrade_Bot.Data
{
    public class BaseData
    {
        private DbContext _db = new();

        public ModeMain GetModeMain(long person)
        {
            var mode = _db.Users.FirstOrDefault(u => u.ChatId == person);
            return mode.ModeMain;
        }
        public ModeGame GetModeGame(long person)
        {
            var mode = _db.Users.FirstOrDefault(u => u.ChatId == person);
            return mode.ModeGame;
        }
        public void SetState(long person, Enum mode)
        {
            if (mode.GetType() == typeof(ModeMain))
            {
                _db.Users.Where(p => p.ChatId == person)
               .Set(m => m.ModeMain, mode).Update();
            }
            if (mode.GetType() == typeof(ModeGame))
            {
                _db.Users.Where(p => p.ChatId == person)
              .Set(m => m.ModeGame, mode).Update();
            }
        }

        public async Task SendMenu(ITelegramBotClient client, Telegram.Bot.Types.Update update, CancellationToken token)
        {
            await client.SendTextMessageAsync(update.Message!.Chat.Id, "Привет, я Бот, который поможет тебе с отслеживанием цен на внутриигровые предметы.\n" +
                "/check_item_price - если ты просто хочешь посмотреть цену на определенный предмет.\n" +
                "/add_item_to_track - если ты хочешь отслеживать его цену.\n" +
                "/check_tracking_item - если ты хочешь посмотреть актуальные цены на все предметы, что ты добавил.\n" +
                "/start - для возврата в главное меню.", cancellationToken: token);
        }
    }
}
