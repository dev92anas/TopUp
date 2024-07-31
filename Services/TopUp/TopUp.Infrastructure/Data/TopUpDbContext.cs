using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TopUp.Domain.Entities;

namespace TopUp.Infrastructure.Data
{
    public class TopUpDbContext : DbContext
    {
        public TopUpDbContext(DbContextOptions<TopUpDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Beneficiary> Beneficiaries { get; set; }
        public DbSet<Lookup> Lookups { get; set; }
        public DbSet<TopUpTransaction> TopUpTransactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
            {
                optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            {
                modelBuilder.Entity<User>()
                 .HasKey(u => u.Id);

                modelBuilder.Entity<User>()
                    .Property(u => u.Id)
                    .ValueGeneratedOnAdd();

                modelBuilder.Entity<Beneficiary>()
                    .HasKey(b => b.Id);

                modelBuilder.Entity<Beneficiary>()
                    .Property(b => b.Id)
                    .ValueGeneratedOnAdd();

                modelBuilder.Entity<Lookup>()
                    .HasKey(l => l.Id);

                modelBuilder.Entity<Lookup>()
                    .Property(l => l.Id)
                    .ValueGeneratedOnAdd();

                modelBuilder.Entity<TopUpTransaction>()
                    .HasKey(t => t.Id);

                modelBuilder.Entity<TopUpTransaction>()
                    .Property(t => t.Id)
                    .ValueGeneratedOnAdd();
            }
        }
    }
}
