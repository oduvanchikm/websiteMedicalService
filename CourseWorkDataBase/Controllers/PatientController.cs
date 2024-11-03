using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseWorkDataBase.Data;
using CourseWorkDataBase.Models;
using Microsoft.AspNetCore.Authorization;

namespace CourseWorkDataBase.Controllers;

// [Authorize(Roles = "Patient")]
public class PatientController : Controller
{
    private readonly ApplicationDbContext _context;

    public PatientController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> PatientPage()
    {
        var doctors = await _context.Doctors
            .Include(d => d.Specialty)
            .ToListAsync();
        return View(doctors);
    }

    public async Task<IActionResult> Watch(long doctorId)
    {
        var slots = await _context.AppointmentSlots
            .Where(s => s.DoctorId == doctorId && s.StartTime >= DateTime.Today && s.Appointment == null)
            .OrderBy(s => s.StartTime)
            .ToListAsync();

        var doctor = await _context.Doctors.Include(d => d.Specialty)
            .FirstOrDefaultAsync(d => d.ID == doctorId);
        if (doctor == null)
        {
            return NotFound();
        }

        var viewModel = new BookAppointmentViewModel
        {
            Doctor = doctor,
            AvailableSlots = slots
        };

        return View(viewModel);
    }
    
    // [HttpPost]
    // [ValidateAntiForgeryToken]
    // public async Task<IActionResult> Book(long slotId)
    // {
    //     var slot = await _context.AppointmentSlots.Include(s => s.Appointment).FirstOrDefaultAsync(s => s.Id == slotId);
    //     if (slot == null || slot.IsBooked)
    //     {
    //         return NotFound();
    //     }
    //
    //     var appointment = new Appointment
    //     {
    //         AppointmentSlotId = slotId,
    //         Id = User.FindFirstValue(ClaimTypes.NameIdentifier),
    //          = DateTime.UtcNow
    //     };
    //
    //     _context.Appointments.Add(appointment);
    //     await _context.SaveChangesAsync();
    //
    //     return RedirectToAction("MyAppointments");
    // }

    // public async Task<IActionResult> MyAppointments()
    // {
    //     var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    //     var appointments = await _context.Appointments
    //         .Include(a => a.AppointmentSlot)
    //         .ThenInclude(s => s.Doctor)
    //         .Include(a => a.AppointmentSlot)
    //         .ThenInclude(s => s.Doctor)
    //         .ThenInclude(d => d.Specialty.ClinicId)
    //         .Where(a => a.Id == userId)
    //         .OrderBy(a => a.AppointmentSlot.StartTime)
    //         .ToListAsync();
    //
    //     return View(appointments);
    // }

    // Просмотр медицинской карты
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