using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CourseWorkDataBase.Models;

public class Role : IdentityRole<long>
{
    public long Id { get; set; }
    public string Name { get; set; }
    
    public List<User> User { get; set; }
}