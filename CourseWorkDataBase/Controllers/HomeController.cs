using Microsoft.AspNetCore.Mvc;
using CourseWorkDataBase.Data;

namespace CourseWorkDataBase.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }
}