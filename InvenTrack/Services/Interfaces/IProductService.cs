using InvenTrack.DTOs.Product;
using InvenTrack.Entities;

namespace InvenTrack.Services.Interfaces
{
    public interface IProductService
    {

        Task<ICollection<ProductResponseDto>> GetAllProductsAsync(
            int? supplierId,
            int? lowStock,
            string? search);
        Task<ProductResponseDto> CreateProductAsync(CreateProductDto dto);
        Task<ProductResponseDto?> GetProductByIdAsync(int id);
        Task UpdateProductAsync(int id, UpdateProductDto dto);
        Task DeleteProductAsync(int ProductId);
    }
}
