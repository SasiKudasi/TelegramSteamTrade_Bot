using LinqToDB.Mapping;


namespace TelegramSteamTrade_Bot.Models
{
    [Table("games")]
    public class GameModel
    {
        [PrimaryKey, Identity]
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("appid")]
        public int AppId { get; set; }
    }
}
