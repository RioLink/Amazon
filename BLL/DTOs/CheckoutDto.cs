using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    /// <summary>
    /// Дані, які користувач заповнює при оформленні замовлення.
    /// </summary>
    public class CheckoutDto
    {
        [Required(ErrorMessage = "Введіть повне ім'я")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть email")]
        [EmailAddress(ErrorMessage = "Некоректний формат email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть номер телефону")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть місто")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть адресу доставки")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть поштовий індекс")]
        public string PostalCode { get; set; } = string.Empty;

        /// <summary>card | cash | digital</summary>
        public string PaymentMethod { get; set; } = "card";
    }
}
