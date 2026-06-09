using System.ComponentModel.DataAnnotations;

namespace Amazon.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Введіть логін або пошту")]
        [Display(Name = "Логін або пошта")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = string.Empty;
    }
}
