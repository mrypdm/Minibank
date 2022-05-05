using MiniBank.Core.Transfers;

namespace MiniBank.Data.Transfers.Mappers
{
    public static class TransferMappers
    {
        public static Transfer ToModel(this TransferDbModel model)
        {
            return new Transfer
            {
                Id = model.Id,
                Amount = model.Amount,
                CurrencyCode = model.CurrencyCode,
                FromAccountId = model.FromAccountId,
                ToAccountId = model.ToAccountId,
                TransferDateTime = model.TransferDateTime
            };
        }

        public static TransferDbModel ToDbModel(this Transfer model)
        {
            return new TransferDbModel
            {
                Id = model.Id,
                Amount = model.Amount,
                CurrencyCode = model.CurrencyCode,
                FromAccountId = model.FromAccountId,
                ToAccountId = model.ToAccountId,
                TransferDateTime = model.TransferDateTime
            };
        }
    }
}