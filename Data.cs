﻿using LinqToDB;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramSteamTrade_Bot.Models;
using static LinqToDB.Reflection.Methods.LinqToDB;
using Update = Telegram.Bot.Types.Update;

namespace TelegramSteamTrade_Bot
{
    public class Data
    {
        private static SteamMethod _steam = new();
        private static DbContext _db = new DbContext();

        internal static ModeMain GetModeMain(long person)
        {
            var mode = _db.Users.FirstOrDefault(u => u.ChatId == person);
            return mode.ModeMain;
        }
        internal static ModeGame GetModeGame(long person)
        {
            var mode = _db.Users.FirstOrDefault(u => u.ChatId == person);
            return mode.ModeGame;
        }
        public static bool GetUser(long person)
        {
            var user = _db.Users.FirstOrDefault(x => x.ChatId == person);

            if (user == null)
            {
                CreateNewUser(person);
            }
            return true;
        }

        private static void CreateNewUser(long person)
        {
            var newUser = new UserModel()
            {
                ChatId = person,
                ModeMain = ModeMain.Start,
                ModeGame = ModeGame.Initial
            };
            _db.InsertWithIdentity(newUser);
        }

        public static void SetState(long person, Enum mode)
        {
            if (mode.GetType() == typeof(ModeMain))
            {
                _db.Users.Where(p => p.ChatId == person)
               .Set(m => m.ModeMain, mode).Update();
            }
            if (mode.GetType() == typeof(ModeGame))
            {
                _db.Users.Where(p => p.ChatId == person)
              .Set(m => m.ModeGame, mode).Update();
            }
        }

        public static async Task SetStateAsync(ITelegramBotClient client, Update update, CancellationToken token)
        {
            var msg = update.Message.Text;
            long person = update.Message.Chat.Id;
            switch (msg)
            {

                case "/start":
                    SetState(person, ModeMain.Start);
                    break;
                case "/add_item_to_track":
                    SetState(person, ModeMain.Start);
                    break;
                case "/check_tracking_item":
                    SetState(person, ModeMain.Start);
                    break;
                case "/check_item_price":
                    await client.SendTextMessageAsync(update.Message!.Chat.Id, "Выберите игру, предметы которой хотите посмотерть", cancellationToken: token);
                    await GetAllGamesName(client, update, token);
                    SetState(person, ModeMain.GetItem);
                    break;
            }
        }

        public static async Task ItemMenuAsync(ITelegramBotClient client, Update update, CancellationToken token)
        {
            var gameMode = GetModeGame(update.Message.Chat.Id);
            var msg = update.Message!.Text;
            if (msg == "/check_item_price")
                return;
            else
            {
                if (gameMode == ModeGame.Initial)
                {
                    switch (msg)
                    {
                        case "/cs2":
                            SetState(update.Message.Chat.Id, ModeGame.GetCSItems);
                            break;
                        case "/dota2":
                            SetState(update.Message.Chat.Id, ModeGame.GetDotaItems);
                            break;
                    }
                    await client.SendTextMessageAsync(update.Message!.Chat.Id, "Пожалуйста, введите название предмета, цену которого хотите посмотреть" +
                        "\nприм. AK-47 | Redline (Minimal Wear)", cancellationToken: token);
                }
                else
                {
                    switch (gameMode)
                    {
                        case ModeGame.GetCSItems:
                            GetItems(client, update, token, GetGameAppId("/cs2"));
                            break;
                        case ModeGame.GetDotaItems:
                            GetItems(client, update, token, GetGameAppId("/dota2"));
                            break;
                    }
                }
            }
        }

        private static async void GetItems(ITelegramBotClient client, Update update, CancellationToken token, int v)
        {
            var msg = update.Message!.Text;
            if (msg == "/cs2" || msg == "/dota2")
                return;
            else
            {
                var price = 0.0;
                var items = _db.Items.FirstOrDefault(n => n.Name == msg);
                if (items == null)
                {
                    CreateNewItem(v, msg);
                    price = await CheckItem(client, update, token, v);
                    SetState(update.Message.Chat.Id, ModeGame.Initial);
                }
                else
                {
                    price = await CheckItem(client, update, token, v);
                    SetState(update.Message.Chat.Id, ModeGame.Initial);
                }
            }
        }

        private static async Task<double> CheckItem(ITelegramBotClient client, Update update, CancellationToken token, int v)
        {
            var msg = update.Message!.Text;
            var price = 0.0;
           price =  await _steam.SearchItemPriceAsync(v, msg!);
            if (_steam.ItemLowestPrice == 0.0)
            {
                await client.SendTextMessageAsync(update.Message!.Chat.Id, "Похоже такого предмета не существует", cancellationToken: token);
                _db.Items.Delete(n => n.Name == msg);
            }
            else
            {
                await client.SendTextMessageAsync(update.Message!.Chat.Id, $"Актуальная цена {msg} на данный момент составляет {price}", cancellationToken: token);
                _db.Items.Where(p => p.Name == msg)
              .Set(m => m.ItemPrice, price).Update();
            }
            return price;

        }

        private static void CreateNewItem(int v, string? msg)
        {
            var item = new ItemModel()
            {
                Name = msg!,
                GameId = v,
            };
            _db.InsertWithIdentity(item);
        }

        public static async Task GetAllGamesName(ITelegramBotClient client, Update update, CancellationToken token)
        {
            var games = _db.Games.ToList();
            foreach (var game in games)
            {
                await client.SendTextMessageAsync(update.Message!.Chat.Id, $"{game.Name}\n", cancellationToken: token);
            }
        }

        private static int GetGameAppId(string? text)
        {
            var gameAppId = _db.Games.FirstOrDefault(gameId => gameId.Name == text)!.AppId;
            return gameAppId;
        }
    }
}
