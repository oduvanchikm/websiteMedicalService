using System.ComponentModel.DataAnnotations;

namespace CourseWorkDataBase.Models;

public class Roles
{
    [Key] 
    public int Id { get; set; }

    [Required] 
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public virtual ICollection<Users> Users { get; set; } = new HashSet<Users>();
}