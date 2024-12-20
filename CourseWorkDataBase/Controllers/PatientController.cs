using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CourseWorkDataBase.DAL;
using CourseWorkDataBase.Models;
using CourseWorkDataBase.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using CourseWorkDataBase.Helpers;

namespace CourseWorkDataBase.Controllers;

[Authorize("PatientPolicy")]
public class PatientController : Controller
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly ILogger<PatientController> _logger;
    private readonly IConfiguration _configuration;

    public PatientController(IDbContextFactory<ApplicationDbContext> dbContextFactory, ILogger<PatientController> logger,
        IConfiguration configuration)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
        _configuration = configuration;
    }
    
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("AuthorizationPage", "Authorization");
    }
    
    public async Task<IEnumerable<DoctorDTO>> GetDoctorsBySpecialtyAsync(long? specialtyId)
    {
        var param = specialtyId ?? 0;
        
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        var doctors = await context.DoctorsDto
            .FromSqlInterpolated($"SELECT * FROM GetDoctorsBySpecialty({param})").ToListAsync();

        return doctors;
    }

    public async Task<IActionResult> PatientPage(long? specialtyId)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        var specialties = await context.Specialties
            .OrderBy(s => s.NameSpecialty)
            .ToListAsync();
        
        var doctors = await GetDoctorsBySpecialtyAsync(specialtyId);
        
        var specialtyItems = specialties
            .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.NameSpecialty
                }
            ).ToList();
        
        specialtyItems.Insert(0, new SelectListItem
        {
            Value = "0",
            Text = "All specialties"
        });
        
        var viewModel = new PatientPageViewModel
        {
            Doctors = doctors,
            Specialties = specialtyItems,
            SelectedSpecialtyId = specialtyId
        };
        
        return View(viewModel);
    }

    public async Task<IActionResult> ViewDoctor(long doctorId)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        var slots = await context.AppointmentSlots
            .Where(s => s.DoctorId == doctorId && s.StartTime >= DateTime.Today && !s.IsBooked)
            .OrderBy(s => s.StartTime)
            .ToListAsync();

        var doctor = await context.Doctors.Include(d => d.Specialty)
            .FirstOrDefaultAsync(d => d.ID == doctorId);
        if (doctor == null)
        {
            _logger.LogError("Doctor Not Found");
            return NotFound();
        }

        var viewModel = new BookAppointmentViewModel
        {
            Doctor = doctor,
            AvailableSlots = slots
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> BookAppointment(long slotId)
    {
        var userId = GetCurrentUserId();
        
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        var patient = await context.Patients
            .Where(p => p.UserId == userId)
            .FirstOrDefaultAsync();
        if (patient == null)
        {
            _logger.LogWarning("Patient not found {UserId}", userId);
            return NotFound("Patient not found.");
        }
        
        Console.Out.WriteLine($"Patient {patient.Id}");
        
        try
        {
            await context.Database.ExecuteSqlRawAsync(
                "CALL BookAppointment({0}, {1})",
                slotId,
                patient.Id
            );
            
            return RedirectToAction("PatientAppointments", "Patient");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error when booking an appointment for a patient {patient.Id}");
            TempData["ErrorMessage"] = ex.Message; 
            return RedirectToAction("PatientPage", "Patient");
        }
    }

    [HttpGet]
    public async Task<IActionResult> PatientAppointments()
    {
        try
        {
            await using var context = await _dbContextFactory.CreateDbContextAsync();
            
            var userId = GetCurrentUserId();
        
            var patient = await context.Patients
                .Where(p => p.UserId == userId)
                .FirstOrDefaultAsync();

            if (patient == null)
            {
                _logger.LogWarning("Patient not found {UserId}", userId);
                return NotFound("Patient not found.");
            }
        
            Console.Out.WriteLine("Patient id " + patient.Id + " and user id " + userId);

            var appointments = await context.Appointments
                .Where(a => a.PatientId == patient.Id)
                .Include(a => a.AppointmentSlot)
                .ThenInclude(s => s.Doctor)
                .Include(a => a.Status)
                .OrderBy(a => a.AppointmentSlot.StartTime)
                .ToListAsync();

            return View(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when receiving patient appointment records.");
            return StatusCode(500, "Internal server error.");
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> CancelAppointment(long appointmentId)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        var appointment = await context.Appointments
            .Include(a => a.AppointmentSlot)
            .FirstOrDefaultAsync(a => a.Id == appointmentId);
        if (appointment == null)
        {
            TempData["ErrorMessage"] = "Appointment not found or you do not have permission to cancel it.";
            return RedirectToAction("PatientAppointments", "Patient");
        }

        if (appointment.StatusId != 1)
        {
            TempData["ErrorMessage"] = "Only scheduled appointments can be canceled.";
            return RedirectToAction("PatientAppointments", "Patient");
        }
        
        if (appointment.AppointmentSlot != null)
        {
            appointment.StatusId = 3;
            appointment.AppointmentSlot.IsBooked = false;
            context.Appointments.Update(appointment);
            context.AppointmentSlots.Update(appointment.AppointmentSlot);
        }
        
        Console.Out.WriteLine($"appointment {appointment.Id} cancelled");

        try
        {
            await context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Appointment has been canceled successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while canceling appointment.");
            TempData["ErrorMessage"] = "An unexpected error occurred. Please try again.";
        }

        return RedirectToAction("PatientAppointments", "Patient");
    }

    public async Task<IActionResult> Show() 
    {
        var userId = GetCurrentUserId();
        
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        var patientid = await context.Patients
            .Where(p => p.UserId == userId)
            .FirstOrDefaultAsync();
        if (patientid == null)
        {
            _logger.LogWarning("Patient not found {UserId}", userId);
            return NotFound("Patient not found.");
        }
        
        Console.Out.WriteLine($"Patient {patientid.Id}");
        
        var patient = await context.Patients
            .AsNoTracking()
            .Include(p => p.Appointments)
            .ThenInclude(appt => appt.MedicalRecords)
            .ThenInclude(mr => mr.MedicalRecordMedications)
            .ThenInclude(mrm => mrm.Medication)
            .FirstOrDefaultAsync(p => p.Id == patientid.Id);
        if (patient == null)
        {
            _logger.LogWarning($"Patient with ID {patientid.Id} not found.");
            return NotFound();
        }

        var medicalRecordsViewModel = patient.Appointments
            .SelectMany(appt => appt.MedicalRecords, (appt, mr) => new { appt, mr })
            .SelectMany(record => record.mr.MedicalRecordMedications, (record, mrm) => new AddMedicalRecordsViewModel
            {
                Description = record.mr.Description,
                Diagnosis = record.mr.Diagnosis,
                NameMedication = mrm.Medication?.Name ?? "not found",
                DescriptionMedication = mrm.Medication?.Description ?? "not found",
            })
            .ToList();

        var viewModel = new MedicalHistoryViewModel
        {
            PatientId = patient.Id,
            PatientName = patient.FirstName,
            PatientSurname = patient.FamilyName,
            MedicalRecords = medicalRecordsViewModel
        };
        
        return View(viewModel);
    }

    private long GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && long.TryParse(userIdClaim.Value, out long userId))
        {
            return userId;
        }
        
        throw new InvalidOperationException("Couldn't get the ID of the current user.");
    }
    
    [HttpGet]
    public async Task<IActionResult> CreatePdfFileWithMedicalRecordsPatient(long id)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        Console.Out.WriteLine($"in get : {id}");
        
        var patient = await context.Patients
            .AsNoTracking()
            .Include(p => p.Appointments)
            .ThenInclude(appt => appt.AppointmentSlot)
            .ThenInclude(slot => slot.Doctor)
            .Include(p => p.Appointments)
            .ThenInclude(appt => appt.MedicalRecords)
            .ThenInclude(mr => mr.MedicalRecordMedications)
            .ThenInclude(mrm => mrm.Medication)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (patient == null)
        {
            throw new Exception($"Patient with ID {id} not found.");
        }

        var appointments = patient.Appointments?.ToList();
        if (appointments == null || !appointments.Any())
        {
            throw new Exception($"No appointments found for patient with ID {id}.");
        }

        var medicalRecordsWithDoctors = appointments
            .Select(appt => new
            {
                Appointment = appt,
                Doctor = appt.AppointmentSlot.Doctor,
                Clinic = appt.AppointmentSlot.Doctor?.Clinic,
                Specialty = appt.AppointmentSlot.Doctor?.Specialty
            })
            .Where(x => x.Doctor != null) 
            .SelectMany(x => x.Appointment.MedicalRecords.Select(mr => new MedicalRecordWithDoctor
            {
                medicalRecord = mr,
                doctor = x.Doctor,
                clinic = x.Clinic,
                specialty = x.Specialty
            }))
            .ToList();

        if (!medicalRecordsWithDoctors.Any())
        {
            throw new Exception($"Not medical record with ID {id}.");
        }

        foreach (var mrwd in medicalRecordsWithDoctors)
        {
            Console.Out.WriteLine($"ID medical record: {mrwd.medicalRecord.Id}, Doctor ID: {mrwd.doctor.ID}");
        }

        var fileName = $"MedicalRecords_{patient.FamilyName}_{patient.FirstName}.pdf";
        var backupFolder = _configuration["PdfFileConfig:PdfFolderPath"];
        if (!Directory.Exists(backupFolder))
        {
            Directory.CreateDirectory(backupFolder);
        }
        
        var fullPath = Path.Combine(backupFolder, fileName);

        var pdfData = new PdfData
        {
            FullPath = fullPath,
            Patient = patient,
            medicalRecordWithDoctor = medicalRecordsWithDoctors
        };

        try
        {
            await SavePdfFile.CreatePdfFileWithMedicalRecords(pdfData);
        }
        catch (IOException ex) 
        {
            throw new Exception($"PdfException occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Unexpected exception occurred: {ex.Message}");
        }

        var fileBytes = System.IO.File.ReadAllBytes(fullPath);

        if (!System.IO.File.Exists(fullPath))
        {
            throw new FileNotFoundException($"File not found at {fullPath}");
        }

        return File(fileBytes, "application/pdf", fileName);
    }
}