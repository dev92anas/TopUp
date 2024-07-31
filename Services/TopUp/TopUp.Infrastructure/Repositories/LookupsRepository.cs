using Microsoft.EntityFrameworkCore;
using TopUp.Domain.Entities;
using TopUp.Domain.Interfaces;
using TopUp.Infrastructure.Data;

namespace TopUp.Infrastructure.Repositories
{
    public class LookupsRepository : Repository<Lookup>, ILookupsRepository
    {
        public LookupsRepository(TopUpDbContext context) : base(context)
        {
        }

        public async Task<List<Lookup>> GetLookupsByGroupIdAsync(int GroupId)
        {
            return await _context.Lookups
                .Where(t => t.GroupId == GroupId)
                .ToListAsync();
        }

        public async Task<Lookup> GetByLookupIdAsync(int lookupId)
        {
            return await _context.Lookups
                .Where(t => t.LookupId == lookupId)
                .FirstOrDefaultAsync();
        }

        
    }
}
