using Microsoft.EntityFrameworkCore;

namespace CourseWorkDataBase.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(ApplicationDbContext context)
    {
        await context.Database.MigrateAsync();
    }
}