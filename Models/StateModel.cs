using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramSteamTrade_Bot.Models
{
    [Table("state")]
    public class StateModel
    {
        [PrimaryKey, Identity]
        [Column("id")]
        public int Id { get; set; }
        [Column("userid")]
        public int UserId { get; set; }        

        [Column(Name = "modemain"), DataType(LinqToDB.DataType.VarChar)]
        public ModeMain ModeMain { get; set; }
        [Column(Name = "modegame"), DataType(LinqToDB.DataType.VarChar)]
        public ModeGame ModeGame { get; set; }
        [Column("lastitem")]
        public int LastItemState { get; set;}        
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
