using InvenTrack.DTOs.Supplier;

namespace InvenTrack.Services.Interfaces
{
    public interface ISupplierService
    {

        Task<ICollection<SupplierResponseDto>> GetAllSuppliersAsync();
        Task<SupplierResponseDto> CreateSupplierAsync(CreateSupplierDto dto);
        Task UpdateSupplierAsync(int Id, UpdateSupplierDto dto);
        Task DeleteSupplierAsync(int SupplierId);
    }
}
