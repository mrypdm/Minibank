using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MiniBank.Core.Exceptions;

namespace MiniBank.Core.Currencies
{
    public class CurrencyConverter : ICurrencyConverter
    {
        private readonly ICurrencyRateProvider _provider;
        private readonly ILogger<CurrencyConverter> _logger;

        public CurrencyConverter(ICurrencyRateProvider provider, ILogger<CurrencyConverter> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        public async Task<double> Convert(double amount, CurrencyCodes fromCurrency, CurrencyCodes toCurrency)
        {
            if (amount < 0)
            {
                throw new ValidationException("Amount must be positive");
            }

            var rate = await _provider.GetRate(fromCurrency, toCurrency);

            var convertedAmount = Math.Round(amount * rate, 2);

            _logger.LogInformation(
                "Converted amount='{Amount}' from currency='{FromCurrency}' to currency='{ToCurrency}' with result='{ConvertedAmount}'",
                amount, fromCurrency, toCurrency, convertedAmount);

            return convertedAmount;
        }
    }
}