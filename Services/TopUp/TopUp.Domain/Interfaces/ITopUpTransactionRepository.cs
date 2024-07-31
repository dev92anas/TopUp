using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopUp.Domain.Entities;

namespace TopUp.Domain.Interfaces
{
    public interface ITopUpTransactionRepository : IRepository<TopUpTransaction>
    {
        Task<IEnumerable<TopUpTransaction>> GetBeneficiaryAndUserTransactionsAsync(int userId, int beneficiaryId, DateTime Date);
        Task<TopUpTransaction> GetByIdempotencyKeyAsync(string IdempotencyKey);
        Task<decimal> GetTotalTopUpForUserAsync(int userId, DateTime Date);
    }
}
