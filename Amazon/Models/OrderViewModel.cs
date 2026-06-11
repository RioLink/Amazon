using Domain.Enums;

namespace Amazon.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }

        // User
        public string UserEmail    { get; set; } = "";
        public string UserName     { get; set; } = "";

        // Delivery
        public string FullName      { get; set; } = "";
        public string Phone         { get; set; } = "";
        public string City          { get; set; } = "";
        public string Address       { get; set; } = "";
        public string? PostalCode   { get; set; }
        public string PaymentMethod { get; set; } = "";
    }
}
