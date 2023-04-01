﻿using Newtonsoft.Json;
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

        public async Task<string> RequestRatesFromApiAsync(HttpClient client)
        {
            //the api has only 300 attempts per month - its run every start of the application
            var request = new HttpRequestMessage
            {
                Method = new HttpMethod("GET"),
                RequestUri = new Uri("https://api.apilayer.com/exchangerates_data/latest?symbols=EUR,HUF,USD,CZK&base=EUR")
            };

            /*	ExchangeApiKey	XwSDWGpGqDKu8ZIbOl56Kne74V14oEpC */
            var settings = await client!.GetFromJsonAsync<Setting[]>("api/settings/get");
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
                SetRates(JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(ratesJson));
                SetApiLastRequestDate(new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0));
                return response.IsSuccessStatusCode.ToString();
            }
            else
            {
                var error = response.Content.ReadAsStringAsync();
                return error.Result;
            }
        }

        public float? GetCurrencyAmount(long? amount, string currency)
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
