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
        
        if (user.RoleId == 1)
        {
            return RedirectToAction("DoctorsList", "Admin");
        }
        else if (user.RoleId == 2)
        {
            return RedirectToAction("DoctorPage", "Doctor");
        }
        else if (user.RoleId == 3)
        {
            return RedirectToAction("PatientPage", "Patient");
        }
        
        return RedirectToAction("AuthorizationPage", "Authorization");
    }

    // [HttpPost]
    // public async Task<IActionResult> LoginDoctor(LoginDoctorRequest request)
    // {
    //     if (!ModelState.IsValid)
    //     {
    //         TempData["ErrorMessage"] = "Invalid login request.";
    //         return RedirectToAction("AuthorizationPage", "Authorization");
    //     }
    //
    //     var user = await _authService.AuthenticateDoctor(request.Email, request.PersonalNumber);
    //     if (user == null)
    //     {
    //         TempData["ErrorMessage"] = "Invalid personal number.";
    //         return RedirectToAction("AuthorizationPage", "Authorization");
    //     }
    //
    //     if (user.RoleId == 2)
    //     {
    //         return RedirectToAction("DoctorPage", "Doctor");
    //     }
    //     
    //     return 
    // }
}