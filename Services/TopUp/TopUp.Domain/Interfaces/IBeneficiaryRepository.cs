using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopUp.Domain.Entities;

namespace TopUp.Domain.Interfaces
{
    public interface IBeneficiaryRepository : IRepository<Beneficiary>
    {
        Task<IEnumerable<Beneficiary>> GetAllBeneficiaryForUserAsync(int userId);
    }
}
