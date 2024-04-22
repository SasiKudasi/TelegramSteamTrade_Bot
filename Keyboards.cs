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
        public static InlineKeyboardMarkup GameKeyboard()
        {
            var keyboadrd = new InlineKeyboardMarkup(
                new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("CS2", "/cs2")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Dota2", "/dota2")
                    }
                });
            return keyboadrd;
        }

        public static InlineKeyboardMarkup KonfirmKeyboard()
        {
            var keyboadrd = new InlineKeyboardMarkup(
                new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Да", "/yes")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Нет", "Старт")
                    }
                });
            return keyboadrd;
        }
    }
}
