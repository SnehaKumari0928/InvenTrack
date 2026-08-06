using InvenTrack.Entities;

namespace InvenTrack.Repositories.Interfaces
{
    public interface IProductRepository
    {

        Task<ICollection<Product>> GetAllProductsAsync();

        Task<Product> CreateProductAsync(Product product);
    }
}
