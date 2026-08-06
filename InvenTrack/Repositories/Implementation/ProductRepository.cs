using InvenTrack.Data;
using InvenTrack.Entities;
using InvenTrack.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InvenTrack.Repositories.Implementation
{
    public class ProductRepository: IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Product>> GetAllProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }
    }
}
