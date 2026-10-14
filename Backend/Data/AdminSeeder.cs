using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ZBooksApi.Models;

namespace ZBooksApi.Data;

public static class AdminSeeder
{
    public static async Task SeedAsync(
        ZBooksDbContext context,
        IConfiguration configuration)
    {
        var adminExists = await context.Users
            .AnyAsync(u => u.Role == "Admin");

        if (adminExists)
            return;

        var email = configuration["Admin:Email"];
        var password = configuration["Admin:Password"];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "Admin credentials are not configured.");
        }

        var admin = new User
        {
            FullName = "ZBOOKS Admin",
            Email = email,
            Role = "Admin",
            CreatedAt = DateTime.UtcNow
        };

        var passwordHasher = new PasswordHasher<User>();

        admin.PasswordHash = passwordHasher.HashPassword(
            admin,
            password);

        context.Users.Add(admin);

        await context.SaveChangesAsync();
    }
}