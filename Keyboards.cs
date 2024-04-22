using Telegram.Bot.Types.ReplyMarkups;
namespace TelegramSteamTrade_Bot
{
    static class Keyboards
    {
        public static ReplyKeyboardMarkup MainKeyboard()
        {
            var keyboardButton = new ReplyKeyboardMarkup(
                new[]
                {
                    new KeyboardButton [] {"Посмотреть актуальную цену на предмет"},
                    new KeyboardButton [] {"Посмотреть цены на предметы из личного списка"},
                    new KeyboardButton [] {"Удалить предмет из списка"},
                    new KeyboardButton [] {"Старт"},
                })
            {
                ResizeKeyboard = true,               

            };
            return keyboardButton;
        }

    }
}
