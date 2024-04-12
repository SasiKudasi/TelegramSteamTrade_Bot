using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramSteamTrade_Bot.Models
{
    public class ItemModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double ItemPrice { get; set; }

        public int GameId { get; set; }
    }
}
