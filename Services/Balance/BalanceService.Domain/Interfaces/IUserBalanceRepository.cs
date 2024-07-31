using BalanceService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalanceService.Domain.Interfaces
{
    public interface IUserBalanceRepository
    {
        Task<UserBalance> GetByUsernameAsync(string username);
        Task AddAsync(UserBalance balance);
        Task UpdateAsync(UserBalance balance);
    }
}
