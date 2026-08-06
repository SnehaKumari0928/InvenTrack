namespace InvenTrack.DTOs.Supplier
{
    public class UpdateSupplierDto
    {
        public string Name { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }
    }
}
