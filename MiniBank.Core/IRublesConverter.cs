namespace MiniBank.Core
{
    public interface IRublesConverter
    {
        int Convert(int amount, string currencyCode);
    }
}