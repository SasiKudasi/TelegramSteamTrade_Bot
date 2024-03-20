using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TelegramSteamTrade_Bot;

class Program
{
    static async Task Main(string[] args)
    {
        string cmd = "";
        List<SteamMethod> items = new List<SteamMethod>();
        while (true)
        {
            int gameId = ChoseTheGame();
            string gameItem = ChoseGameItem();
            SteamMethod steam = new();
            await steam.SearchItemPriceAsync(gameId, gameItem);
            Console.WriteLine($"Предмет {steam.ItemName} стоит: {steam.ItemLowestPrice} руб\nХотите ли вы добавить этот предмет для отслеживания его цены?");
            cmd = Console.ReadLine();
            if (cmd == "Да")
            {
                items.Add(new SteamMethod(steam.GameID, steam.ItemName, steam.ItemLowestPrice));
                foreach (SteamMethod item in items)
                {
                    double newPrice = await steam.SearchItemPriceAsync(item.GameID, item.ItemName);
                    Console.WriteLine($"{item.GameID} Предмет {item.ItemName} стоил на момент добавления: {item.ItemLowestPrice} \nЕго актуальная цена {newPrice}");
                }
            }
        }


    }

    static int ChoseTheGame()
    {
        Console.WriteLine("Выберите игру");
        string game = Console.ReadLine();
        int gameId = 0;
        switch (game)
        {
            case "CS2":
                gameId = 730;
                break;
            case "Dota2":
                gameId = 570;
                break;
            default:
                break;
        }
        return gameId;
    }
    static string ChoseGameItem()
    {
        Console.WriteLine("Введите название предмета\nприм. AK-47 | Redline (Minimal Wear)");
        string item = Console.ReadLine();
        return item;
    }
}

