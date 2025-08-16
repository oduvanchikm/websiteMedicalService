using Microsoft.EntityFrameworkCore;
using CourseWorkDataBase.Models;
using CourseWorkDataBase.DAL;
using BCrypt.Net;

namespace CourseWorkDataBase.Services;

public class AuthorizationService(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    ILogger<AuthorizationService> logger)
{
    public async Task<User> AuthenticateUser(string email, string password)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var user = await context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            logger.LogWarning("Authentication failed: User with email {Email} not found.", email);
            return null;
        }
        
        bool isPasswordValid;
        try
        {
            isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
        }
        catch (SaltParseException ex)
        {
            logger.LogError(ex, "Error parsing salt for user with email {Email}", email);
            return null;
        }

        if (isPasswordValid) return user;
        logger.LogWarning("Authentication failed: Invalid password for user with email {Email}", email);
        return null;

    }
}