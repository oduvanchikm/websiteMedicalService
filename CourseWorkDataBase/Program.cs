using Microsoft.EntityFrameworkCore;
using CourseWorkDataBase.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// builder.Services.AddAuthorization(options =>
// {
//     options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
//     options.AddPolicy("Doctor", policy => policy.RequireRole("Doctor"));
//     options.AddPolicy("Patient", policy => policy.RequireRole("Patient"));
// });

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

// builder.WebHost.ConfigureKestrel(serverOptions =>
// {
//     serverOptions.ListenAnyIP(7241, listenOptions =>
//     {
//         listenOptions.UseHttps();
//     });
// });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    await DbInitializer.InitializeAsync(context);
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

// Дополнительный маршрут (по желанию)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Определение маршрутов
app.MapControllerRoute(
    name: "patient",
    pattern: "{controller=Patient}/{action=PatientPage}/{id?}");


app.Run();