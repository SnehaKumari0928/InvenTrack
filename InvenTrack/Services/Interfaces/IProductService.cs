using InvenTrack.Entities;

namespace InvenTrack.Services.Interfaces
{
    public interface IProductService
    {

        Task<ICollection<Product>> GetAllProductsAsync();
        Task<Product> CreateProductAsync(Product product);
    }
}
