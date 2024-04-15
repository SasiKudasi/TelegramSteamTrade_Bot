using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramSteamTrade_Bot.Models;

namespace TelegramSteamTrade_Bot.Data
{
    internal class StateData
    {
        private DbContext _db = new();
        public StateModel CreateStateForNewUser(UserModel newUser)
        {
            var state = new StateModel()
            {
                UserId = newUser.Id,
                ModeMain = ModeMain.Start,
                ModeGame = ModeGame.Initial,
                LastItemState = 0
            };
            _db.InsertWithIdentity(state);
            return state;
        }

        public StateModel GetMode(UserModel user)
        {
           var state = _db.State.FirstOrDefault(x=>x.UserId == user.Id);
            return state!;
        }
    }
}
