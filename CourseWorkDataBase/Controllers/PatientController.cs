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

// [Authorize(Roles = "Patient")]
public class PatientController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly SlotGenerationService _slotService;

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
    
    [HttpPost]
    public async Task<IActionResult> BookAppointment(long slotId)
    {
        Console.Out.WriteLine("BookAppointment1");
        try
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("AuthorizationPage", "Authorization");
            }
            Console.Out.WriteLine("BookAppointment2");
            
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim == null)
            {
                return RedirectToAction("AuthorizationPage", "Authorization");
            }
            Console.Out.WriteLine("BookAppointment3");

            if (!long.TryParse(userIdClaim.Value, out long userId))
            {
                return RedirectToAction("AuthorizationPage", "Authorization");
            }
            Console.Out.WriteLine("BookAppointment4");
        
            var slot = await _context.AppointmentSlots
                .Include(s => s.Doctor)
                .FirstOrDefaultAsync(s => s.Id == slotId);
    
            if (slot == null || slot.IsBooked)
            {
                Console.Out.WriteLine("BookAppointment4");
                return RedirectToAction("PatientPage", "Patient");
            }
            Console.Out.WriteLine("BookAppointment5");
    
            var appointment = new Appointment
            {
                AppointmentSlotId = slot.Id,
                PatientId = userId,
                Date = DateTime.UtcNow,
                StatusId = 1
            };
            Console.Out.WriteLine("BookAppointment6");
    
            slot.IsBooked = true; 
            Console.Out.WriteLine("BookAppointment7");
    
            _context.Appointments.Add(appointment);
            Console.Out.WriteLine("BookAppointment8");
            _context.Entry(slot).State = EntityState.Modified;
            Console.Out.WriteLine("BookAppointment9");
        
            try
            {
                Console.Out.WriteLine("BookAppointment10");
                await _context.SaveChangesAsync();
                Console.Out.WriteLine("BookAppointment11");
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["ErrorMessage"] = "There was an error with your booking. Try again.";
                return RedirectToAction("PatientPage", "Patient");
            }
        }
        catch (Exception e)
        {
            Console.Out.WriteLine(e);
            throw;
        }
        Console.Out.WriteLine("BookAppointment12");
        return RedirectToAction("PatientAppointments", "Patient");
    }

    public async Task<IActionResult> PatientAppointments(long id)
    {
        var user = await _context.Patients
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (user == null)
        {
            return NotFound();
        }
        
        var appointments = await _context.Appointments
            .Include(a => a.AppointmentSlot)
            .ThenInclude(s => s.Doctor)
            .Where(a => a.PatientId == user.Id)
            .OrderBy(a => a.AppointmentSlot.StartTime) 
            .ToListAsync();
    
        return View(appointments);
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