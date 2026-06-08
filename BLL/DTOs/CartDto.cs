using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTOs
{
    // це сама корзина, яка містить список товарів та загальну вартість
    public class CartDto
    {
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        public decimal TotalPrice { get; set; } 
    }
    // Товар в корзине 
    public class CartItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Price * Quantity; 
    }
}
