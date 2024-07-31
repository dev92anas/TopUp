using Microsoft.EntityFrameworkCore;
using TopUp.Domain.Entities;
using TopUp.Domain.Interfaces;
using TopUp.Infrastructure.Data;

namespace TopUp.Infrastructure.Repositories
{
    public class BeneficiaryRepository : Repository<Beneficiary>, IBeneficiaryRepository
    {
        public BeneficiaryRepository(TopUpDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Beneficiary>> GetAllBeneficiaryForUserAsync(int userId)
        {
            return await _context.Beneficiaries
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }
    }
}
