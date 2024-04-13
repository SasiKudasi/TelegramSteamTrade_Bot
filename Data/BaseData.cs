using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
