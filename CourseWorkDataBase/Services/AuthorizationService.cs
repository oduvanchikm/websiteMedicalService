using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CourseWorkDataBase.Models;
using CourseWorkDataBase.DAL;
using BCrypt.Net;

namespace CourseWorkDataBase.Data;

public class AuthorizationService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly ILogger<AuthorizationService> _logger;

    public AuthorizationService(IDbContextFactory<ApplicationDbContext> dbContextFactory, ILogger<AuthorizationService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    public async Task<User> AuthenticateUser(string email, string password)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var user = await context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            _logger.LogWarning("Authentication failed: User with email {Email} not found.", email);
            return null;
        }

        Console.WriteLine($"Stored Password Hash: {user.Password}");

        bool isPasswordValid;
        try
        {
            isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
        }
        catch (SaltParseException ex)
        {
            _logger.LogError(ex, "Error parsing salt for user with email {Email}", email);
            Console.WriteLine($"Error parsing salt in Patient/Admin: {ex.Message}");
            return null;
        }

        if (!isPasswordValid)
        {
            _logger.LogWarning("Authentication failed: Invalid password for user with email {Email}", email);
            Console.Out.WriteLine("Invalid Password");
            return null;
        }
        
        return user;
    }
}