using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using SecurityService.Application.ExternalServices;
using SecurityService.Domain.Entities;
using SecurityService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityService.Application.Services
{
    public class UserService : IUserService
    {
        private readonly ISecurityUserInfoRepository _userRepository;
        private readonly IExternalServices _externalServices;

        public UserService(ISecurityUserInfoRepository userRepository, IExternalServices externalServices)
        {
            _userRepository = userRepository;
            _externalServices = externalServices;
        }

        public async Task<SecurityUserInfo> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.AuthenticateAsync(username, password);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return null;
            }

            return new SecurityUserInfo
            {
                Id = user.Id,
                Username = user.Username,
                IsActive = user.IsActive
            };
        }

        public async Task<SecurityUserInfo> RegisterAsync(string username, string password)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(username);

            if (existingUser != null)
            {
                throw new Exception("Username already taken");
            }

            var user = new SecurityUserInfo
            {
                Username = username,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                IsActive = true
            };

            await _userRepository.RegisterAsync(user);

            // I recommend to call Async Message Queue service, to complete the registration and add user records on the
            // other services, then send the user an email after the registration completed successfully
            
            // Call ExternalServices to add the user record to BalanceService
            await _externalServices.AddUserBalanceAsync(username);

            // Call ExternalServices to add the user record to TopUpService
            await _externalServices.AddUserTopUpAsync(username, false);

            return new SecurityUserInfo
            {
                Id = user.Id,
                Username = user.Username,
                IsActive = user.IsActive
            };
        }
    }

}
