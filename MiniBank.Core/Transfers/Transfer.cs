using System;
using MiniBank.Core.Currencies;

namespace MiniBank.Core.Transfers
{
    public class Transfer
    {
        public string Id { get; set; }
        public double Amount { get; set; }
        public CurrencyCodes CurrencyCode { get; set; }
        public string FromAccountId { get; set; }
        public string ToAccountId { get; set; }
        public DateTime TransferDateTime { get; set; }
    }
}