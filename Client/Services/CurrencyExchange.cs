using Newtonsoft.Json;
using System.Net.Http.Json;
using Cargotruck.Shared.Model;
using Cargotruck.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Cargotruck.Shared.Model.Dto;
using System.Net.Http;

namespace Cargotruck.Client.Services
{
    public class CurrencyExchange : ICurrencyExchange
    {
        private Dictionary<string, dynamic>? Rates { get; set; }
        private DateTime ApiLastRequestDate = new();
        private string currency = "HUF";

        public async Task<string?> RequestRatesFromApiAsync(HttpClient client)
        {
            //the api has only 300 attempts per month (free version) - its run every start of the application
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.apilayer.com/exchangerates_data/latest?symbols=EUR,HUF,USD,CZK&base=EUR");

            /*	ExchangeApiKey	XwSDWGpGqDKu8ZIbOl56Kne74V14oEpC */
            var settings = await client!.GetFromJsonAsync<Setting[]>("api/settings/get");
            var key = settings?.FirstOrDefault(x => x.SettingName == "ExchangeApiKey");

            if (key != null)
            {
                request.Headers.Add("apikey", key?.SettingValue);
            }

            var response = client != null ? await client.SendAsync(request) : null;
            var result = response != null ? await response.Content.ReadAsStringAsync() : null;

            if (result != null && response != null && response.IsSuccessStatusCode)
            {
                Dictionary<string, dynamic>? dict = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(result);
                var ratesJson = dict?["rates"].ToString().Trim('\n').Replace(" ", "");
                SetRates(JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(ratesJson));
                SetApiLastRequestDate(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0));
                return response.IsSuccessStatusCode.ToString();
            }
            else
            {
                var error = result ?? (response != null ? await response.Content.ReadAsStringAsync() : null);
                return error;
            }
        }

        public float? GetCurrencyAmount(long? amount, string currency)
        {
            if (Rates != null && currency != "HUF")
            {
                if (currency != "EUR")
                {
                    return (float?)(amount / Rates["HUF"] * Rates[currency]);
                }
                else
                {
                    return (float?)(amount / Rates["HUF"]);
                }
            }
            else
            {
                return amount;
            }
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
            var getWaitTimeSetting = await client?.GetFromJsonAsync<SettingDto>("api/settings/getwaittime")!;
            return getWaitTimeSetting != null ? int.Parse(getWaitTimeSetting?.SettingValue!) : 0;
        }

        public void SetApiLastRequestDate(DateTime newDate)
        {
            ApiLastRequestDate = newDate;
        }

        public DateTime GetApiLastRequestDate()
        {
            return ApiLastRequestDate;
        }

        public async Task<DateTime> GetNextApiRequestDate(HttpClient client)
        {
            return ApiLastRequestDate.AddSeconds(await GetWaitTimeAsync(client));
        }
    }
}
