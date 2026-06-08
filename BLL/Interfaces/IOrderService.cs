using System;
using System.Collections.Generic;
using System.Text;
using BLL.DTOs;

namespace BLL.Interfaces
{
    public interface IOrderService
    {
        Task<int> CreateOrderAsync(string userId, CheckoutDto model);
        Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId);
    }
}
