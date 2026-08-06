using InvenTrack.Entities;

namespace InvenTrack.Repositories.Interfaces
{
    public interface IProductRepository
    {

        Task<ICollection<Product>> GetAllProductsAsync(
            int? supplierId,
            int? lowStock,
            string? search);
        Task<Product> CreateProductAsync(Product product);
        Task UpdateProductAsync(Product product);

        Task DeleteProductAsync(Product product);
        Task<Product> GetProductByIdAsync(int productId);
    }
}
