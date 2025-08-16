using CourseWorkDataBase.DAL;
using CourseWorkDataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseWorkDataBase.Services;

public class AdminService(IDbContextFactory<ApplicationDbContext> dbContextFactory, ILogger<AdminService> logger)
{
    private readonly ILogger<AdminService> _logger = logger;

    public async Task<Doctor> AddDoctorAsync(
            string email,
            string familyName,
            string firstName,
            string personalNumber,
            long? specialtyId,
            string? specialtyName,
            string? specialtyDescription,
            long? clinicId,
            string? clinicAddress,
            string? clinicPhoneNumber
            )
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        
        if (await context.Users.AnyAsync(x => x.Email == email && x.Id == 2))
        {
            throw new ApplicationException("A user with this email already exists.");
        }
        
        var role = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Doctor");
        if (role == null)
        {
            throw new ApplicationException("Role 'Doctor' not found.");
        }
        
        var hashedPersonalNumber = BCrypt.Net.BCrypt.HashPassword(personalNumber);

        var user = new User
        {
            Email = email,
            Password = hashedPersonalNumber,
            RoleId = role.Id,
            CreatedAt = DateTime.UtcNow
        };

        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
                
            Clinic clinic;
            if (clinicId.HasValue)
            {
                clinic = await context.Clinics.FindAsync(clinicId.Value) ?? throw new InvalidOperationException();
                if (clinic == null)
                {
                    throw new ApplicationException("Clinic not found.");
                }
            }
            else
            {
                if (string.IsNullOrEmpty(clinicAddress))
                {
                    throw new ApplicationException("The clinic address is required.");
                }
                    
                clinic = await context.Clinics
                    .FirstOrDefaultAsync(s => s.Address.ToLower() == clinicAddress.ToLower()) ?? throw new InvalidOperationException();
                if (clinic != null)
                {
                    throw new ApplicationException("A clinic with this name already exists.");
                }
                
                clinic = new Clinic
                {
                    Address = clinicAddress.Trim(),
                    PhoneNumber = clinicPhoneNumber?.Trim() ?? throw new InvalidOperationException(),
                };
                    
                context.Clinics.Add(clinic);
                await context.SaveChangesAsync();
            }
                
            Specialty specialty;
            if (specialtyId.HasValue)
            {
                specialty = await context.Specialties.FindAsync(specialtyId.Value) ?? throw new InvalidOperationException();
                if (specialty == null)
                {
                    throw new ApplicationException("Specialization not found.");
                }
            }
            else
            {
                if (string.IsNullOrEmpty(specialtyName))
                {
                    throw new ApplicationException("The specialty name is required if specialtyId is not provided.");
                }
                    
                specialty = await context.Specialties
                    .FirstOrDefaultAsync(s => s.NameSpecialty.ToLower() == specialtyName.ToLower()) ?? throw new InvalidOperationException();
                if (specialty != null)
                {
                    throw new ApplicationException("A specialization with this name already exists.");
                }
                
                specialty = new Specialty
                {
                    NameSpecialty = specialtyName.Trim(),
                    Description = specialtyDescription?.Trim(),
                };
                
                context.Specialties.Add(specialty);
                await context.SaveChangesAsync();
            }
                
            var doctor = new Doctor
            {
                UserId = user.Id,
                ClinicId = clinic.Id,
                FirstName = firstName,
                FamilyName = familyName,
                SpecialtyID = specialty.Id
            };

            context.Doctors.Add(doctor);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return doctor;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<Doctor>> GetAllDoctorsAsync()
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var doctors = await context.Doctors
            .Include(d => d.Specialty)
            .Include(d => d.Clinic)
            .ToListAsync();
    
        return doctors;
    }
}