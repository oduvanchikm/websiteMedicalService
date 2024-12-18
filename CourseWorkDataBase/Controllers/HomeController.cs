using Microsoft.AspNetCore.Mvc;
using CourseWorkDataBase.DAL;
using Microsoft.EntityFrameworkCore;

namespace CourseWorkDataBase.Controllers;

public class HomeController : Controller
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    
    public HomeController(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public IActionResult Index()
    {
        return View();
    }
}