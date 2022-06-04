using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
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

        public async Task<double> GetRate(CurrencyCodes fromCurrency, CurrencyCodes toCurrency)
        {
            var response = await _client.GetFromJsonAsync<CurrencyRateResponse>("daily_json.js");

            if (response == null)
            {
                throw new WebException("Failed to get up-to-date exchange rates");
            }

            double GetRateFromResponse(CurrencyCodes code) =>
                code == CurrencyCodes.RUB ? 1 : response.Valute[code.ToString()].Value;

            var fromCurrencyRate = GetRateFromResponse(fromCurrency);
            var toCurrencyRate = GetRateFromResponse(toCurrency);

            return fromCurrencyRate / toCurrencyRate;
        }
    }
}