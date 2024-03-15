using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        string itemName = "AK-47 | Redline (Minimal Wear)"; // Название предмета
      
        string url = $"https://steamcommunity.com/market/priceoverview/?appid=730&currency=5&market_hash_name={Uri.EscapeDataString(itemName)}";
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
                        Console.WriteLine($"Response Content: item name {itemName} : {res} руб.");
                    }
                    
                }
                else
                {
                    Console.WriteLine($"Такого предмета не существует");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        Console.ReadKey();
    }
}
public class GetRequestFields
{
    public string lowest_price { get; set; }
}
