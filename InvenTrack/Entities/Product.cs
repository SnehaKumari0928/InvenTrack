namespace InvenTrack.Entities
{
    public class Product: BaseEntity
    {

        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public int SupplierId { get; set; }

     public Supplier Supplier { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
