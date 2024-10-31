using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseWorkDataBase.Services;
using Microsoft.Extensions.Configuration;
// using Microsoft.IdentityModel.Tokens;
// using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CourseWorkDataBase.Data;
using CourseWorkDataBase.Models;

namespace CourseWorkDataBase.Controllers;

// [ApiController]
// [Route("api/[controller]")]
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
    public async Task<IActionResult> Register(Users users)
    {
        if (!ModelState.IsValid)
        {
            return View(users);
        }
        
        if (await _context.Users.AnyAsync(x => x.Email == users.Email))
        {
            return BadRequest("Email is already taken");
        }

        // var role = await _context.Roles.FirstOrDefaultAsync(x => x.Name == users.Role.Name);
        // if (role == null)
        // {
        //     return BadRequest("Role does not exist");
        // }
        
        string saltedPassword = PasswordHelper.HashPassword(users.Password, users.Email);

        // var user = new Users
        // {
        //     ID = users.ID,
        //     Name = users.Name,
        //     FamilyName = users.FamilyName,
        //     Email = users.Email,
        //     Password = saltedPassword,
        //     // RoleId = role.Id,
        //     CreatedAt = DateTime.UtcNow
        // };
        
        users.Password = saltedPassword;
        
        _context.Users.Add(users);
        await _context.SaveChangesAsync();
        return RedirectToAction("Login");
    }
    
    public IActionResult Login()
    {
        return View();
    }
    
    public async Task<IActionResult> Login(string username, string password) 
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            return View("Error");
        }
        
        var user = await _context.Users.FirstOrDefaultAsync(u => u.FamilyName == username);
        if (user == null)
        {
            return View("Error");
        }

        var passwordUser = await _context.Users.FirstOrDefaultAsync(u => u.Password == password);
        if (passwordUser == null)
        {
            return View("Error");
        }
        
        return RedirectToAction("HomePage");
    }
}