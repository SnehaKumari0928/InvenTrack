using InvenTrack.Data;
using InvenTrack.DTOs.Order;
using InvenTrack.Entities;
using InvenTrack.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InvenTrack.Repositories.Implementation
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrderAsync(
            DTOs.Order.CreateOrderRequestDto request,
            int userId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var items = request.Items;

                var productIds = items.Select(x => x.ProductId).Distinct().ToList();

                var products = await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToListAsync();

                if (products.Count != productIds.Count)
                {
                    var foundIds = products.Select(p => p.Id).ToHashSet();
                    var missingIds = productIds.Where(id => !foundIds.Contains(id));
                    throw new KeyNotFoundException($"Product(s) not found: {string.Join(", ", missingIds)}");
                }

                decimal totalAmount = 0;

                var order = new Order
                {
                    UserId = userId,
                    CustomerName = request.CustomerName,
                    CustomerEmail = request.CustomerEmail,
                    Status = Enums.OrderStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Orders.Add(order);

                foreach (var item in items)
                {
                    var product = products.First(p => p.Id == item.ProductId);

                    var affectedRows = await _context.Products
                        .Where(p => p.Id == item.ProductId && p.StockQuantity >= item.Quantity)
                        .ExecuteUpdateAsync(setters => setters.SetProperty(p => p.StockQuantity, p => p.StockQuantity - item.Quantity));

                    if (affectedRows == 0)
                    {
                        throw new InvalidOperationException($"Insufficient stock for product '{product.Name}'. Requested: {item.Quantity}, Available: {product.StockQuantity}.");
                    }

                    var itemTotal = product.Price * item.Quantity;
                    totalAmount += itemTotal;

                    var orderItem = new OrderItem
                    {
                        Order = order,
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price,
                        SubTotal = itemTotal
                    };

                    _context.OrderItems.Add(orderItem);
                }

                order.TotalAmount = totalAmount;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return order;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}
