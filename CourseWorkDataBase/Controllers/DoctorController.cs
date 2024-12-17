using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using CourseWorkDataBase.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Rendering;
using CourseWorkDataBase.Services;
using CourseWorkDataBase.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CourseWorkDataBase.Controllers;

[Authorize("DoctorPolicy")]
public class DoctorController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DoctorController> _logger;
    private readonly DoctorService _doctorService;

    public DoctorController(ApplicationDbContext context, 
        ILogger<DoctorController> logger,
        DoctorService doctorService)
    {
        _context = context;
        _logger = logger;
        _doctorService = doctorService;
    }
    
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("AuthorizationPage", "Authorization");
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

    public async Task<IActionResult> DoctorPage()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            _logger.LogWarning("User is not logged in.");
            return NotFound();
        }
        
        var doctor = await _context.Doctors
            .Where(d => d.UserId == userId)
            .FirstOrDefaultAsync();
        if (doctor == null)
        {
            _logger.LogWarning("Doctor is not logged in.");
            return NotFound();
        }
        
        var appointmentSlots = await _context.AppointmentSlots
            .Include(a => a.Appointment)
            .ThenInclude(ap => ap.Patient)
            .Where(a => a.DoctorId == doctor.ID && a.IsBooked == true)
            .ToListAsync();
        return View(appointmentSlots);
    }
    
    private async Task<IEnumerable<SelectListItem>> GetMedicationsSelectListAsync()
    {
        var medications = await _context.Medications
            .OrderBy(x => x.Name)
            .ToListAsync();
    
        return medications.Select(s => new SelectListItem
        {
            Value = s.MedicationId.ToString(),
            Text = s.Name
        }).ToList();
    }

    [HttpGet]
    public async Task<IActionResult> AddMedicalRecords(long id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Invalid appointment ID.");
            return NotFound();
        }

        var medications = await GetMedicationsSelectListAsync();
    
        var model = new AddMedicalRecordsViewModel
        {
            MedicationsList = medications,
            AppointmentId = id
        };
        
        Console.Out.WriteLine($"in get : {model.AppointmentId}");
        
        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddMedicalRecords(AddMedicalRecordsViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var medicationsList = await _context.Medications.ToListAsync();
            model.MedicationsList = medicationsList.Select(m => new SelectListItem
            {
                Value = m.MedicationId.ToString(),
                Text = m.Name
            });
            return View(model);
        }

        try
        {
            var medicalRecords = await _doctorService.AddMedicalRecordsAsync(
                model.AppointmentId,
                model.Description,
                model.Diagnosis,
                model.MedicationID,
                model.NameMedication,
                model.DescriptionMedication);
            
            return RedirectToAction("DoctorPage", "Doctor");
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null)
            {
                Console.Out.WriteLine($"Internal error: {ex.InnerException.Message}");
            }
            ModelState.AddModelError("", "An unknown error has occurred. Please try again later.");
        }
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> ShowMedicalRecords(long id) // это айдишником пока что бцдет пациенат
    {
        if (id <= 0)
        {
            _logger.LogWarning("Invalid patient ID in show medical records.");
            return NotFound();
        }

        var patient = await _context.Patients
            .AsNoTracking()
            .Include(p => p.Appointments)
            .ThenInclude(appt => appt.MedicalRecords)
            .ThenInclude(mr => mr.MedicalRecordMedications)
            .ThenInclude(mrm => mrm.Medication)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (patient == null)
        {
            _logger.LogWarning($"Patient with ID {id} not found.");
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
}