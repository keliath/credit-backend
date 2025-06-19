using System;
using System.Linq;
using System.Threading.Tasks;
using CreditApp.Domain.Entities;
using CreditApp.Domain.Enums;
using CreditApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CreditApp.Infrastructure.Data.Seeders;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Retry para esperar a que SQL Server esté listo
        var maxRetries = 10;
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                await context.Database.MigrateAsync();
                break;
            }
            catch (Exception ex) when (i < maxRetries - 1)
            {
                Console.WriteLine($"Intento {i + 1} de conexión a SQL fallido. Esperando 5 segundos... Error: {ex.Message}");
                await Task.Delay(5000);
            }
        }

        // Seed Users
        if (!await context.Users.AnyAsync())
        {
            var users = new[]
            {
                new User(
                    username: "user1",
                    email: "user1@example.com",
                    passwordHash: BCrypt.Net.BCrypt.HashPassword("Password123!"),
                    role: UserRole.User
                ),
                new User(
                    username: "user2",
                    email: "user2@example.com",
                    passwordHash: BCrypt.Net.BCrypt.HashPassword("Password123!"),
                    role: UserRole.User
                ),
                new User(
                    username: "analyst1",
                    email: "analyst1@example.com",
                    passwordHash: BCrypt.Net.BCrypt.HashPassword("Password123!"),
                    role: UserRole.Analyst
                ),
                new User(
                    username: "analyst2",
                    email: "analyst2@example.com",
                    passwordHash: BCrypt.Net.BCrypt.HashPassword("Password123!"),
                    role: UserRole.Analyst
                ),
                new User(
                    username: "admin1",
                    email: "admin1@example.com",
                    passwordHash: BCrypt.Net.BCrypt.HashPassword("Password123!"),
                    role: UserRole.Admin
                )
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();

            var savedUsers = await context.Users.ToListAsync();
            var regularUsers = savedUsers.Where(u => u.Role == UserRole.User).ToList();
            var analysts = savedUsers.Where(u => u.Role == UserRole.Analyst).ToList();

            // Seed Credit Requests
            var creditRequests = new[]
            {
                new CreditRequest(
                    regularUsers[0].Id,
                    Money.Create(5000m, "USD"),
                    12,
                    Money.Create(3000m, "USD"),
                    2,
                    "Renovación de vivienda"
                ),
                new CreditRequest(
                    regularUsers[0].Id,
                    Money.Create(10000m, "USD"),
                    24,
                    Money.Create(3000m, "USD"),
                    2,
                    "Compra de vehículo"
                ),
                new CreditRequest(
                    regularUsers[1].Id,
                    Money.Create(15000m, "USD"),
                    36,
                    Money.Create(4000m, "USD"),
                    3,
                    "Inversión en negocio"
                )
            };

            await context.CreditRequests.AddRangeAsync(creditRequests);
            await context.SaveChangesAsync();

            var requests = await context.CreditRequests.ToListAsync();
        }
    }
}