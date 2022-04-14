using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MiniBank.Core.Exceptions;
using MiniBank.Core.Users;
using MiniBank.Core.Users.Repositories;
using MiniBank.Data.Context;
using MiniBank.Data.Users.Mappers;

namespace MiniBank.Data.Users.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MiniBankContext _context;

        public UserRepository(MiniBankContext context)
        {
            _context = context;
        }

        public Task<bool> ExistsWithId(string id, CancellationToken token)
        {
            return _context.Users.AnyAsync(u => u.Id == id, token);
        }

        public async Task<User> GetById(string id, CancellationToken token)
        {
            var dbModel = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, token);

            if (dbModel == null)
            {
                throw new ValidationException($"User with id {id} doesn't exist");
            }

            return dbModel.ToModel();
        }

        public async Task Create(User user, CancellationToken token)
        {
            await _context.Users.AddAsync(user.ToDbModel(), token);
        }

        public async Task Update(User user, CancellationToken token)
        {
            var dbModel = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id, token);

            if (dbModel == null)
            {
                throw new ValidationException($"User with id {user.Id} doesn't exist");
            }

            dbModel.Login = user.Login;
            dbModel.Email = user.Email;
        }

        public async Task DeleteById(string id, CancellationToken token)
        {
            var dbModel = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, token);

            if (dbModel == null)
            {
                throw new ValidationException($"User with id {id} doesn't exist");
            }

            _context.Users.Remove(dbModel);
        }
    }
}