using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiniBank.Core;
using MiniBank.Core.BankAccounts.Repositories;
using MiniBank.Core.Currencies;
using MiniBank.Core.Transfers.Repositories;
using MiniBank.Core.Users.Repositories;
using MiniBank.Data.BankAccounts.Repositories;
using MiniBank.Data.Context;
using MiniBank.Data.Currencies;
using MiniBank.Data.Transfers.Repositories;
using MiniBank.Data.Users.Repositories;

namespace MiniBank.Data
{
    public static class Bootstrap
    {
        public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<ICurrencyRateProvider, CurrencyRateProvider>(options =>
            {
                options.BaseAddress = new Uri(configuration["CurrencyRateSourceUrl"]);
            });

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBankAccountRepository, BankAccountRepository>();
            services.AddScoped<ITransferRepository, TransferRepository>();

            services.AddScoped<IUnitOfWork, EfUnitOfWork>();

            services.AddDbContext<MiniBankContext>(op => op.UseNpgsql(configuration["PostgreSqlConnectionString"]));

            return services;
        }

        public static IApplicationBuilder MigrateDatabase(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<MiniBankContext>();
            dbContext.Database.Migrate();
            return app;
        }
    }
}