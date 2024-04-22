using LinqToDB.Mapping;

namespace TelegramSteamTrade_Bot.Models
{
    [Table("users")]
    public class UserModel
    {
        [PrimaryKey, Identity]
        [Column("id")]
        public int Id { get; set; }
        [Column("chatid")]
        public long ChatId { get; set; }        
    }

   
}
