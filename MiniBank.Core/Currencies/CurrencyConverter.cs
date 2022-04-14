using System;
using System.Threading.Tasks;
using MiniBank.Core.Exceptions;

namespace MiniBank.Core.Currencies
{
    public class CurrencyConverter : ICurrencyConverter
    {
        private readonly ICurrencyRateProvider _provider;

        public CurrencyConverter(ICurrencyRateProvider provider)
        {
            _provider = provider;
        }

        public async Task<double> Convert(double amount, CurrencyCodes fromCurrency, CurrencyCodes toCurrency)
        {
            if (amount < 0)
            {
                throw new ValidationException("Amount must be positive");
            }

            double rate = await _provider.GetRate(fromCurrency, toCurrency);

            return Math.Round(amount / rate, 2);
        }
    }
}