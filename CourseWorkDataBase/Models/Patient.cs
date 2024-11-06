using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseWorkDataBase.Models;

public class Patient
{
    public long Id { get; set; }
    
    public string FirstName { get; set; }
    
    public string FamilyName { get; set; }
    
    public string Gender { get; set; }
    
    public DateTime Date { get; set; } = DateTime.UtcNow;
    
    public User User { get; set; }
    public long UserId { get; set; }
    
    public List<Appointment> Appointments { get; set; }
}