using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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

        internal class Map : IEntityTypeConfiguration<TransferDbModel>
        {
            public void Configure(EntityTypeBuilder<TransferDbModel> builder)
            {
                builder.ToTable("transfer");
                
                builder.Property(it => it.CurrencyCode).HasConversion<string>();

                builder.HasKey(it => it.Id).HasName("pk_transfer_id");
            }
        }
    }
}