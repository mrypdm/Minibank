using System;
using MiniBank.Core.Currencies;

namespace MiniBank.Web.Controllers.BankAccounts.Dto
{
    public class BankAccountCreateDto
    {
        public string UserId { get; set; }
        public CurrencyCodes CurrencyCode { get; set; }
        public double Amount { get; set; }
    }

    public class BankAccountDto
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public double Amount { get; set; }
        public CurrencyCodes CurrencyCode { get; set; }
        public bool IsClosed { get; set; }
        public DateTime OpeningDate { get; set; }
        public DateTime? ClosingDate { get; set; }
    }
}