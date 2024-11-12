// using Microsoft.AspNetCore.Identity;
// using CourseWorkDataBase.Models;
// using BCrypt.Net;
// using Microsoft.Extensions.DependencyInjection;
// using System;
// using System.Threading.Tasks;
//
// namespace CourseWorkDataBase.Helpers;
//
// public static class RoleInitializer
// {
//     private static readonly string[] Roles = { "Admin", "Doctor", "Patient" };
//
//     public static async Task InitializeAsync(IServiceProvider serviceProvider, ILogger<Role> logger)
//     {
//         // Получаем менеджеры ролей и пользователей
//         var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
//         var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
//
//         // Инициализация ролей
//         foreach (var role in Roles)
//         {
//             if (!await roleManager.RoleExistsAsync(role))
//             {
//                 var normalizedRole = role.ToUpper();
//                 var roleResult = await roleManager.CreateAsync(new Role { Name = role, NormalizedName = normalizedRole });
//
//                 if (!roleResult.Succeeded)
//                 {
//                     var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
//                     logger.LogError($"Не удалось создать роль '{role}': {errors}");
//                     throw new Exception($"Не удалось создать роль '{role}': {errors}");
//                 }
//             }
//         }
//
//         // Данные администратора
//         string adminEmail = "mkgubareva2005@gmail.com";
//         string adminPassword = "Admin123!"; // Убедитесь, что пароль соответствует требованиям
//
//         // Поиск администратора по Email
//         var adminUser = await userManager.FindByEmailAsync(adminEmail);
//
//         if (adminUser == null)
//         {
//             var newAdmin = new User
//             {
//                 Email = adminEmail,
//                 UserName = adminEmail, // Установка UserName
//                 CreatedAt = DateTimeOffset.UtcNow,
//                 // Другие необходимые свойства
//             };
//             
//             // Создание пользователя с паролем
//             var result = await userManager.CreateAsync(newAdmin, adminPassword);
//             if (result.Succeeded)
//             {
//                 // Назначение роли 'Admin'
//                 var addToRoleResult = await userManager.AddToRoleAsync(newAdmin, "Admin");
//                 if (!addToRoleResult.Succeeded)
//                 {
//                     var errors = string.Join(", ", addToRoleResult.Errors.Select(e => e.Description));
//                     logger.LogError($"Не удалось добавить администратора в роль 'Admin': {errors}");
//                     throw new Exception($"Не удалось добавить администратора в роль 'Admin': {errors}");
//                 }
//             }
//             else
//             {
//                 // Логирование ошибок создания пользователя
//                 foreach (var error in result.Errors)
//                 {
//                     logger.LogError($"Ошибка при создании администратора: {error.Description}");
//                 }
//                 
//                 var errorsList = string.Join(", ", result.Errors.Select(e => e.Description));
//                 throw new Exception("Не удалось создать администратора: " + errorsList);
//             }
//         }
//     }
// }