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

    public async Task<IActionResult> ViewDoctor(long doctorId)
    {
        var slots = await _context.AppointmentSlots
            .Where(s => s.DoctorId == doctorId && s.StartTime >= DateTime.Today)
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
    // public async Task<IActionResult> BookAppointment(long slotId)
    // {
    //     var slot = await _context.AppointmentSlots
    //         .Include(s => s.Doctor)
    //         .FirstOrDefaultAsync(s => s.Id == slotId);
    //
    //     if (slot == null || slot.IsBooked)
    //     {
    //         return RedirectToAction("PatientPage");
    //     }
    //
    //     var appointment = new Appointment
    //     {
    //         AppointmentSlotId = slot.Id,
    //         PatientId = 
    //          = DateTime.UtcNow
    //     };
    //
    //     slot.IsBooked = true; // Mark the slot as booked
    //
    //     _context.Appointments.Add(appointment);
    //     _context.Entry(slot).State = EntityState.Modified;
    //
    //     await _context.SaveChangesAsync();
    //     
    //     return RedirectToAction("MyAppointments");
    // }

    // public async Task<IActionResult> MyAppointments()
    // {
    //     var userId = /* Get current patient's ID, e.g., from User.Identity */;
    //     var appointments = await _context.Appointments
    //         .Include(a => a.Slot)
    //         .ThenInclude(s => s.Doctor)
    //         .Where(a => a.PatientId == userId)
    //         .OrderByDescending(a => a.BookedAt)
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