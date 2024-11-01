using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CourseWorkDataBase.Models;

namespace CourseWorkDataBase.Data;

public class RegistrationService
{
    private readonly ApplicationDbContext _context;

    public RegistrationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Patient> RegisterPage(string email, string password, string firstName, string familyName, string contactInfo, string gender)
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

        var user = new User
        {
            Email = email,
            Password = password,
            RoleId = role.Id,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var patient = new Patient
        {
            UserID = user.Id,
            FirstName = firstName,
            FamilyName = familyName,
            ContactInfo = contactInfo,
            Gender = gender
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();
        
        return patient;
    }

}