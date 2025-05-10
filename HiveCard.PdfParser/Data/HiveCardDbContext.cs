using Microsoft.EntityFrameworkCore;
using HiveCard.PdfParser.Models;

namespace HiveCard.PdfParser.Data
{
    public class HiveCardDbContext : DbContext
    {
        public DbSet<CreditCardAccount> Accounts { get; set; }
        public DbSet<AccountSummary> AccountSummaries { get; set; }
        public DbSet<TransactionDetail> TransactionDetails { get; set; }
        public DbSet<InstallmentDetail> InstallmentDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options) =>
            options.UseSqlServer("Your_Connection_String");

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<CreditCardAccount>()
              .HasOne(a => a.Summary)
              .WithOne(s => s.CreditCardAccount)
              .HasForeignKey<AccountSummary>(s => s.CreditCardAccountId);

            mb.Entity<CreditCardAccount>()
              .HasMany(a => a.Details)
              .WithOne(d => d.CreditCardAccount)
              .HasForeignKey(d => d.CreditCardAccountId);

            mb.Entity<CreditCardAccount>()
              .HasMany(a => a.Installments)
              .WithOne(i => i.CreditCardAccount)
              .HasForeignKey(i => i.CreditCardAccountId);
        }
    }
}
