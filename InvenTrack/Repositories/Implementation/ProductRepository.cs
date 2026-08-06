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

       public async  Task<ICollection<Product>> GetAllProductsAsync(
     int? supplierId,
     int? lowStock,
     string? search)
        {

            var query = _context.Products.Include(p => p.Supplier).AsQueryable();
            if (supplierId.HasValue)
            {
                query = query.Where(p => p.SupplierId == supplierId.Value);
            }
            if (lowStock.HasValue)
            {
                query = query.Where(p => p.StockQuantity < lowStock.Value);
            }
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search) || p.SKU.Contains(search));
            }
            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
           await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return await _context.Products
                .Include(p => p.Supplier)
                .FirstAsync(p => p.Id == product.Id);
        }


        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
        public async Task<Product> GetProductByIdAsync(int productId)
        {
            return await _context.Products
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.Id == productId);
        }
    }
}
