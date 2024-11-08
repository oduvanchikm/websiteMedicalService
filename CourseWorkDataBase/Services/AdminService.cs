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
            string firstName)
    {
        if (await _context.Users.AnyAsync(x => x.Email == email))
        {
            throw new ApplicationException("Пользователь с таким Email уже существует.");
        }
        
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Doctor");
        if (role == null)
        {
            throw new ApplicationException("Role 'Doctor' not found.");
        }

        var personalNumber = GeneratePersonalNumber.GenerateRandomNumber();
        Console.Out.WriteLine(personalNumber);
        var hashedPersonalNumber = BCrypt.Net.BCrypt.HashPassword(personalNumber);
        Console.Out.WriteLine(hashedPersonalNumber);
        
        // var roleId = await _context.Roles
        //     .Where(r => r.Name == "Doctor")
        //     .Select(r => r.Id)
        //     .FirstOrDefaultAsync();
        
        Console.Out.WriteLine(role.Id + " " + role.Name);

        var user = new User
        {
            Email = email,
            PersonalNumber = hashedPersonalNumber,
            RoleId = role.Id,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        Console.Out.WriteLine("ID USER AAAAAA" + user.Id);

        var doctor = new Doctor
        {
            UserId = user.Id,
            FirstName = firstName,
            FamilyName = familyName,
            SpecialtyID = 1
        };

        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync();

        return doctor;
    }

    public async Task<List<Doctor>> GetAllDoctorsAsync()
    {
        return await _context.Doctors.Include(d => d.User).ToListAsync();
    }
}