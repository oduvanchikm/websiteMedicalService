// using Microsoft.AspNetCore.Mvc;
// using CourseWorkDataBase.Data;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.EntityFrameworkCore;
//
// namespace CourseWorkDataBase.Controllers;
//
//
// [Authorize(Roles = "Admin")]
// public class AdminController : Controller
// {
//     private readonly AdminService _adminService;
//     private readonly ApplicationDbContext _context;
//
//     public AdminController(AdminService adminService, ApplicationDbContext context)
//     {
//         _adminService = adminService;
//         _context = context;
//     }
//
//     [HttpGet]
//     public IActionResult CreateDoctor()
//     {
//         var specialties = _context.Specialties.ToList();
//         var model = new CreateDoctorViewModel
//         {
//             Specialties = specialties
//         };
//         return View(model);
//     }
//
//     [HttpPost]
//     public async Task<IActionResult> CreateDoctor(CreateDoctorViewModel model)
//     {
//         if(ModelState.IsValid)
//         {
//             try
//             {
//                 var doctor = await _adminService.CreateDoctorAsync(
//                     model.Email,
//                     model.Password,
//                     model.FirstName,
//                     model.FamilyName,
//                     model.ContactInfo,
//                     model.SpecialtyId
//                 );
//                 return RedirectToAction("DoctorsList");
//             }
//             catch(Exception ex)
//             {
//                 ModelState.AddModelError("", ex.Message);
//             }
//         }
//
//         model.Specialties = _context.Specialties.ToList();
//         return View(model);
//     }
//
//     [HttpGet]
//     public IActionResult DoctorsList()
//     {
//         var doctors = _context.Doctors.Include(d => d.Specialty).ToList();
//         return View(doctors);
//     }
// }