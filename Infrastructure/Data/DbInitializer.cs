using Microsoft.EntityFrameworkCore;
using RealEstateAPI.Domain.Entities;
using RealEstateAPI.Domain.Enums;

namespace RealEstateAPI.Infrastructure.Data;

/// <summary>
/// Database initializer and seeder
/// </summary>
public static class DbInitializer
{
    /// <summary>
    /// Seed initial data for development/testing
    /// </summary>
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await context.Database.MigrateAsync();

        // Seed Admin if not exists
        var adminExists = await context.Users.AnyAsync(u => u.Email == "admin@realestate.com");

        if (!adminExists)
        {
            var adminUser = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@realestate.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                PhoneNumber = "+1234567890",
                Role = UserRole.Admin,
                IsActive = true
            };

            await context.Users.AddAsync(adminUser);
        }

        // Seed Regular Users
    
        await context.SaveChangesAsync();
    }
}
