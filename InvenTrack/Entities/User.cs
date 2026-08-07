using InvenTrack.Enums;

namespace InvenTrack.Entities
{
    public class User: BaseEntity
    {

        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
