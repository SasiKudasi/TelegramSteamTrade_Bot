using Telegram.Bot.Types.ReplyMarkups;
using TelegramSteamTrade_Bot.Data;

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
            var keyboadrd = new InlineKeyboardMarkup(GamesData.GameKeyboard());


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
