namespace CourseWorkDataBase.Data;

using CourseWorkDataBase.Models;

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        context.Database.EnsureCreated();

        if (context.User.Any())
        {
            return;
        }

        context.SaveChanges();
    }
}