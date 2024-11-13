using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using CourseWorkDataBase.DAL;
using Microsoft.AspNetCore.Authorization;
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

    // [HttpGet]
    // public IActionResult DoctorPage()
    // {
    //     return View();
    // }
    
    private long GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && long.TryParse(userIdClaim.Value, out long userId))
        {
            return userId;
        }
        
        throw new InvalidOperationException("Couldn't get the ID of the current user.");
    }
    
    // [HttpGet]
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
    
        // Get a list of booked appointment slots
        var appointmentSlots = await _context.AppointmentSlots
            .Include(a => a.Appointment)
            .ThenInclude(ap => ap.Patient)
            .Where(a => a.DoctorId == doctor.ID && a.IsBooked == true)
            .ToListAsync();

        Console.Out.WriteLine($"Current doctor {doctor.ID} has {appointmentSlots.Count} booked appointments.");

        return View(appointmentSlots);
    }
    
    // [HttpPost]
    // [ValidateAntiForgeryToken]
    // public async Task<IActionResult> CreateSlot(AppointmentSlot model)
    // {
    //     if (ModelState.IsValid)
    //     {
    //         var doctorID = model.DoctorId;
    //         _context.AppointmentSlots.Add(model);
    //         await _context.SaveChangesAsync();
    //         return RedirectToAction(nameof());
    //     }
    //
    //     return View(model);
    // }
    
    // public async Task<IActionResult> DeleteSlot(long id)
    // {
    //     var slot = await _context.AppointmentSlots.FindAsync(id);
    //     if (slot == null || slot.IsBooked)
    //     {
    //         return NotFound();
    //     }
    //     
    //     _context.AppointmentSlots.Remove(slot);
    //     await _context.SaveChangesAsync();
    //     return RedirectToAction(nameof(ManageSlots));
    // }
}