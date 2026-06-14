using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
using Domain.Enums;

namespace BLL.Interfaces
{
    public interface IOrderService
    {
        Task CreateOrderAsync(string userId, IEnumerable<CartItem> items,
            string fullName = "", string phone = "", string city = "",
            string address = "", string? postalCode = null, string paymentMethod = "", decimal shippingCost = 0);
        Task<IEnumerable<Order>> GetUserOrdersAsync(string userId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task ChangeOrderStatusAsync(int orderId, OrderStatus newStatus);
    }
}
