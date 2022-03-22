using MiniBank.Core.BankAccounts;

namespace MiniBank.Data.BankAccounts.Mappers
{
    public static class BankAccountsMappers
    {
        public static BankAccount ToModel(this BankAccountDbModel model)
        {
            return new BankAccount
            {
                Id = model.Id,
                UserId = model.UserId,
                Amount = model.Amount,
                CurrencyCode = model.CurrencyCode,
                OpeningDate = model.OpeningDate,
                ClosingDate = model.ClosingDate,
                IsClosed = model.IsClosed,
            };
        }

        public static BankAccountDbModel ToDbModel(this BankAccount model)
        {
            return new BankAccountDbModel
            {
                Id = model.Id,
                UserId = model.UserId,
                Amount = model.Amount,
                CurrencyCode = model.CurrencyCode,
                OpeningDate = model.OpeningDate,
                ClosingDate = model.ClosingDate,
                IsClosed = model.IsClosed,
            };
        }
    }
}