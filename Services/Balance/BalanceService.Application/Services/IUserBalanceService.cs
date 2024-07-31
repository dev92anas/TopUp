using BalanceService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalanceService.Application.Services
{
    public interface IUserBalanceService
    {
        Task<decimal> GetBalanceAsync(string username);
        Task<bool> DebitBalanceAsync(string username, decimal amount);
        Task<bool> CreditBalanceAsync(string username, decimal amount);
        Task<bool> CreateUserBalanceAsync(string username, decimal initialBalance);
    }
}
