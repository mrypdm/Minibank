using System;
using Microsoft.Extensions.DependencyInjection;
using MiniBank.Core.BankAccounts.Repositories;
using MiniBank.Core.Currencies;
using MiniBank.Core.Transfers.Repositories;
using MiniBank.Core.Users.Repositories;
using MiniBank.Data.BankAccounts.Repositories;
using MiniBank.Data.Currencies;
using MiniBank.Data.Transfers.Repositories;
using MiniBank.Data.Users.Repositories;

namespace MiniBank.Data
{
    public static class Bootstrap
    {
        public static IServiceCollection AddData(this IServiceCollection services)
        {
            services.AddHttpClient<ICurrencyRateProvider, CurrencyRateProvider>(options =>
            {
                options.BaseAddress = new Uri("https://www.cbr-xml-daily.ru/");
            });

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBankAccountRepository, BankAccountRepository>();
            services.AddScoped<ITransferRepository, TransferRepository>();

            return services;
        }
    }
}