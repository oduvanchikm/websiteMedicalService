using BCrypt.Net;
using CourseWorkDataBase.Models;
using CourseWorkDataBase.DAL;
using Microsoft.EntityFrameworkCore;

namespace CourseWorkDataBase.Data;

public class RegistrationService
{
    private readonly ApplicationDbContext _context;

    public RegistrationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Patient> RegisterPage(string email, string password, string firstName, string familyName, string gender)
    {
        if (await _context.Users.AnyAsync(u => u.Email == email))
        {
            throw new ApplicationException("Email already exists.");
        }
    
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Patient");
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
    
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    
        var patient = new Patient
        {
            UserId = user.Id,
            FirstName = firstName,
            FamilyName = familyName,
            Gender = gender,
            Date = DateTime.UtcNow
        };
    
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();
        
        return patient;
    }
}