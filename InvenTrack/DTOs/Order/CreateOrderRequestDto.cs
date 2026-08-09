namespace InvenTrack.DTOs.Order
{
    public class CreateOrderRequestDto
    {
        public List<CreateOrderItemDto> Items { get; set; } = new();

        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
    }

    public class CreateOrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
