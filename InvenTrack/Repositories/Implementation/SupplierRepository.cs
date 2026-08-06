using InvenTrack.Data;
using InvenTrack.Entities;
using InvenTrack.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InvenTrack.Repositories.Implementation
{
    public class SupplierRepository: ISupplierRepository
    {

        private readonly AppDbContext _context;

        public SupplierRepository(AppDbContext context)
        {
            _context = context;
        }



        public async Task<ICollection<Supplier>> GetAllSuppliersAsync()
        {
            return await _context.Suppliers.ToListAsync();
        }
        public async Task<Supplier> CreateSupplierAsync(Supplier supplier)
        {
            await _context.Suppliers.AddAsync(supplier);
            await _context.SaveChangesAsync();
            return supplier;
        }
        public async Task UpdateSupplierAsync(Supplier supplier)
        {
            _context.Suppliers.Update(supplier);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteSupplierAsync(Supplier supplier)
        {
            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();

        }
        public async Task<Supplier> GetSupplierByIdAsync(int Id)
        {
            return await _context.Suppliers.FindAsync(Id);
        }
    }
}
