﻿using LinqToDB;
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
            _db.Close();
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
        public async Task SwitchStateAsync(ITelegramBotClient client, Update update, CancellationToken token)
        {
            var msg = update.Message!.Text;
            long person = update.Message.Chat.Id;
            switch (msg)
            {
                case "Старт":
                    await SetState(person, ModeMain.Start);
                    break;
                case "Удалить предмет из списка":
                    await client.SendTextMessageAsync(update.Message!.Chat.Id, "Введите номер предмета, который хотите удалить", cancellationToken: token);
                    await SetState(person, ModeMain.DeleteItem);
                    break;
                case "Посмотреть цены на предметы из личного списка":
                    await SetState(person, ModeMain.GetAllItem);
                    break;
                case "Посмотреть актуальную цену на предмет":
                    await client.SendTextMessageAsync(update.Message!.Chat.Id, "Выберите игру, предметы которой хотите посмотерть", cancellationToken: token);
                    await _gamesData.GetAllGamesName(client, update, token);
                    await SetState(person, ModeMain.GetItem);
                    break;
            }
        }

    }
}
