using System;
using MiniBank.Core;

namespace MiniBank.Data
{
    public class CurrencyRateDatabase : IDatabase
    {
        private readonly Random _random = new();
        
        public int Get(string key)
        {
            return _random.Next(0, 100);
        }
    }
}
