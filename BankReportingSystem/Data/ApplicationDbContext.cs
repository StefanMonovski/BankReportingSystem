using BankReportingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BankReportingSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Partner> Partners { get; set; }
        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Partner>(entity =>
            {
                entity.HasIndex(p => p.Name)
                    .IsUnique();

                entity.HasMany(p => p.Merchants)
                    .WithOne(m => m.Partner)
                    .HasForeignKey(m => m.PartnerId);
            });

            modelBuilder.Entity<Merchant>(entity =>
            {
                entity.HasIndex(m => m.Name)
                    .IsUnique();

                entity.HasMany(m => m.Transactions)
                    .WithOne(t => t.Merchant)
                    .HasForeignKey(t => t.MerchantId);
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasIndex(t => t.ExternalId)
                    .IsUnique();

                entity.Property(t => t.Direction)
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(t => t.Status)
                    .HasConversion<string>()
                    .IsRequired();
            });
        }
    }
}
