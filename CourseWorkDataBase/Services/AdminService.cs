using CourseWorkDataBase.DAL;
using CourseWorkDataBase.Models;
using CourseWorkDataBase.Helpers;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace CourseWorkDataBase.Data;

public class AdminService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AdminService> _logger;

    public AdminService(ApplicationDbContext context, ILogger<AdminService> logger)
    {
        _context = context;
        _logger = logger;
    }

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
        if (await _context.Users.AnyAsync(x => x.Email == email))
        {
            throw new ApplicationException("\nA user with this email already exists.");
        }
        
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Doctor");
        if (role == null)
        {
            throw new ApplicationException("Role 'Doctor' not found.");
        }

        // var personalNumber = GeneratePersonalNumber.GenerateRandomNumber();
        // Console.Out.WriteLine(personalNumber);
        
        var hashedPersonalNumber = BCrypt.Net.BCrypt.HashPassword(personalNumber);
        Console.Out.WriteLine(hashedPersonalNumber);

        Console.Out.WriteLine(role.Id + " " + role.Name);

        var user = new User
        {
            Email = email,
            PersonalNumber = hashedPersonalNumber,
            RoleId = role.Id,
            CreatedAt = DateTime.UtcNow
        };

        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                Console.Out.WriteLine("ID USER AAAAAA" + user.Id);
                
                Clinic clinic;
                if (clinicId.HasValue)
                {
                    clinic = await _context.Clinics.FindAsync(clinicId.Value);
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
                    
                    clinic = await _context.Clinics
                        .FirstOrDefaultAsync(s => s.Address.ToLower() == clinicAddress.ToLower());
                    if (clinic != null)
                    {
                        throw new ApplicationException("A clinic with this name already exists.");
                    }
                
                    clinic = new Clinic
                    {
                        Address = clinicAddress.Trim(),
                        PhoneNumber = clinicPhoneNumber?.Trim()
                    };
                    
                    Console.Out.WriteLine("A new clinic has created with ID: " + clinic.Id);
                
                    _context.Clinics.Add(clinic);
                    await _context.SaveChangesAsync();
                    Console.Out.WriteLine("A new clinic has created with ID: " + clinic.Id);
                }
                
                Specialty specialty;
                if (specialtyId.HasValue)
                {
                    specialty = await _context.Specialties.FindAsync(specialtyId.Value);
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
                    
                    specialty = await _context.Specialties
                        .FirstOrDefaultAsync(s => s.NameSpecialty.ToLower() == specialtyName.ToLower());
                    if (specialty != null)
                    {
                        throw new ApplicationException("A specialization with this name already exists.");
                    }
                
                    specialty = new Specialty
                    {
                        ClinicId = clinic.Id,
                        NameSpecialty = specialtyName.Trim(),
                        Description = specialtyDescription?.Trim()
                    };
                
                    _context.Specialties.Add(specialty);
                    await _context.SaveChangesAsync();
                    Console.Out.WriteLine("A new specialization has created with ID: " + specialty.Id);
                }
                
                var doctor = new Doctor
                {
                    UserId = user.Id,
                    FirstName = firstName,
                    FamilyName = familyName,
                    SpecialtyID = specialty.Id
                };

                _context.Doctors.Add(doctor);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return doctor;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw;
            }    
        }
    }

    public async Task<List<Doctor>> GetAllDoctorsAsync()
    {
        return await _context.Doctors.Include(d => d.User).ToListAsync();
    }
}