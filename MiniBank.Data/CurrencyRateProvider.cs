using System;
using MiniBank.Core;

namespace MiniBank.Data
{
    public class CurrencyRateProvider : ICurrencyRateProvider
    {
        private readonly Random _random = new();
        
        public int GetRate(string fromCurrency, string toCurrency)
        {
            return _random.Next(0, 100);
        }
    }
}
