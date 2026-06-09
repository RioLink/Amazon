using Domain.Enums;

namespace Amazon.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } 
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
    }
}
