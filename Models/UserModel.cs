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

        [Column(Name = "userstatemain"), DataType(LinqToDB.DataType.VarChar)]
        public ModeMain ModeMain { get; set; }
        [Column(Name = "userstategame"), DataType(LinqToDB.DataType.VarChar)]
        public ModeGame ModeGame { get; set; }
    }

    public enum ModeMain
    {
        [MapValue(Value = "Start")]
        Start,
        [MapValue(Value = "ChouseGame")]
        ChouseGame,
        [MapValue(Value = "AddItem")]
        AddItem,
        [MapValue(Value = "GetItem")]
        GetItem,
        [MapValue(Value = "GetAllItem")]
        GetAllItem
    }

    public enum ModeGame
    {
        [MapValue(Value = "Initial")]
        Initial,
        [MapValue(Value = "GetCSItems")]
        GetCSItems,
        [MapValue(Value = "GetDotaItems")]
        GetDotaItems
    }
}
