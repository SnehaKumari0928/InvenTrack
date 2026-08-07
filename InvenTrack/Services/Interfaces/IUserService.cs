using InvenTrack.DTOs.User;

namespace InvenTrack.Services.Interfaces
{
    public interface IUserService
    {

        Task<ICollection<UserResponseDto>> GetAllUsersAsync();
        Task<UserResponseDto> GetUserByIdAsync(int id);

    }
}
