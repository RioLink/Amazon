using System.ComponentModel.DataAnnotations;

namespace Amazon.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введіть логін")]
        [StringLength(32, MinimumLength = 3, ErrorMessage = "Логін: від 3 до 32 символів")]
        [RegularExpression(@"^[a-zA-Z0-9_.-]+$", ErrorMessage = "Логін може містити лише латиницю, цифри, «_», «.» та «-»")]
        [Display(Name = "Логін")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть пошту")]
        [EmailAddress(ErrorMessage = "Невірний формат email")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть пароль")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Пароль: мінімум 8 символів")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Підтвердіть пароль")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Паролі не збігаються")]
        [Display(Name = "Підтвердження паролю")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
