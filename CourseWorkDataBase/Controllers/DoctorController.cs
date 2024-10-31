using Microsoft.AspNetCore.Mvc;
using CourseWorkDataBase.Data;

namespace CourseWorkDataBase.Controllers;

public class DoctorController : Controller
{
    private readonly ApplicationDbContext _context;

    public DoctorController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> DoctorPage()
    {
        return View();
    }
}