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
            throw new NotImplementedException();
        }

        internal static void SetState(long person, Mode start)
        {
            throw new NotImplementedException();
        }
    }
}
