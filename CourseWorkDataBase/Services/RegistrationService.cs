using BCrypt.Net;
using CourseWorkDataBase.Models;
using CourseWorkDataBase.DAL;
using Microsoft.EntityFrameworkCore;

namespace CourseWorkDataBase.Data;

public class RegistrationService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public RegistrationService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<Patient> RegisterPage(string email, string password, string firstName, string familyName, string gender)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        if (await context.Users.AnyAsync(u => u.Email == email && u.RoleId == 3))
        {
            throw new ApplicationException("Email already exists.");
        }
    
        var role = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Patient");
        if (role == null)
        {
            throw new ApplicationException("Role 'Patient' not found.");
        }
        
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        
        var user = new User
        {
            Email = email,
            Password = passwordHash,
            RoleId = role.Id,
            CreatedAt = DateTime.UtcNow
        };
    
        context.Users.Add(user);
        await context.SaveChangesAsync();
    
        var patient = new Patient
        {
            UserId = user.Id,
            FirstName = firstName,
            FamilyName = familyName,
            Gender = gender,
            Date = DateTime.UtcNow
        };
    
        context.Patients.Add(patient);
        await context.SaveChangesAsync();
        
        return patient;
    }
}