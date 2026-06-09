using BLL.Interfaces;
using DAL.Repositories.Interfaces;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmazonMVC.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IGenericRepository<Order> _orderRepository;

        public OrderService(IGenericRepository<Order> orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Order> CreateOrderAsync(string userId, IEnumerable<OrderItem> items, decimal totalAmount)
        {
            var newOrder = new Order
            {
                UserId = userId,
                Date = DateTime.UtcNow,
                TotalAmount = totalAmount,
                Items = (ICollection<OrderItem>)items
            };

            await _orderRepository.AddAsync(newOrder);
            return newOrder;
        }

        public async Task<IEnumerable<Order>> GetUserOrdersAsync(string userId)
        {
            var allOrders = await _orderRepository.GetAllAsync();
            return allOrders.Where(o => o.UserId == userId).ToList();
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllAsync();
        }

        public async Task ChangeOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order != null)
            {
                order.Status = newStatus;
                await _orderRepository.UpdateAsync(order);
            }
        }
    }
}