using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using MiniBank.Core.Currencies;

namespace MiniBank.Data.Currencies
{
    public class CurrencyRateProvider : ICurrencyRateProvider
    {
        private readonly HttpClient _client;

        public CurrencyRateProvider(HttpClient client)
        {
            _client = client;
        }

        public double GetRate(CurrencyCodes fromCurrency, CurrencyCodes toCurrency)
        {
            var response = _client.GetFromJsonAsync<CurrencyRateResponse>("daily_json.js")
                .GetAwaiter().GetResult();

            if (response == null)
            {
                throw new WebException("Failed to get up-to-date exchange rates");
            }

            double GetRateFromResponse(CurrencyCodes code) =>
                code == CurrencyCodes.RUB ? 1 : response.Valute[code.ToString()].Value;

            var toFirstRate = GetRateFromResponse(fromCurrency);
            var toSecondRate = GetRateFromResponse(toCurrency);

            return toSecondRate / toFirstRate;
        }
    }
}