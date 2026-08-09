using InvenTrack.DTOs.Order;

namespace InvenTrack.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateOrderAsync(
            CreateOrderRequestDto request,
            int userId);

        Task<OrderResponseDto?> GetOrderByIdAsync(int id);
    }
}
