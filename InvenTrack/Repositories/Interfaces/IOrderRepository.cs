using InvenTrack.DTOs.Order;
using InvenTrack.Entities;

namespace InvenTrack.Repositories.Interfaces
{
    public interface IOrderRepository
    {

        Task<Order?> GetOrderByIdAsync(int id);

        // Create an order for the given user. The repository will
        // perform the necessary stock checks and atomic decrements
        // inside a transaction and will throw if stock is insufficient
        // or if any products are missing.
        Task<Order> CreateOrderAsync(
            DTOs.Order.CreateOrderRequestDto request,
            int userId);
    }
}



