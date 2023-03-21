using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System.Net.Http.Json;
using Cargotruck.Shared.Models;

namespace Cargotruck.Client.UtilitiesClasses
{
    public static class CurrencyExchange
    {
        [CascadingParameter] public static Dictionary<string, dynamic>? Rates { get; set; }
        public static DateTime CurrencyApiDate = new();

        public static string currency = "HUF";

        public static async Task<dynamic> GetRatesAsync(HttpClient? client)
        {
            //the api has only 300 attempts per month - its run every start of the application
            var request = new HttpRequestMessage
            {
                Method = new HttpMethod("GET"),
                RequestUri = new Uri("https://api.apilayer.com/exchangerates_data/latest?symbols=EUR,HUF,USD,CZK&base=EUR")
            };

            /*	ExchangeApiKey	XwSDWGpGqDKu8ZIbOl56Kne74V14oEpC */
            var settings = await client!.GetFromJsonAsync<Settings[]>("api/settings/get");
            var key = settings?.FirstOrDefault(x => x.SettingName == "ExchangeApiKey");

            if (key != null)
            {
                request.Headers.Add("apikey", key?.SettingValue);
            }

            var response = await client?.SendAsync(request)!;
            var result = response.Content.ReadAsStringAsync().Result;
            if (response.IsSuccessStatusCode)
            {
                Dictionary<string, dynamic> dict = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(result)!;
                var ratesJson = dict["rates"].ToString().Trim(new char[] { '\n' }).Replace(" ", "");
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
            if (Rates != null && currency != "HUF")
            {
                if (currency != "EUR")
                {
                    conversionNum = (float)(amount / Rates["HUF"] * Rates[currency]);
                }
                else
                {
                    conversionNum = (float)(amount / Rates["HUF"]);
                }
            }
            return conversionNum;
        }

        public async static Task<int> GetWaitTimeAsync(HttpClient client)
        {
            var getWaitTimeSetting = (await client.GetFromJsonAsync<Settings>("api/settings/getwaittime"));
            return (getWaitTimeSetting != null ? Int32.Parse(getWaitTimeSetting?.SettingValue!) : 0);      
        }

        public async static Task<DateTime> GetNextCurrencyApiDateAsync(HttpClient client)
        {
            return CurrencyExchange.CurrencyApiDate.AddSeconds(await GetWaitTimeAsync(client));
        }
    }
}
