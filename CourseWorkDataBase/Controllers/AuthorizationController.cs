using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using CourseWorkDataBase.Data;
using CourseWorkDataBase.Models;

namespace CourseWorkDataBase.Controllers;

[Route("login")]
public class AuthorizationController : Controller
{
    private readonly AuthorizationService _authService;

    public AuthorizationController(AuthorizationService authService)
    {
        _authService = authService;
    }
    
    [HttpGet("")]
    public IActionResult AuthorizationPage()
    {
        return View();
    }

    [HttpPost("/login/user")]
    public async Task<IActionResult> LoginUser(LoginUserRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Неверный запрос на вход.");
        }
        
        var user = await _authService.AuthenticateUser(request.Email, request.Password);
        if (user == null)
        {
            return Unauthorized("Неверный адрес электронной почты или пароль.");
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role.Name) 
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true, 
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1) 
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        return RedirectToAction("PatientPage", "Patient");
    }
    
    [HttpGet("AccessDenied")]
    public IActionResult AccessDenied()
    {
        return View();
    }


    // [HttpPost]
    // public async Task<IActionResult> LoginDoctor(LoginDoctorRequest request)
    // {
    //     if (!ModelState.IsValid)
    //     {
    //         return BadRequest("Invalid login request.");
    //     }
    //     
    //     var user = await _authService.AuthenticateDoctor(request.PersonalNumber);
    //     if (user == null)
    //     {
    //         return Unauthorized("Invalid personal number.");
    //     }
    //
    //     return RedirectToAction("DoctorPage", "Doctor");
    // }
}