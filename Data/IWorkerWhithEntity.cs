using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramSteamTrade_Bot.Data
{
    public interface IWorkerWhithEntity
    {
        public Task CreateNewEntity<T>(T entity) where T: class;
        public Task<T> GetEntity<T>(string name) where T : class;
    }
}
