using System.ComponentModel.DataAnnotations;

namespace CourseWorkDataBase.Models;

public class Clinic
{
    [Key]
    public long Id { get; set; }
    
    [Required]
    public string Address { get; set; }
    
    [Required]
    public string PhoneNumber { get; set; } 
}