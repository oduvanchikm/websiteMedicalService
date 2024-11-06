using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CourseWorkDataBase.Models;
using CourseWorkDataBase.DAL;

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
        return await _context.Users
            .Include(u => u.Id)
            .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
    }
    
    // public async Task<Doctor> AuthenticateDoctor(string personalNumber)
    // {
    //     return await _context.Doctors
    //         .Include(d => d.Specialty)
    //         .FirstOrDefaultAsync(d => d.PersonalNumber == personalNumber);
    // }
}