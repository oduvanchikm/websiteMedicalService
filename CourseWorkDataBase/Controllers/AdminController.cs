using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CourseWorkDataBase.Data;
using CourseWorkDataBase.DAL;
using CourseWorkDataBase.Models;
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
    public IActionResult AddDoctor()
    {
        // var specialties = await GetSpecialtiesSelectListAsync();
        //
        // foreach(var s in specialties)
        // {
        //     Console.WriteLine($"ID: {s.Value}, Name: {s.Text}");
        // }
        //
        // var viewModel = new AddDoctorViewModel()
        // {
        //     Doctor = new AddDoctorRequest(),
        //     Specialties = specialties
        // };
        
        return View();
    }

    // private async Task<IEnumerable<SelectListItem>> GetSpecialtiesSelectListAsync()
    // {
    //     var specialties = await _context.Specialties
    //         .OrderBy(s => s.NameSpecialty)
    //         .ToListAsync();
    //
    //     return specialties.Select(s => new SelectListItem
    //     {
    //         Value = s.Id.ToString(),
    //         Text = s.NameSpecialty
    //     }).ToList();
    // }
    
    // public async Task<IActionResult> AddDoctor()
    // {
    //     var specialties = await GetSpecialtiesSelectListAsync();
    //     ViewBag.Specialties = specialties;
    //     return View(new AddDoctorRequest());
    // }

    [HttpPost]
    public async Task<IActionResult> AddDoctor(AddDoctorRequest model)
    {
        Console.Out.WriteLine(model.email);
        Console.Out.WriteLine(model.familyName);
        Console.Out.WriteLine(model.firstName);
        
        if (!ModelState.IsValid)
        {
            foreach (var modelState in ModelState)
            {
                foreach (var error in modelState.Value.Errors)
                {
                    Console.Out.WriteLine(error.ErrorMessage);
                }
            }
            return View(model);
        }

        try
        {
            var doctor = await _adminService.AddDoctorAsync(
                model.email,
                model.familyName,
                model.firstName
                // model.SpecialtyId,
                // model.ClinicAddress,
                // model.ClinicPhoneNumber
            );

            return RedirectToAction("AddedDoctor", "Admin");
        }
        catch (Exception ex)
        {
            Console.Out.WriteLine($"Ошибка: {ex.Message}");
            Console.Out.WriteLine($"Стек вызовов: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.Out.WriteLine($"Внутренняя ошибка: {ex.InnerException.Message}");
            }

            ModelState.AddModelError("", "Произошла неизвестная ошибка. Пожалуйста, попробуйте позже.");
        }

        
        // var specialties = await GetSpecialtiesSelectListAsync();
        // ViewBag.Specialties = specialties;
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> DoctorsList()
    {
        var doctors = await _adminService.GetAllDoctorsAsync();
        return View(doctors);
    }

    public async Task<IActionResult> AddedDoctor(long id)
    {
        var doctor = await _context.Doctors
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.ID == id);

        if (doctor == null)
        {
            return NotFound();
        }

        var model = new DoctorAddedViewModel
        {
            DoctorId = doctor.ID,
            FullName = $"{doctor.FirstName} {doctor.FamilyName}",
            PersonalNumber = doctor.User.PersonalNumber
        };

        return View(model);
    }
}