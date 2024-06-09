﻿using System.Text.Json;

namespace TelegramSteamTrade_Bot
{
    public class SteamMethod
    {
        public double ItemLowestPrice { get; set; }
        public string SteamMarketHashName { get; set; }

        public async Task<double> SearchItemPriceAsync(int gameId, string itemName)
        {
            string url = $"https://steamcommunity.com/market/priceoverview/?appid={gameId}&currency=5&market_hash_name={Uri.EscapeDataString(itemName)}";
            double result = 0.0;
            string lowestPrice = "";
            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonSerializer.Deserialize<SteamMakretRequestFields>(responseBody);
                    if (jsonResponse != null && jsonResponse.lowest_price != null)
                    {
                        lowestPrice = jsonResponse.lowest_price.ToString();
                        bool success = double.TryParse(lowestPrice.AsSpan(0, lowestPrice.IndexOf(" ")), out result);
                        ItemLowestPrice = result;
                    }
                }
                else
                    result = 0.0;

                return result;
            }
        }
       
    }
    public class SteamMakretRequestFields
    {
        public string lowest_price { get; set; }
    }

    public class SteamInventoryResponse
    {
        public List<Asset> assets { get; set; }
        public List<InventoryItem> descriptions { get; set; }
        public int total_inventory_count { get; set; }
        public int success { get; set; }
    }

    public class Asset
    {
        public long appid { get; set; }
        public string contextid { get; set; }
        public string assetid { get; set; }
        public string classid { get; set; }
        public string instanceid { get; set; }
        public string amount { get; set; }
    }

    public class InventoryItem
    {
        public string classid { get; set; }
        public string instanceid { get; set; }
        public string market_hash_name { get; set; }
        public string market_name { get; set; }
    }
}