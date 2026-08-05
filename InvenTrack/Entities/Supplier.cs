namespace InvenTrack.Entities
{
    public class Supplier: BaseEntity
    {

        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
