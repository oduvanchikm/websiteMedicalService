using System.ComponentModel.DataAnnotations;

namespace CourseWorkDataBase.Models;

public class Clinic
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Address { get; set; } = String.Empty;
    
    [Required]
    public string PhoneNumber { get; set; } = String.Empty;
}