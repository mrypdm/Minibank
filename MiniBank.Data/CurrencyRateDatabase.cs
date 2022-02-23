using System;
using MiniBank.Core;

namespace MiniBank.Data
{
    public class RubleRateDatabase : IRubleRateDatabase
    {
        private readonly Random _random = new();
        
        public int Get(string currencyCode)
        {
            return _random.Next(0, 100);
        }
    }
}
