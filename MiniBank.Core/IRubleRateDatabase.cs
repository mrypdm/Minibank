using System.Diagnostics.Tracing;
using System.Linq;

namespace MiniBank.Core
{
    public interface IRubleRateDatabase
    {
        int Get(string currencyCode);
    }
}