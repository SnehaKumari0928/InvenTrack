using InvenTrack.Entities;

namespace InvenTrack.Repositories.Interfaces
{
    public interface ISupplierRepository
    {

        Task<ICollection<Supplier>> GetAllSuppliersAsync();
        Task<Supplier> CreateSupplierAsync(Supplier supplier);
        Task UpdateSupplierAsync(Supplier supplier);
        Task DeleteSupplierAsync(Supplier supplier);
        Task<Supplier> GetSupplierByIdAsync(int supplierId);
    }
}
