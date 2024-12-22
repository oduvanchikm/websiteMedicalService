using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseWorkDataBase.Models;

public class Doctor
{
    public long ID { get; set; }
    
    public string FirstName { get; set; }
    
    public string FamilyName { get; set; }
    
    public Specialty Specialty { get; set; }
    public long SpecialtyID { get; set; }
    
    public User? User { get; set; }
    public long UserId { get; set; }
    
    public Clinic Clinic { get; set; }
    public long? ClinicId { get; set; }
    
    public List<AppointmentSlot> AppointmentSlots { get; set; }
}