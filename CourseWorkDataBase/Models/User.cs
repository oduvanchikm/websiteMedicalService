using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CourseWorkDataBase.Models;

public class User
{
    public long Id { get; set; }
    
    [Required] 
    [EmailAddress] 
    public string Email { get; set; }

    [Required] 
    public string Password { get; set; }

    [ForeignKey("Role")] 
    public long RoleId { get; set; }
    public Role Role { get; set; }

    [Required] 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}