using System.ComponentModel.DataAnnotations;

namespace InvenTrack.DTOs.Product
{
    public class CreateProductDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string SKU { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int SupplierId { get; set; }
    }
}
