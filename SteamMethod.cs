using System.Text.Json;

namespace TelegramSteamTrade_Bot
{
    public class SteamMethod
    {
        public double ItemLowestPrice { get; set; }


        public async Task<double> SearchItemPriceAsync(int gameId, string itemName)
        {
            string url = $"https://steamcommunity.com/market/priceoverview/?appid={gameId}&currency=5&market_hash_name={Uri.EscapeDataString(itemName)}";
            double result = 0.0;
            string lowestPrice ="";
            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonSerializer.Deserialize<GetRequestFields>(responseBody);
                    if (jsonResponse != null)
                    {
                        try
                        {
                            lowestPrice = jsonResponse.lowest_price.ToString();
                        }
                        catch
                        {
                          return  result = 0.0;
                        }
                        bool success = double.TryParse(lowestPrice.AsSpan(0, lowestPrice.IndexOf(" ")), out result);
                            ItemLowestPrice = result;
                       
                    }
                }
                else
                    result = 0.0;

                return result;
            }
        }
        public class GetRequestFields
        {
            public string lowest_price { get; set; }
        }
    }
}