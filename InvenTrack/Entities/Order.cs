using InvenTrack.Enums;

namespace InvenTrack.Entities
{
    public class Order: BaseEntity
    {

        public int UserId { get; set; }
        public User User { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();


    }

    }

