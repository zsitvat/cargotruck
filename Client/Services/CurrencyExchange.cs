using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System;

namespace Cargotruck.Client.Services
{
    public static class CurrencyExchange
    {
        [Inject]
        static HttpClient? client { get; set; }
        public static async Task<dynamic> GetRatesAsync(HttpClient? client)
        {
            var request = new HttpRequestMessage
            {
                Method = new HttpMethod("GET"),
                RequestUri = new Uri("https://api.apilayer.com/exchangerates_data/latest?symbols=EUR,HUF,USD,CZK&base=EUR")
            };

            request.Headers.Add("apikey", "XwSDWGpGqDKu8ZIbOl56Kne74V14oEpC");

            var response = await client.SendAsync(request);
            var result = response.Content.ReadAsStringAsync().Result;
            if (response.IsSuccessStatusCode)
            {
                Dictionary<string, dynamic> dict = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(result);
                var ratesJson = dict["rates"].ToString().Trim(new Char[] { '\n' }).Replace(" ", "");
                return JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(ratesJson);
            }
            else 
            {
                var error = response.Content.ReadAsStringAsync();
                return error;
            }
        }

    }
}
