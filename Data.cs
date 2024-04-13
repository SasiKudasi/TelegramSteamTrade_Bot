using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramSteamTrade_Bot.Models;

namespace TelegramSteamTrade_Bot
{
    public class Data
    {
        private static DbContext _db = new DbContext();

        internal static Mode GetMode(long person)
        {
            var mode = _db.Users.FirstOrDefault(u => u.ChatId == person);           
            return mode.Mode;
        }

        public static bool GetUser(long person)
        {
            var user = _db.Users.FirstOrDefault(x => x.Id == person);
            Console.WriteLine(user);
            if (user == null)
            {
                CreateNewUser(person);
            }
            return true;

        }

        private static void CreateNewUser(long person)
        {
            var newUser = new UserModel()
            {
                ChatId = person,
                Mode = Mode.Start
            };
            _db.InsertWithIdentity(newUser);
        }

        public static void SetState(long person, Mode mode)
        {
            _db.Users.Where(p=>p.ChatId ==person)
                .Set(m=>m.Mode, mode).Update();
        }
    }
}
