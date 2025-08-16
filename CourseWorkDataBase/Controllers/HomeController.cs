using Microsoft.AspNetCore.Mvc;
using CourseWorkDataBase.DAL;
using Microsoft.EntityFrameworkCore;

namespace CourseWorkDataBase.Controllers;

public class HomeController(IDbContextFactory<ApplicationDbContext> dbContextFactory) : Controller
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory = dbContextFactory;

    public IActionResult Index()
    {
        return View();
    }
}