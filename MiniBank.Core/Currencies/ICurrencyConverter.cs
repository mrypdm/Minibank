namespace MiniBank.Core.Currencies
{
    public interface ICurrencyConverter
    {
        double Convert(double amount, CurrencyCodes fromCurrency, CurrencyCodes toCurrency);
    }
}