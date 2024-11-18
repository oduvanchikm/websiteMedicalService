using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using CourseWorkDataBase.Services;
using CourseWorkDataBase.DAL;
using CourseWorkDataBase.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CourseWorkDataBase.Controllers;

[Authorize]
public class AdminController : Controller
{
    private readonly AdminService _adminService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AdminController> _logger;

    public AdminController(AdminService adminService, ApplicationDbContext context, ILogger<AdminController> logger)
    {
        _adminService = adminService;
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> СreateArchivedCopiesOfTheDatabase()
    {
        _logger.LogDebug("start СreateArchivedCopiesOfTheDatabase");
        var databaseName = _context.Database.GetDbConnection().Database;

        var backupFolder = "/Users/oduvanchik/Desktop/CourseWorkDataBase/CourseWorkDataBase/Backups";
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        var backupFileName = $"{databaseName}_{timestamp}.bak";
        var backupPath = Path.Combine(backupFolder, backupFileName);

        var pgDumpPath = "/opt/homebrew/bin/pg_dump";

        var connection = (Npgsql.NpgsqlConnection)_context.Database.GetDbConnection();
        var builder = new Npgsql.NpgsqlConnectionStringBuilder(connection.ConnectionString);

        var host = builder.Host;
        var port = builder.Port;
        var userName = builder.Username;
        var password = builder.Password;

        Environment.SetEnvironmentVariable("PGPASSWORD", password);

        var arguments = $"-h {host} -p {port} -U {userName} -F c -b -v -f \"{backupPath}\" {databaseName}";

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = pgDumpPath,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        try
        {
            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            process.WaitForExit();

            Environment.SetEnvironmentVariable("PGPASSWORD", null);

            if (process.ExitCode == 0)
            {
                _logger.LogDebug("The database backup has been successfully created.");
                return RedirectToAction("AdminMainPage", "Admin");
            }
            else
            {
                _logger.LogWarning("The database backup could not be created.");
                throw new Exception($"Error when creating a backup: {error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "The database backup could not be created.");
            Environment.SetEnvironmentVariable("PGPASSWORD", null);
            throw new Exception($"Exception when creating a backup: {ex.Message}");
        }
    }

    [HttpGet]
    public IActionResult AdminMainPage()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("AuthorizationPage", "Authorization");
    }

    [HttpGet]
    public async Task<IActionResult> AddDoctor()
    {
        var specialties = await GetSpecialtiesSelectListAsync();

        foreach (var s in specialties)
        {
            Console.Out.WriteLine($"ID: {s.Value}, Name: {s.Text}");
        }

        var clinics = await GetClinicsSelectListAsync();

        foreach (var s in clinics)
        {
            Console.Out.WriteLine($"ID: {s.Value}, Name: {s.Text}");
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
        // TODO ОНО НЕПРАВИЛЬНО НЕ РАБОТАЕТ
        Console.Out.WriteLine("Delete Doctor1");
        var doctor = await _context.Doctors
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.ID == id);

        Console.Out.WriteLine("Delete Doctor2");

        if (doctor == null)
        {
            Console.Out.WriteLine("Doctor not found");
            return NotFound();
        }

        _context.Doctors.Remove(doctor);

        Console.Out.WriteLine("Delete Doctor3");

        if (doctor.User != null)
        {
            _context.Users.Remove(doctor.User);
            Console.Out.WriteLine("Delete Doctor4");
        }

        Console.Out.WriteLine("Delete Doctor5");
        await _context.SaveChangesAsync();

        return RedirectToAction("DoctorsList", "Admin");
    }

    [HttpGet]
    public async Task<IActionResult> DoctorsList()
    {
        var doctors = await _adminService.GetAllDoctorsAsync();
        return View(doctors);
    }
}