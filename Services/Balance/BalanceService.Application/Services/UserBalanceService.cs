using System.Threading.Tasks;
using BalanceService.Domain.Entities;
using BalanceService.Domain.Interfaces;

namespace BalanceService.Application.Services
{
    public class UserBalanceService : IUserBalanceService
    {
        private readonly IUserBalanceRepository _balanceRepository;

        public UserBalanceService(IUserBalanceRepository balanceRepository)
        {
            _balanceRepository = balanceRepository;
        }

        public async Task<decimal> GetBalanceAsync(string username)
        {
            var balance = await _balanceRepository.GetByUsernameAsync(username);
            return balance?.Balance ?? 0;
        }

        public async Task<bool> DebitBalanceAsync(string username, decimal amount)
        {
            var balance = await _balanceRepository.GetByUsernameAsync(username);
            if (balance != null && balance.Balance >= amount)
            {
                balance.Balance -= amount;
                await _balanceRepository.UpdateAsync(balance);
                return true;
            }
            return false;
        }

        public async Task<bool> CreditBalanceAsync(string username, decimal amount)
        {
            var balance = await _balanceRepository.GetByUsernameAsync(username);
            if (balance != null)
            {
                balance.Balance += amount;
                await _balanceRepository.UpdateAsync(balance);
                return true;
            }

            return false;

        }

        public async Task<bool> CreateUserBalanceAsync(string username, decimal initialBalance)
        {
            var balance = await _balanceRepository.GetByUsernameAsync(username);

            if (balance != null)
                throw new Exception("User balance already saved");

            var newbalance = new UserBalance
            {
                Username = username,
                Balance = initialBalance
            };
            await _balanceRepository.AddAsync(newbalance);

            return true;
        }
    }
}
