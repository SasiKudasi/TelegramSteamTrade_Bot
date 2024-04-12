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
        [Column("itemid")]
        public int ItemId { get; set; }
        [Column("gameid")]
        public int GameId { get; set; }
        [Column("oldprice")]
        public double OldPrice { get; set; }
        [Column("userstate")]
        public Mode Mode { get; set; }
    }

    public enum Mode
    {
        Start = 1,
        ChouseGame = 2,
        AddItem = 3,
        GetItem = 4,
        GetAllItem = 5
    }
}
