using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System.Net.Http.Json;
using Cargotruck.Shared.Model;

namespace Cargotruck.Client.Services
{
    public class CurrencyExchange : ICurrencyExchange
    {
        private Dictionary<string, dynamic>? Rates { get; set; }
        private DateTime CurrencyApiDate = new();

        private string currency = "HUF";

        public async Task<dynamic> GetRatesFromApiAsync(HttpClient? client)
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

        public float? GetCurrency(long? amount, string currency)
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

        public Dictionary<string, dynamic>? GetRates()
        {
            return Rates;
        }

        public void SetRates(Dictionary<string, dynamic>? newRates)
        {
            Rates = newRates;
        }

        public string GetCurrencyType()
        {
            return currency;
        }

        public void SetCurrencyType(string newCurrency)
        {
            currency = newCurrency;
        }

        public async Task<int> GetWaitTimeAsync(HttpClient client)
        {
            var getWaitTimeSetting = await client.GetFromJsonAsync<Settings>("api/settings/getwaittime");
            return getWaitTimeSetting != null ? int.Parse(getWaitTimeSetting?.SettingValue!) : 0;
        }

        public void SetCurrencyApiDate(DateTime newDate)
        {
            CurrencyApiDate = newDate;
        }

        public DateTime GetCurrencyApiDate()
        {
            return CurrencyApiDate;
        }

        public async Task<DateTime> GetNextCurrencyApiDateAsync(HttpClient client)
        {
            return CurrencyApiDate.AddSeconds(await GetWaitTimeAsync(client));
        }
    }
}
