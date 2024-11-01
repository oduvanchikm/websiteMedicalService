using System.ComponentModel.DataAnnotations;

namespace CourseWorkDataBase.Models;

public class Role
{
    public long Id { get; set; }

    [Required] 
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}