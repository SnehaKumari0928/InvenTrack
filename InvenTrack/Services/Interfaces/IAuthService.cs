using InvenTrack.DTOs.Auth;

namespace InvenTrack.Services.Interfaces
{
    public interface IAuthService
    {

        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto);

        Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);

        Task<AuthResponseDto> RefreshTokenAsync(
            RefreshTokenRequestDto dto);

        Task LogoutAsync(string refreshToken);
    }
}
