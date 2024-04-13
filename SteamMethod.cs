using System.Text.Json;

namespace TelegramSteamTrade_Bot
{
    public class SteamMethod
    {
        public int GameID { get; set; }
        public string ItemName { get; set; }
        public double ItemLowestPrice { get; set; }
        public SteamMethod() { }
        public SteamMethod(int gameID, string itemName, double itemLowestPrice)
        {
            GameID = gameID;
            ItemName = itemName;
            ItemLowestPrice = itemLowestPrice;
        }

        public async Task<double> SearchItemPriceAsync(int gameId, string itemName)
        {
            string url = $"https://steamcommunity.com/market/priceoverview/?appid={gameId}&currency=5&market_hash_name={Uri.EscapeDataString(itemName)}";
            double res = 0.0;

            using (var httpClient = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var jsonResponse = JsonSerializer.Deserialize<GetRequestFields>(responseBody);
                        if (jsonResponse != null)
                        {
                            var lowestPrice = jsonResponse.lowest_price.ToString();
                            bool success = double.TryParse(lowestPrice.AsSpan(0, lowestPrice.IndexOf(" ")), out res);
                            GameID = gameId;
                            ItemName = itemName;
                            ItemLowestPrice = res;
                            return res;
                        }

                    }
                    else
                    {
                        return 0.0;
                    }
                }
                catch
                {
                    return 0.0;
                }
            }
            return res;
        }
    }
    public class GetRequestFields
    {
        public string lowest_price { get; set; }
    }
}
