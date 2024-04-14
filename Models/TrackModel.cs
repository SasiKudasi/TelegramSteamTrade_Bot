using LinqToDB.Mapping;
using System.ComponentModel.DataAnnotations.Schema;
using ColumnAttribute = LinqToDB.Mapping.ColumnAttribute;
using TableAttribute = LinqToDB.Mapping.TableAttribute;

namespace TelegramSteamTrade_Bot.Models
{
    [Table("tracks")]
    public class TrackModel
    {
        [PrimaryKey, Identity]
        [Column("id")]
        public int Id { get; set; }
        [Column("userid")]
        [ForeignKey("fk_userid")]
        public int UserId { get; set; }
        [Column("itemid")]
        [ForeignKey("fk_itemid")]
        public int ItemId { get; set; }
        [Column("lastactualprice")]
        public double LastActualPrice { get; set; }

        [Association(ThisKey = nameof(TrackModel.UserId), OtherKey = nameof(UserModel.Id))]
        public IEnumerable<UserModel> Users { get; set; } = new List<UserModel>();
        [Association(ThisKey = nameof(TrackModel.ItemId), OtherKey = nameof(ItemModel.Id))]
        public IEnumerable<ItemModel> Items { get; set; } = new List<ItemModel>();
    }
}
