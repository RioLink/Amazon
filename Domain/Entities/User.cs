using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Domain.Models
{
    public class User: IdentityUser
    {
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<CartItem> Cart { get; set; } = new List<CartItem>();
    }
}
