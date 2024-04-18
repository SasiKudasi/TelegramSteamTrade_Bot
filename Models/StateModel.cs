using LinqToDB.Mapping;
using System.ComponentModel.DataAnnotations.Schema;
using ColumnAttribute = LinqToDB.Mapping.ColumnAttribute;
using TableAttribute = LinqToDB.Mapping.TableAttribute;

namespace TelegramSteamTrade_Bot.Models
{
    [Table("state")]
    public class StateModel
    {
        [PrimaryKey, Identity]
        [Column("id")]
        public int Id { get; set; }
        [ForeignKey("fk_userid")]
        [Column("userid")]
        public int UserId { get; set; }

        [Column(Name = "modemain"), DataType(LinqToDB.DataType.VarChar)]
        public ModeMain ModeMain { get; set; }
        [Column(Name = "modegame"), DataType(LinqToDB.DataType.VarChar)]
        public ModeGame ModeGame { get; set; }
        [Column("lastitem")]
        public int LastItemState { get; set; }
        [Association(ThisKey = nameof(StateModel.UserId), OtherKey = nameof(UserModel.Id))]
        public IEnumerable<UserModel> Users { get; set; } = new List<UserModel>();
    }
    public enum ModeMain
    {
        [MapValue(Value = "Start")]
        Start,
        [MapValue(Value = "ChouseGame")]
        ChouseGame,
        [MapValue(Value = "DeleteItem")]
        DeleteItem,
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
