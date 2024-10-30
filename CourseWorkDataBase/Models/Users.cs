using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CourseWorkDataBase.Models;

public class Users
{
    [Key] 
    public int ID { get; set; }

    [Required] 
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required] 
    [MaxLength(100)]
    public string FamilyName { get; set; } = string.Empty;
    
    [Required] 
    [EmailAddress] 
    public string Email { get; set; } = string.Empty;

    [Required] 
    public string Password { get; set; } = string.Empty;

    [ForeignKey("Roles")] 
    public int RoleId { get; set; }
    public Roles Role { get; set; } = null!;

    [Required] 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<Appointments> Appointments { get; set; } = new HashSet<Appointments>();
}