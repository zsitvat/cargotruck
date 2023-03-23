namespace Cargotruck.Client.Services
{
    public interface ICurrencyExchange
    {
        float? GetCurrency(long? amount, string currency);
        DateTime GetCurrencyApiDate();
        string GetCurrencyType();
        Task<DateTime> GetNextCurrencyApiDateAsync(HttpClient client);
        Task<dynamic> GetRatesFromApiAsync(HttpClient? client);
        Task<int> GetWaitTimeAsync(HttpClient client);
        void SetCurrencyApiDate(DateTime newDate);
        void SetCurrencyType(string newCurrency);
        Dictionary<string, dynamic>? GetRates();

        void SetRates(Dictionary<string, dynamic>? newRates);
    }
}