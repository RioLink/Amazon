using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
using Domain.Enums;

namespace BLL.Interfaces
{
    public interface IOrderService
    {
        Task CreateOrderAsync(string userId, IEnumerable<CartItem> items);
        Task<IEnumerable<Order>> GetUserOrdersAsync(string userId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();

    //    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    //    {
    //        return new List<Order>
    //{
    //    new Order { Id = 1, TotalAmount = 150.00m, Status = OrderStatus.Pending },
    //    new Order { Id = 2, TotalAmount = 250.50m, Status = OrderStatus.Shipped }
    //};
    //    }
        Task ChangeOrderStatusAsync(int orderId, OrderStatus newStatus);
    }
}
