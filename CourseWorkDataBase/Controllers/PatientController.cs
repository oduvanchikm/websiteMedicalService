using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseWorkDataBase.Data;
using CourseWorkDataBase.Models;

namespace CourseWorkDataBase.Controllers;

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
}