using Telegram.Bot.Types.ReplyMarkups;
using TelegramSteamTrade_Bot.Data;

namespace TelegramSteamTrade_Bot
{
    delegate InlineKeyboardButton[][] Keyboard();
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
        public static InlineKeyboardMarkup InlineKeyboard(Keyboard keyboard)
        {
            var keyboadrd = new InlineKeyboardMarkup(keyboard());
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
        public static InlineKeyboardButton[][] GoToStartBtn()
        {
            var btns = new InlineKeyboardButton[1][];
            btns[0] = new[]
            {
                InlineKeyboardButton.WithCallbackData("Старт", "Старт")
            };
            return btns;
        }
    }
}
