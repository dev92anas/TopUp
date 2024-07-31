using SecurityService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityService.Domain.Interfaces
{
    public interface ISecurityUserInfoRepository
    {
        Task<SecurityUserInfo> GetByUsernameAsync(string username);
        Task<SecurityUserInfo> AuthenticateAsync(string username, string password);
        Task RegisterAsync(SecurityUserInfo user);
    }
}
