using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class User : IdentityUser
    {
        public string? AvatarPath { get; set; }

        public ICollection<Order>    Orders    { get; set; } = new List<Order>();
        public ICollection<CartItem> Cart      { get; set; } = new List<CartItem>();
        public ICollection<Address>  Addresses { get; set; } = new List<Address>();
    }
}
