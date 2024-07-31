using BalanceService.Domain.Entities;
using BalanceService.Domain.Interfaces;
using BalanceService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BalanceService.Infrastructure.Repositories
{
    public class UserBalanceRepository : IUserBalanceRepository
    {
        private readonly BalanceDbContext _context;

        public UserBalanceRepository(BalanceDbContext context)
        {
            _context = context;
        }

        public async Task<UserBalance> GetByUsernameAsync(string username)
        {
            return await _context.UserBalances.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task AddAsync(UserBalance balance)
        {
            await _context.UserBalances.AddAsync(balance);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserBalance balance)
        {
            _context.UserBalances.Update(balance);
            await _context.SaveChangesAsync();
        }
    }
}
