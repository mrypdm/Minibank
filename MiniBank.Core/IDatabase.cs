using System.Diagnostics.Tracing;
using System.Linq;

namespace MiniBank.Core
{
    public interface IDatabase
    {
        int Get(string key);
    }
}