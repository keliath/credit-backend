using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using CreditApp.Domain.ValueObjects;
using CreditApp.Domain.Entities;

namespace CreditApp.Infrastructure.Data.Configurations
{
    public static class MoneyConfiguration
    {
        public static void ConfigureMoney(this ModelBuilder modelBuilder)
        {
            var moneyConverter = new ValueConverter<Money, decimal>(
                v => v.Amount,
                v => Money.Create(v, "USD"));

            modelBuilder.Entity<CreditRequest>()
                .Property(c => c.Amount)
                .HasConversion(moneyConverter)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<CreditRequest>()
                .Property(c => c.MonthlyIncome)
                .HasConversion(moneyConverter)
                .HasColumnType("decimal(18,2)");
        }
    }
} 