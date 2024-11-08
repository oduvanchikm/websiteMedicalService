using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CourseWorkDataBase.Models;
using CourseWorkDataBase.DAL;
using BCrypt.Net;

namespace CourseWorkDataBase.Data;

public class AuthorizationService
{
    private readonly ApplicationDbContext _context;

    public AuthorizationService(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User> AuthenticateUser(string email, string password)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
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
            Console.WriteLine($"Error parsing salt: {ex.Message}");
            return null;
        }

        if (!isPasswordValid)
        {
            return null;
        }


        return user;
    }

    public async Task<User> AuthenticateDoctor(string personalNumber)
    {
        var doctor = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.PersonalNumber == personalNumber && u.Role.Name == "Doctor");

        if (doctor == null)
        {
            return null;
        }

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(personalNumber, doctor.PersonalNumber);

        if (!isPasswordValid)
        {
            return null;
        }

        return doctor;
    }
}