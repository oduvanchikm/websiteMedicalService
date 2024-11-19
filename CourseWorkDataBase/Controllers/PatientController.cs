using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CourseWorkDataBase.DAL;
using CourseWorkDataBase.Models;
using CourseWorkDataBase.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

namespace CourseWorkDataBase.Controllers;

[Authorize]
public class PatientController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PatientController> _logger;

    public PatientController(ApplicationDbContext context, ILogger<PatientController> logger)
    {
        _context = context;
        _logger = logger;
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

        var doctors = await _context.DoctorsDto
            .FromSqlInterpolated($"SELECT * FROM GetDoctorsBySpecialty({param})").ToListAsync();

        return doctors;
    }

    public async Task<IActionResult> PatientPage(long? specialtyId)
    {
        var specialties = await _context.Specialties
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
        var slots = await _context.AppointmentSlots
            .Where(s => s.DoctorId == doctorId && s.StartTime >= DateTime.Today && !s.IsBooked)
            .OrderBy(s => s.StartTime)
            .ToListAsync();

        var doctor = await _context.Doctors.Include(d => d.Specialty)
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

        var patient = await _context.Patients
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
            await _context.Database.ExecuteSqlRawAsync(
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
            var userId = GetCurrentUserId();
        
            var patient = await _context.Patients
                .Where(p => p.UserId == userId)
                .FirstOrDefaultAsync();

            if (patient == null)
            {
                _logger.LogWarning("Patient not found {UserId}", userId);
                return NotFound("Patient not found.");
            }
        
            Console.Out.WriteLine("Patient id " + patient.Id + " and user id " + userId);

            var appointments = await _context.Appointments
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
        var appointment = await _context.Appointments
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
            _context.Appointments.Update(appointment);
            _context.AppointmentSlots.Update(appointment.AppointmentSlot);
        }
        
        Console.Out.WriteLine($"appointment {appointment.Id} cancelled");

        try
        {
            await _context.SaveChangesAsync();
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

        var patientid = await _context.Patients
            .Where(p => p.UserId == userId)
            .FirstOrDefaultAsync();
        if (patientid == null)
        {
            _logger.LogWarning("Patient not found {UserId}", userId);
            return NotFound("Patient not found.");
        }
        
        Console.Out.WriteLine($"Patient {patientid.Id}");
        
        var patient = await _context.Patients
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
}