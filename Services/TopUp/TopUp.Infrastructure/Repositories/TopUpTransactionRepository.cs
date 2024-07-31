using Microsoft.EntityFrameworkCore;
using TopUp.Domain.Entities;
using TopUp.Domain.Interfaces;
using TopUp.Infrastructure.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TopUp.Infrastructure.Repositories
{
    public class TopUpTransactionRepository : Repository<TopUpTransaction>, ITopUpTransactionRepository
    {
        public TopUpTransactionRepository(TopUpDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TopUpTransaction>> GetBeneficiaryAndUserTransactionsAsync(int userId, int beneficiaryId, DateTime Date)
        {
            return await _context.TopUpTransactions
                .Where(t => t.UserId == userId && t.BeneficiaryId == beneficiaryId && t.Date >= Date)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalTopUpForUserAsync(int userId, DateTime Date)
        {
            return await _context.TopUpTransactions
                .Where(t => t.UserId == userId && t.Date >= Date)
                .SumAsync(t => t.Amount);
        }

        public async Task<TopUpTransaction> GetByIdempotencyKeyAsync(string IdempotencyKey) {
            return await _context.TopUpTransactions
                    .Where(t => t.IdempotencyKey == IdempotencyKey)
                    .FirstOrDefaultAsync();
        }

    }
}
