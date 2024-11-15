using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseWorkDataBase.Models;

public class Appointment
{
    public long Id { get; set; }
    
    public Patient Patient { get; set; }
    public long PatientId { get; set; }
    
    public AppointmentSlot AppointmentSlot { get; set; }
    public long AppointmentSlotId { get; set; }
    
    public DateTimeOffset Date { get; set; } = DateTime.UtcNow;

    public Status Status { get; set; }
    public long StatusId { get; set; }
    
    // public long? MedicalRecordsId { get; set; }
    // public MedicalRecords? MedicalRecords { get; set; }
    
    public ICollection<MedicalRecords> MedicalRecords { get; set; }
}