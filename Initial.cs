using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramSteamTrade_Bot.Data;
using TelegramSteamTrade_Bot.Models;

namespace TelegramSteamTrade_Bot
{
    internal class Initial
    {      
        public long UserChatId { get; set; }
        public string Message { get; set; }
        public ModeMain State { get; set; }
        public  Initial(long userId, string msg, ModeMain mode)
        {
            UserChatId = userId;
            Message = msg;
            State = mode;           
        }
    }
}
