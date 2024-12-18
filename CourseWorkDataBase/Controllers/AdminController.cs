using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using CourseWorkDataBase.Services;
using CourseWorkDataBase.DAL;
using CourseWorkDataBase.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CourseWorkDataBase.Controllers;

[Authorize ("AdminPolicy")]
public class AdminController : Controller
{
    private readonly AdminService _adminService;
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly ILogger<AdminController> _logger;
    private readonly IConfiguration _configuration;

    
    public AdminController(AdminService adminService, IDbContextFactory<ApplicationDbContext> dbContextFactory, 
        ILogger<AdminController> logger, 
        IConfiguration configuration)
    {
        _adminService = adminService;
        _dbContextFactory = dbContextFactory;
        _logger = logger;
        _configuration = configuration;
    }
    
    [HttpGet]
    public IActionResult RestoreDataBase()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> СreateArchivedCopiesOfTheDatabase()
    {
        _logger.LogDebug("start СreateArchivedCopiesOfTheDatabase");
        
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        var databaseName = context.Database.GetDbConnection().Database;
        
        var backupFolder = _configuration["BackupConfig:BackupFolderPath"];
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        
        var backupFileName = $"{databaseName}_{timestamp}.bak";
        var backupPath = Path.Combine(backupFolder, backupFileName);
        var pgDumpPath = "/opt/homebrew/bin/pg_dump";
        
        var connection = (Npgsql.NpgsqlConnection)context.Database.GetDbConnection();
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
    public IActionResult GetAvailableBackups()
    {
        _logger.LogDebug("Retrieving a list of available database backups.");
        var backupFolder = _configuration["BackupConfig:BackupFolderPath"];

        var backupFiles = Directory.GetFiles(backupFolder)
            .Select(Path.GetFileName)
            .OrderByDescending(f => f)
            .ToList();

        if (!backupFiles.Any())
        {
            _logger.LogInformation("Backup folder '{BackupFolder}' no backup files.", backupFolder);
            return Ok(new List<string>());
        }

        _logger.LogInformation("Find {Count} backup files in folder '{BackupFolder}'.", backupFiles.Count,
            backupFolder);
        return Ok(backupFiles);
    }

    [HttpPost]
    public async Task<IActionResult> RestoreArchivedCopiesOfTheDatabase(string backupFile)
    {
        _logger.LogDebug("Starting database restoration process.");

        if (string.IsNullOrWhiteSpace(backupFile))
        {
            _logger.LogWarning("No backup file specified.");
            return BadRequest("No backup file specified.");
        }
        
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        var backupFolder = _configuration["BackupConfig:BackupFolderPath"];

        string pathFile = Path.Combine(backupFolder, backupFile);

        if (!System.IO.File.Exists(pathFile))
        {
            _logger.LogWarning($"Backup file '{pathFile}' does not exist.");
            return NotFound("Backup file not found.");
        }

        try
        {
            var connection = (Npgsql.NpgsqlConnection)context.Database.GetDbConnection();
            var builder = new Npgsql.NpgsqlConnectionStringBuilder(connection.ConnectionString);

            string host = builder.Host;
            int port = builder.Port;
            string userName = builder.Username;
            string password = builder.Password;
            string databaseName = builder.Database;

            string pgRestorePath = "/opt/homebrew/bin/pg_restore";

            Environment.SetEnvironmentVariable("PGPASSWORD", password);

            string arguments = $"-h {host} -p {port} -U {userName} -d {databaseName} -c \"{pathFile}\"";

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = pgRestorePath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            _logger.LogDebug($"debug: {pgRestorePath}");
            process.Start();
            string error = await process.StandardError.ReadToEndAsync();

            process.WaitForExit();

            Environment.SetEnvironmentVariable("PGPASSWORD", null);

            if (process.ExitCode == 0)
            {
                _logger.LogInformation("Database backup restored successfully.");
                System.IO.File.Delete(pathFile);
                return RedirectToAction("AdminMainPage", "Admin");
            }
            else
            {
                _logger.LogError($"Error restoring database backup. Exit code: {process.ExitCode}, Error: {error}");
                throw new Exception($"Error restoring backup. Details: {error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception occurred during database restoration: {ex.Message}");
            Environment.SetEnvironmentVariable("PGPASSWORD", null);
            if (System.IO.File.Exists(pathFile))
            {
                _logger.LogWarning($"Deleting backup file at '{pathFile}' due to error.");
                System.IO.File.Delete(pathFile);
            }

            return BadRequest($"Exception during database restoration: {ex.Message}");
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
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var specialties = await context.Specialties
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
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var clinic = await context.Clinics
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

    [HttpPost]
    public async Task<IActionResult> DeleteDoctor(long id)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        Console.Out.WriteLine($"ID: {id}");
        Console.Out.WriteLine("Delete Doctor1");
        _logger.LogInformation($"Initiating deletion process for Doctor with ID: {id}");
    
        var doctor = await context.Doctors
            .Include(d => d.User)
            .Include(d => d.AppointmentSlots)
            .FirstOrDefaultAsync(d => d.ID == id);
        if (doctor == null)
        {
            _logger.LogWarning($"Doctor with ID {id} not found.");
            return NotFound(new { message = "Doctor not found." });
        }

        var doctorUserId = doctor.UserId;
    
        using (var transaction = context.Database.BeginTransaction())
        {
            try
            {
                _logger.LogInformation($"Start transaction for Doctor with ID: {id}");
                
                var appointments = await context.Appointments
                    .Where(a => a.AppointmentSlot.DoctorId == id)
                    .Include(a => a.MedicalRecords)
                    .ToListAsync();
    
                if (appointments.Any())
                {
                    foreach (var appointment in appointments)
                    {
                        if (appointment.MedicalRecords != null && appointment.MedicalRecords.Any())
                        {
                            context.MedicalRecords.RemoveRange(appointment.MedicalRecords);
                            _logger.LogInformation(
                                $"Deleted MedicalRecords for Appointment ID {appointment.Id} associated with Doctor ID {id}.");
                        }
    
                        context.Appointments.Remove(appointment);
                        _logger.LogInformation($"Deleted Appointment ID {appointment.Id} for Doctor ID {id}.");
                    }
                }
                
                _logger.LogInformation($"transaction2 for Doctor with ID: {id}");
                
                if (doctor.AppointmentSlots != null && doctor.AppointmentSlots.Any())
                {
                    context.AppointmentSlots.RemoveRange(doctor.AppointmentSlots);
                    _logger.LogInformation(
                        $"Deleted {doctor.AppointmentSlots.Count} appointment slots for Doctor ID {id}.");
                }
                
                _logger.LogInformation($"transaction3 for Doctor with ID: {id}");
    
                var user = await context.Users
                    .Include(d => d.Doctor)
                    .FirstOrDefaultAsync(d => d.Id == doctor.ID);
                if (user == null)
                {
                    Console.Out.WriteLine("Doctor not found");
                    return NotFound();
                }
                
                _logger.LogInformation($"transaction4 for Doctor with ID: {id}");
    
                Console.Out.WriteLine("Delete Doctor2");
                context.Doctors.Remove(doctor);
                Console.Out.WriteLine("Delete Doctor3");
    
                if (doctor.User != null)
                {
                    context.Users.Remove(user);
                    Console.Out.WriteLine("Delete Doctor4");
                }
                
                _logger.LogInformation($"transaction5 for Doctor with ID: {id}");
    
                Console.Out.WriteLine("Delete Doctor5");
                
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                
                _logger.LogInformation($"End transaction for Doctor with ID: {id}");
                
                return RedirectToAction("DoctorsList", "Admin");
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                Console.WriteLine(e);
                throw;
            }
        }
    }

    [HttpGet]
    public async Task<IActionResult> DoctorsList()
    {
        var doctors = await _adminService.GetAllDoctorsAsync();
        return View(doctors);
    }
}