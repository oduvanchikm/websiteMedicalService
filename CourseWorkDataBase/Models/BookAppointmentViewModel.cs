namespace CourseWorkDataBase.Models;

public class BookAppointmentViewModel
{
    public Doctor Doctor { get; set; }
    public IEnumerable<AppointmentSlot> AvailableSlots { get; set; }
    
    public long? SelectedSlotId { get; set; }
}