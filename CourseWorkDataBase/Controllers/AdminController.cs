using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CourseWorkDataBase.Data;
using CourseWorkDataBase.DAL;
using CourseWorkDataBase.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CourseWorkDataBase.Controllers;

public class AdminController : Controller
{
    private readonly AdminService _adminService;
    private readonly ApplicationDbContext _context;

    public AdminController(AdminService adminService, ApplicationDbContext context)
    {
        _adminService = adminService;
        _context = context;
    }
    
    [HttpGet]
    public async Task<IActionResult> AddDoctor()
    {
        var specialties = await GetSpecialtiesSelectListAsync();
        
        foreach(var s in specialties)
        {
            Console.WriteLine($"ID: {s.Value}, Name: {s.Text}");
        }
        
        var clinics = await GetClinicsSelectListAsync();
        
        foreach(var s in clinics)
        {
            Console.WriteLine($"ID: {s.Value}, Name: {s.Text}");
        }
        
        var viewModel = new AddDoctorRequest()
        {
            Specialties = specialties,
            Clinics = clinics
        };
        
        return View(viewModel);
    }

    private async Task<IEnumerable<SelectListItem>> GetSpecialtiesSelectListAsync()
    {
        var specialties = await _context.Specialties
            .OrderBy(s => s.NameSpecialty)
            .ToListAsync();
    
        return specialties.Select(s => new SelectListItem
        {
            Value = s.Id.ToString(),
            Text = s.NameSpecialty
        }).ToList();
    }
    
    private async Task<IEnumerable<SelectListItem>> GetClinicsSelectListAsync()
    {
        var clinic = await _context.Clinics
            .OrderBy(s => s.Address)
            .ToListAsync();
    
        return clinic.Select(s => new SelectListItem
        {
            Value = s.Id.ToString(),
            Text = s.Address
        }).ToList();
    }

    [HttpPost]
    public async Task<IActionResult> AddDoctor(AddDoctorRequest model)
    {
        if (!ModelState.IsValid)
        {
            foreach (var modelState in ModelState)
            {
                foreach (var error in modelState.Value.Errors)
                {
                    Console.Out.WriteLine(error.ErrorMessage);
                }
            }
            model.Specialties = await GetSpecialtiesSelectListAsync();
            model.Clinics = await GetClinicsSelectListAsync();
            return View(model);
        }

        try
        {
            var doctor = await _adminService.AddDoctorAsync(
                model.email,
                model.familyName,
                model.firstName,
                model.personalNumber,
                model.specialtyId,
                model.specialtyName,
                model.description,
                model.clinicId,
                model.clinicAddress,
                model.clinicPhoneNumber
            );

            return RedirectToAction("DoctorsList", "Admin");
        }
        catch (Exception ex)
        {
            Console.Out.WriteLine($"Error: {ex.Message}");
            Console.Out.WriteLine($"Call stack: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.Out.WriteLine($"Internal error: {ex.InnerException.Message}");
            }

            ModelState.AddModelError("", "An unknown error has occurred. Please try again later.");
        }

        
        model.Specialties = await GetSpecialtiesSelectListAsync();
        model.Clinics = await GetClinicsSelectListAsync();
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> DeleteDoctor(long id)
    {
        var doctor = await _context.Doctors
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.ID == id);

        if (doctor == null)
        {
            return Json(new { success = false, message = "Doctor not found." });
        }
        
        _context.Doctors.Remove(doctor);

        if (doctor.User != null)
        {
            _context.Users.Remove(doctor.User);
        }
        
        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> DoctorsList()
    {
        var doctors = await _adminService.GetAllDoctorsAsync();
        return View(doctors);
    }
}