using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MiniBank.Data.BankAccounts;
using MiniBank.Data.Transfers;
using MiniBank.Data.Users;

namespace MiniBank.Data.Context
{
    public class MiniBankContext : DbContext
    {
        public DbSet<UserDbModel> Users { get; set; }
        public DbSet<BankAccountDbModel> BankAccounts { get; set; }
        public DbSet<TransferDbModel> Transfers { get; set; }

        public MiniBankContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MiniBankContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }

    public class MiniBankContextFactory : IDesignTimeDbContextFactory<MiniBankContext>
    {
        public MiniBankContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder()
                .UseNpgsql("FakeConnectionStringForMigrationsCreation")
                .UseSnakeCaseNamingConvention()
                .Options;
    
            return new MiniBankContext(options);
        }
    }
}