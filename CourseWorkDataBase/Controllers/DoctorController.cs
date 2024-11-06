using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using CourseWorkDataBase.DAL;
using Microsoft.AspNetCore.Authorization;
using CourseWorkDataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseWorkDataBase.Controllers;

public class DoctorController : Controller
{
    private readonly ApplicationDbContext _context;

    public DoctorController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult DoctorPage()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> ManageSlots(AppointmentSlot model)
    {
        if (model == null)
        {
            return NotFound();
        }
        
        var userId = model.DoctorId;
        var slots = await _context.AppointmentSlots
            .Where(s => s.DoctorId == userId && s.StartTime >= DateTime.Today)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    
        return View(slots);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSlot(AppointmentSlot model)
    {
        if (ModelState.IsValid)
        {
            var doctorID = model.DoctorId;
            _context.AppointmentSlots.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ManageSlots));
        }
    
        return View(model);
    }
    
    public async Task<IActionResult> DeleteSlot(long id)
    {
        var slot = await _context.AppointmentSlots.FindAsync(id);
        if (slot == null || slot.IsBooked)
        {
            return NotFound();
        }
        
        _context.AppointmentSlots.Remove(slot);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(ManageSlots));
    }
}