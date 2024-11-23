using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using CourseWorkDataBase.Data;
using CourseWorkDataBase.ViewModels;
using System.Security.Claims;

namespace CourseWorkDataBase.Controllers;

public class AuthorizationController : Controller
{
    public enum UserRole
    {
        Admin = 1,
        Doctor = 2,
        Patient = 3
    }
    
    private readonly AuthorizationService _authService;
    private readonly ILogger<AuthorizationController> _logger;

    public AuthorizationController(AuthorizationService authService, ILogger<AuthorizationController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult AuthorizationPage()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AuthorizationPage(LoginUserRequest request)
    {
        Console.Out.WriteLine(request.Email);
        Console.Out.WriteLine(request.Password);

        if (!ModelState.IsValid)
        {
            Console.Out.WriteLine("not valid");
            return RedirectToAction("AuthorizationPage", "Authorization");
        }

        var user = await _authService.AuthenticateUser(request.Email, request.Password);
        if (user == null)
        {
            Console.Out.WriteLine("Wrong email address or password");
            return RedirectToAction("AuthorizationPage", "Authorization");
        }
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role.Name)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            AllowRefresh = true
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties
        );
        
        var userRole = (UserRole)user.RoleId;

        switch (userRole)
        {
            case UserRole.Admin:
                return RedirectToAction("AdminMainPage", "Admin");
            case UserRole.Doctor:
                return RedirectToAction("DoctorPage", "Doctor");
            case UserRole.Patient:
                return RedirectToAction("PatientPage", "Patient");
            default:
                _logger.LogWarning("Unknown role {RoleId} for user with email {Email}", user.RoleId, user.Email);
                TempData["ErrorMessage"] = "Unknown user role.";
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("AuthorizationPage", "Authorization");
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("AuthorizationPage", "Authorization");
    }
}