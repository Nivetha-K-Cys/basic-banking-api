using System.Reflection.Emit;
using BasicBankingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BasicBankingAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<BankAccount> Accounts => Set<BankAccount>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BankAccount>()
            .HasIndex(account => account.AccountNumber)
            .IsUnique();

        modelBuilder.Entity<BankAccount>()
            .Property(account => account.Balance)
            .HasPrecision(18, 2);
    }
}