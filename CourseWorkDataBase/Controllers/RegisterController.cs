using Microsoft.AspNetCore.Mvc;
using CourseWorkDataBase.ViewModels;
using CourseWorkDataBase.Data;

namespace CourseWorkDataBase.Controllers;

public class RegisterController : Controller
{
    private readonly RegistrationService _registrationService;

    public RegisterController(RegistrationService registrationService)
    {
        _registrationService = registrationService;
    }
    
    [HttpGet]
    public IActionResult RegisterPage()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RegisterPage(RegisterPatientRequest request)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Please fill in all fields correctly.";
            return RedirectToAction("RegisterPage", "Register");
        }
        
        try
        {
            var patient = await _registrationService.RegisterPage(
                request.Email, 
                request.Password, 
                request.FirstName, 
                request.FamilyName, 
                request.Gender);
            
            return RedirectToAction("AuthorizationPage", "Authorization");
        }
        catch (ApplicationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction("RegisterPage", "Register");
        }
    }
}