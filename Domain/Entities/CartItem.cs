using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }

        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;


        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
