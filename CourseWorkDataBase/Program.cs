using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using CourseWorkDataBase.Data;
using CourseWorkDataBase.DAL;
using CourseWorkDataBase.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
    });

builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<RegistrationService>();
builder.Services.AddScoped<AuthorizationService>();
builder.Services.AddScoped<SlotInitializer>();

builder.Services.AddHostedService<SlotGenerationService>();


builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var initializer = services.GetRequiredService<SlotInitializer>();
        await initializer.InitializeSlotAsync();
        logger.LogInformation("Слоты успешно инициализированы.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Произошла ошибка при инициализации слотов.");
        throw;
    }
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
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

app.MapControllerRoute(
    name: "doctor",
    pattern: "{controller=Doctor}/{action=DoctorPage}/{id?}");

app.MapControllerRoute(
    name: "patient",
    pattern: "{controller=Patient}/{action=PatientPage}/{id?}");

app.MapControllerRoute(
    name: "admin",
    pattern: "{controller=Admin}/{action=AddDoctor}/{id?}");

app.MapControllerRoute(
    name: "admin",
    pattern: "{controller=Admin}/{action=AddDoctor}/{id?}");

app.MapControllerRoute(
    name: "admin/added",
    pattern: "{controller=Admin}/{action=AddedDoctor}/{id?}");

app.Run();