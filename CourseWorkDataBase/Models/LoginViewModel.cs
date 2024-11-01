using System.ComponentModel.DataAnnotations;

namespace CourseWorkDataBase.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "Пожалуйста, введите имя пользователя.")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Пожалуйста, введите пароль.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}