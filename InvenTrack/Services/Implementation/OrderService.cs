using System.Linq;
using InvenTrack.DTOs.Order;
using InvenTrack.Repositories.Interfaces;
using InvenTrack.Services.Interfaces;

namespace InvenTrack.Services.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderResponseDto> CreateOrderAsync(
            CreateOrderRequestDto request,
            int userId)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var items = request.Items;

            if (items == null || items.Count == 0)
                throw new ArgumentException("Order must contain at least one item.");

            if (items.Any(x => x.Quantity <= 0))
                throw new ArgumentException("Quantity must be greater than zero.");

            var duplicate = items.GroupBy(x => x.ProductId).FirstOrDefault(g => g.Count() > 1);
            if (duplicate != null)
                throw new ArgumentException($"Product {duplicate.Key} appears multiple times.");

            var createdOrder = await _orderRepository.CreateOrderAsync(request, userId);

            return MapToDto(createdOrder);
        }

        public async Task<OrderResponseDto?> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null) return null;
            return MapToDto(order);
        }

        private static OrderResponseDto MapToDto(Entities.Order order)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                TotalAmount = order.TotalAmount,
                UserId = order.UserId,
                CustomerName = order.CustomerName,
                CustomerEmail = order.CustomerEmail,
                Status = order.Status.ToString(),
                Items = order.OrderItems.Select(item => new OrderItemResponseDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product?.Name ?? string.Empty,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.SubTotal
                }).ToList()
            };
        }
    }
}
