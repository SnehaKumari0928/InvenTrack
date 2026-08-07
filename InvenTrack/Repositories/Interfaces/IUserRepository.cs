using InvenTrack.Entities;

namespace InvenTrack.Repositories.Interfaces
{
    public interface IUserRepository
    {
       Task<User> GetUserByIdAsync(int userId);
       Task<ICollection<User>> GetAllUsersAsync();
        Task<User> CreateUserAsync(User user);

        Task<User> GetByEmailAsync(string email);

    }
}
