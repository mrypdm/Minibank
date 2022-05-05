using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using MiniBank.Core.BankAccounts.Services;
using MiniBank.Core.Currencies;
using MiniBank.Core.DateTimes;
using MiniBank.Core.Users.Services;

namespace MiniBank.Core
{
    public static class Bootstrap
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            services.AddScoped<ICurrencyConverter, CurrencyConverter>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBankAccountService, BankAccountService>();
            services.AddScoped<IDateTimeProvider, DateTimeProvider>();

            services.AddFluentValidation().AddValidatorsFromAssembly(typeof(Bootstrap).Assembly);

            return services;
        }
    }
}