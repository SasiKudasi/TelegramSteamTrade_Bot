using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramSteamTrade_Bot.Models
{
    [Table("items")]
    public class ItemModel
    {
        [PrimaryKey, Identity]
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("price")]
        public double ItemPrice { get; set; }
        [Column("gameid")]
        public int GameId { get; set; }
    }
}
