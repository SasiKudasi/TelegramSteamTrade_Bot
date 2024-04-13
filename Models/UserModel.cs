using LinqToDB.Mapping;
using System.Runtime.Serialization;

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

        [Column(Name = "userstate"), DataType(LinqToDB.DataType.VarChar)]
        public Mode Mode { get; set; }
    }

    public enum Mode
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
}
