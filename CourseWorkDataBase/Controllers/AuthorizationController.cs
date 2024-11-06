using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CourseWorkDataBase.Data;
using CourseWorkDataBase.Models;

namespace CourseWorkDataBase.Controllers;

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
    public async Task<IActionResult> LoginUser(LoginUserRequest request)
    {
        var user = await _authService.AuthenticateUser(request.Email, request.Password);
        if (user == null)
        {
            return Unauthorized("Invalid email or password.");
        }

        return RedirectToAction("PatientPage", "Patient");
    }

    // [HttpPost]
    // public async Task<IActionResult> LoginDoctor(LoginDoctorRequest request)
    // {
    //     var doctor = await _authService.AuthenticateDoctor(request.PersonalNumber);
    //     if (doctor == null)
    //     {
    //         return Unauthorized("Invalid personal number.");
    //     }
    //
    //     return RedirectToAction("DoctorPage", "Doctor");
    // }
}