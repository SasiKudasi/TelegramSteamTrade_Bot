using LinqToDB.Data;
using LinqToDB;

namespace TelegramSteamTrade_Bot.Models
{
    public class DbContext : DataConnection
    {
       static string Connectionstring()
        {
            var connectionString = File.ReadAllText("connectionstring.txt");
            return connectionString;
        }
        public DbContext()
            : base("PostgreSQL", Connectionstring())
        {
        }
        public ITable<GameModel> Games => this.GetTable<GameModel>();
        public ITable<UserModel> Users => this.GetTable<UserModel>();
        public ITable<ItemModel> Items => this.GetTable<ItemModel>();
    }
}
