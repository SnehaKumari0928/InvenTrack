using InvenTrack.DTOs.User;
using InvenTrack.Enums;
using InvenTrack.Repositories.Interfaces;
using InvenTrack.Services.Interfaces;

namespace InvenTrack.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<ICollection<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return users.Select(u => new UserResponseDto
            {
                Id = u.Id,
                Username = u.UserName,
                Email = u.Email,
                Role = u.Role,
            }).ToList();
        }
        public async Task<UserResponseDto> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
                return null;

            return new UserResponseDto
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Role = user.Role,
            };
        }

        public async Task<UserResponseDto> CreateStaffAsync(string username, string email, string password)
        {
            var existing = await _userRepository.GetByEmailAsync(email);
            if (existing != null)
                throw new Exceptions.BadRequestException("Email is already registered.");

            var user = new Entities.User
            {
                UserName = username,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = Enums.UserRole.Staff
            };

            var created = await _userRepository.CreateUserAsync(user);

            return new UserResponseDto
            {
                Id = created.Id,
                Username = created.UserName,
                Email = created.Email,
                Role = created.Role
            };
        }
    }
}
