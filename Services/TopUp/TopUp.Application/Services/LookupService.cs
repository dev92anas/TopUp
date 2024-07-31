using TopUp.Application.Enums;
using TopUp.Application.Services.IServices;
using TopUp.Domain.Entities;
using TopUp.Domain.Interfaces;

namespace TopUp.Application.Services
{
    public class LookupService : ILookupService
    {
        private readonly ILookupsRepository _lookupRepository;

        public LookupService(ILookupsRepository lookupRepository)
        {
            _lookupRepository = lookupRepository;
        }

        public async Task<List<Lookup>> GetTopUpOptionsAsync()
        {
            return  await _lookupRepository.GetLookupsByGroupIdAsync((int)LookupsGroups.TopUpOptions);
        }
    }
}
