using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;

        public string FullName      { get; set; } = string.Empty;
        public string Phone         { get; set; } = string.Empty;
        public string City          { get; set; } = string.Empty;
        public string Address       { get; set; } = string.Empty;
        public string? PostalCode   { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
