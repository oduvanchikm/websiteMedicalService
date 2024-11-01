using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CourseWorkDataBase.Models;
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
        try
        {
            var patient = await _registrationService.RegisterPage(request.Email, request.Password, request.FirstName, request.FamilyName, request.ContactInfo, request.Gender);
            return RedirectToAction("AuthorizationPage", "Authorization");
        }
        catch (ApplicationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}