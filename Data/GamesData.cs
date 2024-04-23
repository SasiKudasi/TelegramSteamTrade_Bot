﻿using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramSteamTrade_Bot.Models;

namespace TelegramSteamTrade_Bot.Data
{
    public class GamesData
    {

        private static DbContext _db = new();
       
        public int GetGameAppId(string? text)
        {

            var gameAppId = _db.Games.FirstOrDefault(gameId => gameId.Name == text)!.AppId;
            _db.Close();
            return gameAppId;

        }
        public static InlineKeyboardButton[][] GameKeyboard()
        {
            var games = _db.Games.ToList();
            _db.Close();
            var btns = new InlineKeyboardButton[games.Count][];
            int i = 0;
            foreach (var game in games)
            {
                btns[i] = new[]
                {
                    InlineKeyboardButton.WithCallbackData(game.Name.Trim('/').ToUpper(), game.Name)
                };             
                i++;
            }
            return btns;
        }       
    }
}
