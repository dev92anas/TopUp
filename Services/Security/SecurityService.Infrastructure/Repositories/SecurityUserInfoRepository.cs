using Microsoft.EntityFrameworkCore;
using SecurityService.Domain.Entities;
using SecurityService.Domain.Interfaces;
using SecurityService.Infrastructure.Data;

namespace SecurityService.Infrastructure.Repositories
{
    public class SecurityUserInfoRepository : ISecurityUserInfoRepository
    {
        private readonly SecurityDbContext _context;

        public SecurityUserInfoRepository(SecurityDbContext context)
        {
            _context = context;
        }

        public async Task<SecurityUserInfo> GetByUsernameAsync(string username)
        {
            return await _context.SecurityUserInfos.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<SecurityUserInfo> AuthenticateAsync(string username, string password)
        {
            return await _context.SecurityUserInfos.FirstOrDefaultAsync(u => u.Username == username && u.IsActive == true);
        }

        public async Task RegisterAsync(SecurityUserInfo user)
        {
            await _context.SecurityUserInfos.AddAsync(user);
            await _context.SaveChangesAsync();
        }
    }
}
