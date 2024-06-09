using System.Text.Json;

namespace TelegramSteamTrade_Bot
{
    public class SteamMethod
    {
        public double ItemLowestPrice { get; set; }
        public List<string> SteamMarketHashName { get; set; }

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

        public async Task<List<InventoryItem>> GetInventoryItemsAsync(string steamId, int appId, int contextId)
        {
            string url = $"https://steamcommunity.com/inventory/{steamId}/{appId}/{contextId}?l=english&count=5000";

            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var inventoryResponse = JsonSerializer.Deserialize<SteamInventoryResponse>(responseBody);
                    var inventoryItems = inventoryResponse.descriptions;
                    foreach (var item in inventoryItems)
                    {
                        if (item.marketable != 0)
                            SteamMarketHashName.Add(item.market_hash_name);
                    }
                    return inventoryResponse?.descriptions ?? new List<InventoryItem>();
                }
                else
                {
                    Console.WriteLine($"HTTP Error: {response.StatusCode}");
                    return new List<InventoryItem>();
                }
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
        public int marketable { get; set; }
    }
}