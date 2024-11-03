// using Microsoft.AspNetCore.Identity;
// using CourseWorkDataBase.Models;
//
// namespace CourseWorkDataBase.Data;
//
// public class AdminService
// {
//     private readonly UserManager<User> _userManager;
//     private readonly RoleManager<IdentityRole> _roleManager;
//     private readonly ApplicationDbContext _context;
//
//     public AdminService(
//         UserManager<User> userManager,
//         RoleManager<IdentityRole> roleManager,
//         ApplicationDbContext context)
//     {
//         _userManager = userManager;
//         _roleManager = roleManager;
//         _context = context;
//     }
//
//     public async Task<Doctor> CreateDoctorAsync(string email, string password, string firstName, string familyName, string specialtyId)
//     {
//         var existingUser = await _userManager.FindByEmailAsync(email);
//         if (existingUser != null)
//         {
//             throw new ApplicationException("Email уже зарегистрирован.");
//         }
//         
//         var user = new User()
//         {
//             Password = password,
//             Email = email,
//             CreatedAt = DateTime.UtcNow 
//         };
//         
//         var result = await _userManager.CreateAsync(user, password);
//         if (!result.Succeeded)
//         {
//             var errors = string.Join(", ", result.Errors.Select(e => e.Description));
//             throw new ApplicationException($"Ошибка при создании пользователя: {errors}");
//         }
//         
//         if (!await _roleManager.RoleExistsAsync("Doctor"))
//         {
//             await _roleManager.CreateAsync(new IdentityRole("Doctor"));
//         }
//         
//         await _userManager.AddToRoleAsync(user, "Doctor");
//         
//         var doctor = new Doctor
//         {
//             FirstName = firstName,
//             FamilyName = familyName,
//             SpecialtyID = specialtyId
//         };
//
//         _context.Doctors.Add(doctor);
//         await _context.SaveChangesAsync();
//         
//         return doctor;
//     }
// }