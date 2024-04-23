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
