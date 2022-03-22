using System;
using MiniBank.Core.Currencies;

namespace MiniBank.Data.Transfers
{
    public class TransferDbModel
    {
        public string Id { get; set; }
        public double Amount { get; set; }
        public CurrencyCodes CurrencyCode { get; set; }
        public string FromAccountId { get; set; }
        public string ToAccountId { get; set; }
        public DateTime TransferDateTime { get; set; }
    }
}