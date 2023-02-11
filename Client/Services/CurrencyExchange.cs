using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace Cargotruck.Client.Services
{
    public static class CurrencyExchange
    {
        [CascadingParameter] public static Dictionary<string, dynamic>? Rates { get; set; }
        public static DateTime? CurrencyApiDate = new();

        public static string currency = "HUF";
        public static async Task<dynamic> GetRatesAsync(HttpClient? client)
        {
            //the api has only 300 attempts per month - its run every start of the application
            var request = new HttpRequestMessage
            {
                Method = new HttpMethod("GET"),
                RequestUri = new Uri("https://api.apilayer.com/exchangerates_data/latest?symbols=EUR,HUF,USD,CZK&base=EUR")
            };

            request.Headers.Add("apikey", "XwSDWGpGqDKu8ZIbOl56Kne74V14oEpC");

            var response = await client?.SendAsync(request)!;
            var result = response.Content.ReadAsStringAsync().Result;
            if (response.IsSuccessStatusCode)
            {
                Dictionary<string, dynamic> dict = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(result)!;
                var ratesJson = dict["rates"].ToString().Trim(new Char[] { '\n' }).Replace(" ", "");
                return JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(ratesJson);
            }
            else
            {
                var error = response.Content.ReadAsStringAsync();
                return error;
            }
        }

        public static float? GetCurrency(int? amount, string currency)
        {
            float? conversionNum = amount;
            if (CurrencyExchange.Rates != null && currency != "HUF")
            {
                if (currency != "EUR")
                {
                    conversionNum = (float)((amount / Rates["HUF"]) * Rates[currency]);
                }
                else
                {
                    conversionNum = (float)(amount / Rates["HUF"]);
                }
            }
            return conversionNum;
        }
    }
}
