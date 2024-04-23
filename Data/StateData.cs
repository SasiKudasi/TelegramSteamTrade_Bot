using LinqToDB;
using Steam.Models.SteamCommunity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramSteamTrade_Bot.Models;

namespace TelegramSteamTrade_Bot.Data
{
    internal class StateData : BaseData, IWorkWhithEntity
    {
        public async Task CreateNewEntity<T>(T entity) where T : class
        {
            await _db.InsertWithIdentityAsync(entity);
            _db.Close();
        }

        public async Task<T> GetEntity<T>(string name) where T : class
        {
            long person = 0;
            var id = long.TryParse(name, out person);
            if (!id)

                return null;
            else
            {
                var state = await _db.State.FirstOrDefaultAsync(x => x.UserId == person) as T;
                _db.Close();
                return state;
            }
        }
    }
}
