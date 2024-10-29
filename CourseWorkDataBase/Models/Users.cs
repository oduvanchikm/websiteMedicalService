using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace CourseWorkDataBase.Models;

public class Users
{
    [Key] 
    public int ID { get; set; }
    
    [Required] 
    public string Name { get; set; } = string.Empty;
    
    [Required] 
    public string FamilyName { get; set; } = string.Empty;
    
    [EmailAddress] 
    [Required] 
    public string Email { get; set; } = string.Empty;
    
    [Required] 
    public string Password { get; set; } = string.Empty;

    [Required]
    public int RoleID { get; set; }

    public Roles Role { get; set; } = null!;
    
    [Required] 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}