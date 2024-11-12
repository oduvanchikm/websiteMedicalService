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
            return View(request);
        }
        
        try
        {
            var patient = await _registrationService.RegisterPage(
                request.Email, 
                request.Password, 
                request.FirstName, 
                request.FamilyName, 
                request.Gender);
            
            // await _signInManager.SignInAsync(patient, isPersistent: false);
            
            return RedirectToAction("AuthorizationPage", "Authorization");
        }
        catch (ApplicationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}