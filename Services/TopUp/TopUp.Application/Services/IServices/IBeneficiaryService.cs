using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopUp.Application.DTOs;
using TopUp.Domain.Entities;

namespace TopUp.Application.Services.IServices
{
    public interface IBeneficiaryService
    {
        Task<IEnumerable<Beneficiary>> GetBeneficiariesAsync(string username);
        Task AddBeneficiaryAsync(string username, string nickname, string phoneNumber);
        Task TopUpBeneficiaryAsync(string username, int beneficiaryId, int lookupAmountValueId, string idempotencyKey);

    }
}
