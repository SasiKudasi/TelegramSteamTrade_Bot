using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramSteamTrade_Bot.Models;

namespace TelegramSteamTrade_Bot.Data
{
    public class GamesData
    {

        private DbContext _db = new();
        public async Task GetAllGamesName(ITelegramBotClient client, Update update, CancellationToken token)
        {
            var games = _db.Games.ToList();
            foreach (var game in games)
            {
                await client.SendTextMessageAsync(update.Message!.Chat.Id, $"{game.Name}\n", cancellationToken: token);
            }
            _db.Close();
        }
        public int GetGameAppId(string? text)
        {

            var gameAppId = _db.Games.FirstOrDefault(gameId => gameId.Name == text)!.AppId;
            _db.Close();
            return gameAppId;

        }
    }
}
