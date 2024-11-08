using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CourseWorkDataBase.Models;

public class User
{
    public long Id { get; set; }
    
    public string Email { get; set; }
    
    public string? Password { get; set; }
    
    public string? PersonalNumber { get; set; }
    
    public Role Role { get; set; }
    public long RoleId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Patient? Patient { get; set; }
    
    public Doctor? Doctor { get; set; }
}