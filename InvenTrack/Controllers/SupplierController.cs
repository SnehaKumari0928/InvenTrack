using InvenTrack.DTOs.Supplier;
using InvenTrack.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InvenTrack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {

        private readonly ISupplierService _supplierService;
        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllSuppliers()
        {
            var suppliers = await _supplierService.GetAllSuppliersAsync();
            return Ok(suppliers);
        }
        [HttpPost]
        public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierDto dto)
        {
            var supplier = await _supplierService.CreateSupplierAsync(dto);
            return CreatedAtAction(nameof(GetAllSuppliers), new { id = supplier.Id }, supplier);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSupplier(int id, [FromBody] UpdateSupplierDto dto)
        {
            await _supplierService.UpdateSupplierAsync(id, dto);
            return NoContent();
        }
    }
}
