using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramSteamTrade_Bot.Models;

namespace TelegramSteamTrade_Bot.Data
{
    internal class StateData : BaseData
    {
        public async Task<StateModel> CreateStateForNewUser(UserModel newUser)
        {
            var state = new StateModel()
            {
                UserId = newUser.Id,
                ModeMain = ModeMain.Start,
                ModeGame = ModeGame.Initial,
                LastItemState = 0
            };
            await _db.InsertWithIdentityAsync(state);
            return state;
        }
    }
}
