using CourseWorkDataBase.Models;
using CourseWorkDataBase.DAL;
using Microsoft.EntityFrameworkCore;

namespace CourseWorkDataBase.Helpers;

public class SlotInitializer
{
    private readonly ApplicationDbContext _context;

    public SlotInitializer(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task InitializeSlotAsync()
    {
        var doctors = _context.Doctors
            .Include(d => d.AppointmentSlots)
            .ToList();

        foreach (var doctor in doctors)
        {
            var startDate = DateTime.UtcNow.Date;
            var endDate = DateTime.UtcNow.Date.AddDays(30);

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                {
                    continue;
                }

                var slotStartTime = DateTime.SpecifyKind(date.AddHours(9), DateTimeKind.Utc);
                var slotEndTime = DateTime.SpecifyKind(date.AddHours(16), DateTimeKind.Utc);
                var currentTime = slotStartTime;

                while (currentTime.AddMinutes(45) <= slotEndTime)
                {
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