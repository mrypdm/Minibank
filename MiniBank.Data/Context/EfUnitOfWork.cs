using System.Threading;
using System.Threading.Tasks;
using MiniBank.Core;

namespace MiniBank.Data.Context
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly MiniBankContext _context;

        public EfUnitOfWork(MiniBankContext context)
        {
            _context = context;
        }

        public Task SaveChanges(CancellationToken token)
        {
            return _context.SaveChangesAsync(token);
        }
    }
}