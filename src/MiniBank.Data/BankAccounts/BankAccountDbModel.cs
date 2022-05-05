using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniBank.Core.Currencies;

namespace MiniBank.Data.BankAccounts
{
    public class BankAccountDbModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public double Amount { get; set; }
        public CurrencyCodes CurrencyCode { get; set; }
        public bool IsClosed { get; set; }
        public DateTime OpeningDate { get; set; }
        public DateTime? ClosingDate { get; set; }

        internal class Map : IEntityTypeConfiguration<BankAccountDbModel>
        {
            public void Configure(EntityTypeBuilder<BankAccountDbModel> builder)
            {
                builder.ToTable("bank_account");
                
                builder.Property(it => it.CurrencyCode).HasConversion<string>();

                builder.HasKey(it => it.Id).HasName("pk_bank_account_id");
            }
        }
    }
}