using System;
using MiniBank.Core.Exceptions;

namespace MiniBank.Core
{
    public class CurrencyConverter : ICurrencyConverter
    {
        private readonly ICurrencyRateProvider _provider;

        public CurrencyConverter(ICurrencyRateProvider provider)
        {
            _provider = provider;
        }

        public int Convert(int amount, string fromCurrency, string toCurrency)
        {
            if (amount < 0)
                throw new UserFriendlyException("Amount must be positive");

            return amount / _provider.GetRate(fromCurrency, toCurrency);
        }
    }
}