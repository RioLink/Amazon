using BLL.Interfaces;
using DAL.Repositories.Interfaces;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly ICartService _cardService;
        private readonly IProductService _productService;

        public OrderService(IGenericRepository<Order> orderRepository, ICartService cardService, IProductService productService)
        {
            _orderRepository = orderRepository;
            _cardService = cardService;
            _productService = productService;
        }

        public async Task CreateOrderAsync(string userId, IEnumerable<CartItem> items)
        {
            if (items == null) return;

            var orderItems = items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Product.Price
            }).ToList();

            var totalAmount = orderItems.Sum(oi => oi.Price * oi.Quantity);

            var newOrder = new Order
            {
                UserId = userId,
                Date = DateTime.UtcNow,
                TotalAmount = totalAmount,
                Items = orderItems
            };

            await _orderRepository.AddAsync(newOrder);

            foreach(var item in items) 
            {
                var product = await _productService.GetProductByIdAsync(item.ProductId);
                if (product != null)
                {
                    var newQuantity = product.Quantity - item.Quantity;
                    await _productService.UpdateProductAsync(product.Id, product.Name, product.Price, product.CategoryId, product.Description, product.ImageUrl, newQuantity);
                }   
                await _cardService.RemoveFromCartAsync(userId, item.ProductId);
            }
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