using System.ComponentModel.DataAnnotations;

namespace CourseWorkDataBase.Models;

public class Clinic
{
    public long Id { get; set; }
 
    public string Address { get; set; }
    
    public string PhoneNumber { get; set; } 
    
    public List<Doctor> Doctors { get; set; }
}