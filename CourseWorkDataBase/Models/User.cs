using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using Microsoft.AspNetCore.Identity;

namespace CourseWorkDataBase.Models;

public class User
{
    public long Id { get; set; }
    
    public string Email { get; set; }
    
    public string Password { get; set; }
    
    // public string? PersonalNumber { get; set; }
    
    public Role Role { get; set; }
    public long RoleId { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Patient? Patient { get; set; }
    
    public Doctor? Doctor { get; set; }
}