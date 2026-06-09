using BLL.DTOs;
using BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Services
{
    public class OrderService : IOrderService
    {
        public async Task<int> CreateOrderAsync(string userId, CheckoutDto model)
        {
            // Возвращаем ID созданного заказа (пока 0)
            return 0;
        }

        public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId)
        {
            return new List<OrderDto>();
        }
    }
}
