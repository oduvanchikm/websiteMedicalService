using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using CourseWorkDataBase.Data;
using Microsoft.AspNetCore.Identity;
using CourseWorkDataBase.DAL;
using CourseWorkDataBase.Helpers;
using CourseWorkDataBase.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Authorization/AuthorizationPage";
        options.AccessDeniedPath = "/Authorization/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(1); 
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("DoctorPolicy", policy => policy.RequireRole("Doctor"));
    options.AddPolicy("PatientPolicy", policy => policy.RequireRole("Patient"));
});

builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<DoctorService>();
builder.Services.AddScoped<RegistrationService>();
builder.Services.AddScoped<AuthorizationService>();
builder.Services.AddScoped<SlotInitializer>();
builder.Services.AddHostedService<SlotGenerationService>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Authorization/Login";
    options.LogoutPath = "/Authorization/Logout";
    options.AccessDeniedPath = "/Authorization/AccessDenied";
});

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

        var context = services.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();
        logger.LogInformation("Применение миграций прошло успешно.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Произошла ошибка при инициализации слотов или базы данных.");
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
    name: "admin/add",
    pattern: "{controller=Admin}/{action=AddDoctor}/{id?}");

app.MapControllerRoute(
    name: "admin",
    pattern: "{controller=Admin}/{action=DoctorsList}/{id?}");

app.Run();