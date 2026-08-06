using InvenTrack.DTOs.Supplier;
using InvenTrack.Entities;
using InvenTrack.Repositories.Interfaces;
using InvenTrack.Services.Interfaces;

namespace InvenTrack.Services.Implementation
{
    public class SupplierService: ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;

        public SupplierService(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<ICollection<SupplierResponseDto>> GetAllSuppliersAsync()
        {
            var suppliers = await _supplierRepository.GetAllSuppliersAsync();
            return suppliers.Select(s => new SupplierResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                Email = s.Email,
                Phone = s.Phone,
                Address = s.Address
            }).ToList();
        }

        public async Task<SupplierResponseDto> CreateSupplierAsync(CreateSupplierDto dto)
        {
            var supplier = new Supplier
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address
            };
            await _supplierRepository.CreateSupplierAsync(supplier);
            return new SupplierResponseDto
            {
                Id = supplier.Id,
                Name = supplier.Name,
                Email = supplier.Email,
                Phone = supplier.Phone,
                Address = supplier.Address
            };
        }

        public async Task UpdateSupplierAsync(int Id, UpdateSupplierDto dto)
        {
            var supplier = await _supplierRepository.GetSupplierByIdAsync(Id);
            if (supplier == null) throw new InvalidOperationException("Supplier not found");

            supplier.Name = dto.Name;
            supplier.Email = dto.Email;
            supplier.Phone = dto.Phone;

            supplier.Address = dto.Address;

            await _supplierRepository.UpdateSupplierAsync(supplier);
        }

        public async Task DeleteSupplierAsync(int SupplierId)
        {
            var supplier = await _supplierRepository.GetSupplierByIdAsync(SupplierId);
            if (supplier == null) throw new InvalidOperationException("Supplier not found");

            await _supplierRepository.DeleteSupplierAsync(supplier);
        }
    }
}

    
