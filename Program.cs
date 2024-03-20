using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TelegramSteamTrade_Bot;

class Program
{
    static async Task Main(string[] args)
    {
        SteamMethod steam = new();
        await steam.SearchItemPriceAsync();
        Console.ReadKey();
       
    }
}

