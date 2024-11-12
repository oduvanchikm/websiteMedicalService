using System.Security.Claims;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseWorkDataBase.DAL;
using CourseWorkDataBase.Data;
using CourseWorkDataBase.Models;
using CourseWorkDataBase.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace CourseWorkDataBase.Controllers;

[Authorize]
public class PatientController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly SlotGenerationService _slotService;
    private readonly ILogger<PatientController> _logger;

    public PatientController(ApplicationDbContext context, ILogger<PatientController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> PatientPage()
    {
        var doctors = await _context.Doctors
            .Include(d => d.Specialty)
            .ToListAsync();
        return View(doctors);
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
        Console.Out.WriteLine("BookAppointment1");

        var userId = GetCurrentUserId();
        
        var slot = await _context.AppointmentSlots
            .Where(x => x.Id == slotId)
            .FirstOrDefaultAsync();
        
        Console.Out.WriteLine("BookAppointment5");

        if (slot.IsBooked || slot == null)
        {
            Console.Out.WriteLine("BookAppointment52");
            return RedirectToAction("PatientPage", "Patient");
        }

        var patient = await _context.Patients
            .Where(p => p.UserId == userId)
            .FirstOrDefaultAsync();
        
        if (patient == null)
        {
            Console.Out.WriteLine("No patient found for user id " + userId);
        }
        
        Console.Out.WriteLine("Patient id " + patient.Id + " and user id " + userId);

        var appointment = new Appointment
        {
            AppointmentSlotId = slotId,
            PatientId = patient.Id,
            Date = DateTime.UtcNow,
            StatusId = 1
        };
        
        Console.Out.WriteLine("slot id " + appointment.AppointmentSlotId);
        Console.Out.WriteLine("patient id " + appointment.PatientId);
        Console.Out.WriteLine("date " + appointment.Date);
        Console.Out.WriteLine("statis id " + appointment.StatusId);
        
        Console.Out.WriteLine("BookAppointment6");

        slot.IsBooked = true;
        Console.Out.WriteLine("BookAppointment7");
        
        try
        {
            _context.Appointments.Add(appointment);
            _context.AppointmentSlots.Update(slot);
            await _context.SaveChangesAsync();
            Console.Out.WriteLine("BookAppointment9");
        }
        catch (DbUpdateConcurrencyException)
        {
            TempData["ErrorMessage"] = "There was an error with your booking. Try again.";
            return RedirectToAction("PatientPage", "Patient");
        }
        catch (Exception ex) 
        {
            _logger.LogError(ex.Message, "Error occurred while booking appointment.");
            TempData["ErrorMessage"] = "An unexpected error occurred. Please try again.";
            return RedirectToAction("PatientPage", "Patient");
        }
        
        Console.Out.WriteLine("BookAppointment12");
        return RedirectToAction("PatientAppointments", "Patient");
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
            _logger.LogError(ex, "Ошибка при получении записей на прием пациента.");
            return StatusCode(500, "Внутренняя ошибка сервера.");
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> CancelAppointment(long appointmentId)
    {
        var appointment = await _context.Appointments
            .Include(a => a.AppointmentSlot)
            .FirstOrDefaultAsync(a => a.Id == appointmentId);

        if (appointment == null || appointment.PatientId != GetCurrentUserId())
        {
            TempData["ErrorMessage"] = "Appointment not found or you do not have permission to cancel it.";
            return RedirectToAction("PatientAppointments");
        }

        if (appointment.StatusId != 1)
        {
            TempData["ErrorMessage"] = "Only scheduled appointments can be canceled.";
            return RedirectToAction("PatientAppointments");
        }

        appointment.StatusId = 3; 
        appointment.AppointmentSlot.IsBooked = false;

        _context.Appointments.Update(appointment);
        _context.AppointmentSlots.Update(appointment.AppointmentSlot);

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

        return RedirectToAction("PatientAppointments");
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

    // public async Task<IActionResult> MedicalRecord(long appointmentId)
    // {
    //     var record = await _context.
    //         .Include(r => r.Appointment)
    //         .ThenInclude(a => a.AppointmentSlot)
    //         .ThenInclude(s => s.Doctor)
    //         .ThenInclude(d => d.Clinic)
    //         .FirstOrDefaultAsync(r => r.AppointmentID == appointmentId);
    //
    //     if (record == null)
    //     {
    //         return NotFound();
    //     }
    //
    //     return View(record);
    // }
}