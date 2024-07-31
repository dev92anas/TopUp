using SecurityService.Domain.Entities;

namespace SecurityService.Application.Services
{
    public interface IUserService
    {
        Task<SecurityUserInfo> AuthenticateAsync(string username, string password);
        Task<SecurityUserInfo> RegisterAsync(string username, string password);
    }
}
