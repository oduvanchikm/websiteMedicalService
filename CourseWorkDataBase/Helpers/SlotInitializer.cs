using CourseWorkDataBase.Models;
using CourseWorkDataBase.DAL;
using Microsoft.EntityFrameworkCore;

namespace CourseWorkDataBase.Services;

public class SlotInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SlotInitializer> _logger;

    public SlotInitializer(ApplicationDbContext context, ILogger<SlotInitializer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task InitializeSlotAsync()
    {
        var doctors = await _context.Doctors
            .Include(d => d.AppointmentSlots)
            .ToListAsync();

        foreach (var doctor in doctors)
        {
            var startDate = DateTime.UtcNow.Date;
            var endDate = DateTime.UtcNow.Date.AddDays(14);

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                {
                    continue;
                }

                var workStartTime = DateTime.SpecifyKind(date.AddHours(9), DateTimeKind.Utc);
                var workEndTime = DateTime.SpecifyKind(date.AddHours(16), DateTimeKind.Utc);
                var breakStartTime = DateTime.SpecifyKind(date.AddHours(12), DateTimeKind.Utc);
                var breakEndTime = DateTime.SpecifyKind(date.AddHours(13), DateTimeKind.Utc); 

                var currentTime = workStartTime;

                while (currentTime.AddMinutes(45) <= workEndTime)
                {
                    if (currentTime >= breakStartTime && currentTime < breakEndTime)
                    {
                        currentTime = breakEndTime;
                        continue;
                    }

                    var existingSlot = doctor.AppointmentSlots
                        .FirstOrDefault(s => s.StartTime == currentTime);

                    if (existingSlot == null)
                    {
                        var slot = new AppointmentSlot
                        {
                            DoctorId = doctor.ID,
                            StartTime = currentTime,
                            EndTime = currentTime.AddMinutes(45)
                        };
                        _context.AppointmentSlots.Add(slot);
                    }

                    currentTime = currentTime.AddMinutes(45);
                }
            }
        }

        await _context.SaveChangesAsync();
    }
}