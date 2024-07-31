using BalanceService.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace BalanceService.Infrastructure.Data
{
    public class BalanceDbContext : DbContext
    {
        public BalanceDbContext(DbContextOptions<BalanceDbContext> options) : base(options) { }

        public DbSet<UserBalance> UserBalances { get; set; }
    }
}
