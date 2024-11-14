using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using CourseWorkDataBase.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using CourseWorkDataBase.Models;
using CourseWorkDataBase.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CourseWorkDataBase.Controllers;

[Authorize]
public class DoctorController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DoctorController> _logger;

    public DoctorController(ApplicationDbContext context, ILogger<DoctorController> logger)
    {
        _context = context;
        _logger = logger;
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

        Console.Out.WriteLine($"Current user: {userId}");

        var doctor = await _context.Doctors
            .Where(d => d.UserId == userId)
            .FirstOrDefaultAsync();

        if (doctor == null)
        {
            _logger.LogWarning("Doctor is not logged in.");
            return NotFound();
        }
    
        Console.Out.WriteLine($"Current doctor: {doctor.ID}");
        
        var appointmentSlots = await _context.AppointmentSlots
            .Include(a => a.Appointment)
            .ThenInclude(ap => ap.Patient)
            .Where(a => a.DoctorId == doctor.ID && a.IsBooked == true)
            .ToListAsync();

        Console.Out.WriteLine($"Current doctor {doctor.ID} has {appointmentSlots.Count} booked appointments.");

        return View(appointmentSlots);
    }

    [HttpGet]
    public async Task<IActionResult> AddMedicalRecords(long id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Invalid appointment ID.");
            return NotFound();
        }
    
        var appointment = await _context.Appointments
            .Include(a => a.Patient)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment == null)
        {
            _logger.LogWarning($"Appointment with ID {id} not found.");
            return NotFound();
        }

        var medications = await _context.Medications
            .ToListAsync();
    
        var viewModel = new AddMedicalRecordsViewModel
        {
            MedicationsList = medications.Select(m => new SelectListItem
            {
                Value = m.MedicationId.ToString(),
                Text = m.Name
            }),
            AppointmentId = appointment.Id
        };

        return View(viewModel);
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
        
        Console.Out.WriteLine($"Adding Medical Records: {model.MedicationsList.Count()}");

        var appointment = await _context.Appointments
            .Include(a => a.Patient)
            .FirstOrDefaultAsync(a => a.Id == model.AppointmentId);
        if (appointment == null)
        {
            return NotFound();
        }
        
        Console.Out.WriteLine($"Adding Medical Records: {appointment.Id}");

        var medicalRecord = new MedicalRecords
        {
            Description = model.Description,
            Diagnosis = model.Diagnosis,
            CreatedAt = DateTime.UtcNow,
            UpdateAt = DateTime.UtcNow,
            Id = appointment.Id
        };
        
        Console.Out.WriteLine($"Ae: {model.Description}");
        Console.Out.WriteLine($"ARecords: {model.Diagnosis}");

        if (model.SelectedMedicationIds != null && model.SelectedMedicationIds.Any())
        {
            foreach (var medicationId in model.SelectedMedicationIds)
            {
                var medication = await _context.Medications.FindAsync(medicationId);
                // if (medication != null)
                // {
                //     medicalRecord.MedicalRecordMedications.Add(new MedicalRecordMedication
                //     {
                //         MedicationId = medicationId,
                //         Medication = medication
                //     });
                // }
            }
        }

        _context.MedicalRecords.Add(medicalRecord);
        await _context.SaveChangesAsync();
        return RedirectToAction("DoctorPage", "Doctor");
    }
}