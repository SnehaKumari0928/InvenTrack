namespace InvenTrack.Entities
{
    public class OrderItem: BaseEntity
    {

        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int Quantity { get; set; } = 0;
        public Decimal UnitPrice { get; set; } = 0;
        public Decimal SubTotal { get; set; } = 0;
    }
}
