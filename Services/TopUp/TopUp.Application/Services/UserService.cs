using TopUp.Application.DTOs;
using TopUp.Application.ExternalServices;
using TopUp.Application.Services.IServices;
using TopUp.Domain.Entities;
using TopUp.Domain.Interfaces;
using TopUp.Application.Enums;
using Microsoft.Extensions.Logging;

namespace TopUpService.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;

        public UserService(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task AddUserAsync(string username)
        {
            try
            {
                await _userRepository.AddAsync(new User {Username = username, IsVerified = false });
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the the topup user.");
            }
        }
    }
}
