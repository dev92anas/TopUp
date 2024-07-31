using TopUp.Domain.Entities;
using TopUp.Domain.Interfaces;

namespace TopUp.Domain.Interfaces
{

    public interface ILookupsRepository : IRepository<Lookup>
    {
        Task<List<Lookup>> GetLookupsByGroupIdAsync(int GroupId);
        Task<Lookup> GetByLookupIdAsync(int lookupId);
    }

}
