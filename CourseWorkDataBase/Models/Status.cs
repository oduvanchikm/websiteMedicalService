using System.ComponentModel.DataAnnotations;

namespace CourseWorkDataBase.Models;

public class Status
{
    public long Id { get; set; }

    public string Name{ get; set; } 
    
    public List<Appointment> Appointments { get; set; }
}