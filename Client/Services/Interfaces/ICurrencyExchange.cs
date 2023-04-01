namespace Cargotruck.Client.Services.Interfaces
{
    public interface ICurrencyExchange
    {
        float? GetCurrencyAmount(long? amount, string currency);
        DateTime GetApiLastRequestDate();
        string GetCurrencyType();
        Task<DateTime> GetNextApiRequestDate(HttpClient client);
        Task<string> RequestRatesFromApiAsync(HttpClient client);
        Task<int> GetWaitTimeAsync(HttpClient client);
        void SetApiLastRequestDate(DateTime newDate);
        void SetCurrencyType(string newCurrency);
        Dictionary<string, dynamic>? GetRates();

        void SetRates(Dictionary<string, dynamic>? newRates);
    }
}