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
        string firstName,
        string familyName,
        long? specialtyId,
        string clinicAddress,
        string clinicPhoneNumber)
    {
        if (await _context.Users.AnyAsync(x => x.Email == email))
        {
            throw new ApplicationException("Пользователь с таким Email уже существует.");
        }

        string personalNumber = GeneratePersonalNumber.GenerateRandomNumber();

        var user = new User
        {
            Email = email,
            PersonalNumber = BCrypt.Net.BCrypt.HashPassword(personalNumber),
            RoleId = await _context.Roles
                .Where(r => r.Name == "Doctor")
                .Select(r => r.Id)
                .FirstOrDefaultAsync(),
            CreatedAt = DateTime.UtcNow
        };

        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                Specialty specialty = null;
                if (specialtyId.HasValue)
                {
                    specialty = await _context.Specialties.FindAsync(specialtyId.Value);
                    if (specialty == null)
                    {
                        throw new ApplicationException("Специализация не найдена.");
                    }
                }

                Clinic clinic = new Clinic
                {
                    Address = clinicAddress,
                    PhoneNumber = clinicPhoneNumber
                };

                _context.Clinics.Add(clinic);
                await _context.SaveChangesAsync();

                specialty.ClinicId = clinic.Id;
                await _context.SaveChangesAsync();

                var doctor = new Doctor
                {
                    ID = user.Id,
                    FirstName = firstName,
                    FamilyName = familyName,
                    SpecialtyID = specialty.Id
                };

                _context.Doctors.Add(doctor);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return doctor;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Ошибка при добавлении врача.");
                throw;
            }
        }
    }

    public async Task<List<Doctor>> GetAllDoctorsAsync()
    {
        return await _context.Doctors.Include(d => d.User).ToListAsync();
    }
}