using System;
using System.Collections.Generic;

namespace MiniBank.Data.Currencies
{
    public class CurrencyData
    {
        public string CharCode { get; set; }
        public double Value { get; set; }
    }
    
    public class CurrencyRateResponse
    {
        public DateTime Date { get; set; }
        public Dictionary<string, CurrencyData> Valute { get; set; }
    }
}