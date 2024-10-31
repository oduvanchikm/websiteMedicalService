using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseWorkDataBase.Services;
// using Microsoft.IdentityModel.Tokens;
// using System.IdentityModel.Tokens.Jwt;
using CourseWorkDataBase.Data;
using CourseWorkDataBase.Models;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace CourseWorkDataBase.Controllers;

public class HomeController : Controller
{
    // private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;

    public HomeController(IConfiguration configuration, ApplicationDbContext context)
    {
        // _configuration = configuration;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Register()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Register(Users user)
    {
        if (!ModelState.IsValid)
        {
            return View(user);
        }

        var emailExists = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == user.Email.ToLower());

        if (emailExists != null)
        {
            ModelState.AddModelError("Email", "Этот Email уже используется.");
            return View(user);
        }

        string saltedPassword = HashPassword(user.Password);
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == 1);

        if (role == null)
        {
            ModelState.AddModelError(string.Empty, "Роль по умолчанию не найдена.");
            return View(user);
        }

        var newUser = new Users
        {
            Name = user.Name,
            FamilyName = user.FamilyName,
            Email = user.Email,
            Password = saltedPassword,
            RoleId = role.Id,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(newUser);
        
        try
        {
            Console.WriteLine($"in try");
            await _context.SaveChangesAsync();
            return RedirectToAction("Login");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"Exception during SaveChangesAsync: {ex.Message}");
            ModelState.AddModelError(string.Empty, "An error occurred during registration. Please try again later.");
            return View(user);
        }
    }
    
    private string HashPassword(string password)
    {
        byte[] salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));

        return $"{Convert.ToBase64String(salt)}.{hashed}";
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Username);
        if (user == null)
        {
            return View(model);
        }

        var passwordUser = await _context.Users.FirstOrDefaultAsync(u => u.Password == model.Password);
        if (passwordUser == null)
        {
            return View(model);
        }
        
        switch (user.RoleId)
        {
            case 1:
                return RedirectToAction("PatientPage", "Patient");
            case 2:
                return RedirectToAction("DoctorPage", "Doctor");
            default:
                ModelState.AddModelError(string.Empty, "Недопустимая роль пользователя.");
                return View(model);
        }
    }
}