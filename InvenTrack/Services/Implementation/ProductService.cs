using InvenTrack.Entities;
using InvenTrack.Repositories.Interfaces;
using InvenTrack.Services.Interfaces;

namespace InvenTrack.Services.Implementation
{
    public class ProductService: IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }


        public async Task<ICollection<Product>> GetAllProductsAsync(){
            return await _productRepository.GetAllProductsAsync();
        }
        public async Task<Product> CreateProductAsync(Product product)
        {
            return await _productRepository.CreateProductAsync(product);
        }
    }
}
