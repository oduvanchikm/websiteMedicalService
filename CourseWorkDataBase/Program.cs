using Microsoft.EntityFrameworkCore;
using CourseWorkDataBase.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.AddScoped<RegistrationService>();
builder.Services.AddScoped<AuthorizationService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        await DbInitializer.InitializeAsync(context);
        logger.LogInformation("Применение миграций прошло успешно.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Произошла ошибка при инициализации базы данных.");
        throw;
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "authorize",
    pattern: "{controller=Authorization}/{action=AuthorizationPage}/{id?}");

app.MapControllerRoute(
    name: "register",
    pattern: "{controller=Register}/{action=RegisterPage}/{id?}");

// app.MapControllerRoute(
//     name: "patient/user",
//     pattern: "{controller=Patient}/{action=PatientPage}/{id?}");
//
// app.MapControllerRoute(
//     name: "patient/user",
//     pattern: "{controller=Patient}/{action=PatientPage}/{id?}");


app.Run();