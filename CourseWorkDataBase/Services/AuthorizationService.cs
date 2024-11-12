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

        if (user.RoleId == 2)
        {
            var doctor = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email && u.Role.Name == "Doctor");
            if (doctor == null)
            {
                return null;
            }
            
            bool isPasswordValid1;
        
            try
            {
                isPasswordValid1 = BCrypt.Net.BCrypt.Verify(password, doctor.PersonalNumber);
            }
            catch (SaltParseException ex)
            {
                Console.WriteLine($"Error parsing salt in Doctor: {ex.Message}");
                return null;
            }

            if (!isPasswordValid1)
            {
                return null;
            }

            return doctor;
        }

        bool isPasswordValid;
        try
        {
            isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
        }
        catch (SaltParseException ex)
        {
            Console.WriteLine($"Error parsing salt in Patient/Admin: {ex.Message}");
            return null;
        }

        if (!isPasswordValid)
        {
            Console.Out.WriteLine("Invalid Password");
            return null;
        }
        
        return user;
    }

    // public async Task<User> AuthenticateDoctor(string email, string personalNumber)
    // {
    // var doctor = await _context.Users
    //     .Include(u => u.Role)
    //     .FirstOrDefaultAsync(u => u.Email == email && u.Role.Name == "Doctor");
    // if (doctor == null)
    // {
    //     return null;
    // }
    //     
    //     // TODO 
    //     if (doctor.RoleId != 2)
    //     {
    //         return null;
    //     }
    //     
    //     bool isPasswordValid;
    //     
    //     try
    //     {
    //         isPasswordValid = BCrypt.Net.BCrypt.Verify(personalNumber, doctor.PersonalNumber);
    //     }
    //     catch (SaltParseException ex)
    //     {
    //         Console.WriteLine($"Error parsing salt in Doctor: {ex.Message}");
    //         return null;
    //     }
    //
    //     if (!isPasswordValid)
    //     {
    //         return null;
    //     }
    //
    //     return doctor;
    // }
}