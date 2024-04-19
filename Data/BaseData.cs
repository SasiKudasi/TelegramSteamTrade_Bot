using LinqToDB;
using LinqToDB.Data;
using System;
using Telegram.Bot;
using TelegramSteamTrade_Bot.Models;
using static LinqToDB.DataProvider.Access.AccessHints;
using static LinqToDB.DataProvider.ClickHouse.ClickHouseHints;

namespace TelegramSteamTrade_Bot.Data
{
    public class BaseData
    {
        protected DbContext _db = new();
        public async Task SetState(long person, object mode)
        {
            var userData = new UsersData();
            var user = await userData.GetEntity<UserModel>(person.ToString());

            if (mode is (ModeMain))
            {
                var modeMain = (ModeMain)mode;
                await _db.State.Where(u => u.UserId == user.Id).
                      Set(m => m.ModeMain, modeMain).
                      UpdateAsync();
            }
            else if (mode is (ModeGame))
            {
                var modeGame = (ModeGame)mode;
                await _db.State.Where(u => u.UserId == user.Id).
                    Set(m => m.ModeGame, modeGame).
                    UpdateAsync();
            }
            else if (mode is (int))
            {
                var lastItem = (int)mode;
                await _db.State.Where(u => u.UserId == user.Id).
                    Set(m => m.LastItemState, lastItem).
                    UpdateAsync();
            }
        }
        public bool ParsStringIntoLong(string str, out long nam)
        {
            if (str == null)
            {
                nam = 0;
                return false;
            }
            else
            {                
                var id = long.TryParse(str, out nam);
                if (id)
                    return true;
                else
                    return false;
            }
        }
        public async Task<StateModel> GetMode(UserModel user)
        {
            StateData _stateData = new();
            var userState = await _stateData.GetEntity<StateModel>(user.Id.ToString());
            if (userState == null)
            {
                await _stateData.CreateNewEntity(new StateModel()
                {
                    UserId = user.Id,
                    ModeMain = ModeMain.Start,
                    ModeGame = ModeGame.Initial,
                    LastItemState = 0
                });
                userState = await _stateData.GetEntity<StateModel>(user.Id.ToString());
            }
            return userState!;
        }

    }
}
