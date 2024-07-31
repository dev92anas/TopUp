using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopUp.Application.ExternalServices
{
    public interface IExternalBalanceService
    {
        Task<decimal> GetBalanceAsync(string username);
        Task<bool> DebitBalanceAsync(string username, decimal amount);
        Task<bool> CreditBalanceAsync(string username, decimal amount);
    }
}
