using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using CourseWorkDataBase.Data;
using CourseWorkDataBase.Models;
using CourseWorkDataBase.ViewModels;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace CourseWorkDataBase.Controllers;

// [Route("login")]
public class AuthorizationController : Controller
{
    private readonly AuthorizationService _authService;

    public AuthorizationController(AuthorizationService authService)
    {
        _authService = authService;
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
            TempData["ErrorMessage"] = "Invalid login request.";
            return RedirectToAction("AuthorizationPage", "Authorization");
        }

        var user = await _authService.AuthenticateUser(request.Email, request.Password);
        if (user == null)
        {
            TempData["ErrorMessage"] = "Invalid email address or password.";
            return RedirectToAction("AuthorizationPage", "Authorization");
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        if (user.Role != null)
        {
            Console.Out.WriteLine("ROLE NAME " + user.Role.Name);
            claims.Add(new Claim(ClaimTypes.Role, user.Role.Name));
            Console.Out.WriteLine("CLAIM ROLE " + ClaimTypes.Role);
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        if (user.RoleId == 1)
        {
            return RedirectToAction("DoctorsList", "Admin");
        }
        
        return RedirectToAction("PatientPage", "Patient");
    }

    [HttpPost]
    public async Task<IActionResult> LoginDoctor(LoginDoctorRequest request)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Invalid login request.";
            return RedirectToAction("AuthorizationPage", "Authorization");
        }

        var user = await _authService.AuthenticateDoctor(request.Email, request.PersonalNumber);
        if (user == null)
        {
            TempData["ErrorMessage"] = "Invalid personal number.";
            return RedirectToAction("AuthorizationPage", "Authorization");
        }

        // if (user.RoleId == 2)
        // {
        return RedirectToAction("DoctorPage", "Doctor");
        // }
    }
}