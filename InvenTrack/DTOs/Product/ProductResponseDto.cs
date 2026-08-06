namespace InvenTrack.DTOs.Product
{
    public class ProductResponseDto
    {

        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string SKU { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public string SupplierName { get; set; } = string.Empty;
    }
}
