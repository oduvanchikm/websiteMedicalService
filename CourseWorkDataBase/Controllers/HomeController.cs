using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseWorkDataBase.Data;
using CourseWorkDataBase.Models;

namespace CourseWorkDataBase.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var doctors = await _context.Doctors
            .Include(d => d.Specialty)
            .Include(d => d.Appointments)
            .ThenInclude(a => a.User) // Предположим, что пользователь – это пациент
            .ToListAsync();

        return View(doctors);
    }

    // Действие для отображения главной страницы
    public IActionResult MainPage()
    {
        return View();
    }

    // Действие для отображения страницы ошибки (опционально)
    public IActionResult Error()
    {
        return View();
    }
}