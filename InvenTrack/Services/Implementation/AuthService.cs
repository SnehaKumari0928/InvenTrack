using InvenTrack.DTOs.Auth;
using InvenTrack.DTOs.User;
using InvenTrack.Entities;
using InvenTrack.Enums;
using InvenTrack.Exceptions;
using InvenTrack.Repositories.Implementation;
using InvenTrack.Repositories.Interfaces;
using InvenTrack.Security;
using InvenTrack.Services.Interfaces;

namespace InvenTrack.Services.Implementation
{
    public class AuthService: IAuthService
    {

        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthService(IUserRepository userRepository, IJwtService jwtService, IRefreshTokenRepository refreshTokenRepository   
            )
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<AuthResponseDto> RegisterAsync(
           RegisterRequestDto dto)
        {
            var existingUser =
                await _userRepository.GetByEmailAsync(dto.Email);

            if (existingUser != null)
            {
                throw new BadHttpRequestException(
                    "Email is already registered.");
            }

            var user = new User
            {
                UserName = dto.Username,
                Email = dto.Email,
                PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(dto.Password),

                Role = UserRole.Staff
            };

            await _userRepository.CreateUserAsync(user);

            return await GenerateAuthResponseAsync(user);
        }


        public async Task<AuthResponseDto> LoginAsync(
            LoginRequestDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null ||
                !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                throw new BadHttpRequestException(
                    "Invalid email or password.");
            }
            return await GenerateAuthResponseAsync(user);
        }


        public async Task<AuthResponseDto> RefreshTokenAsync(
    RefreshTokenRequestDto dto)
        {
            var refreshToken =
                await _refreshTokenRepository
                    .GetByTokenAsync(dto.RefreshToken);

            if (refreshToken == null)
            {
                throw new UnauthorizedException(
                    "Invalid refresh token.");
            }

            if (refreshToken.IsRevoked)
            {
                throw new UnauthorizedException(
                    "Refresh token has been revoked.");
            }

            if (refreshToken.ExpiresAt <= DateTime.UtcNow)
            {
                throw new UnauthorizedException(
                    "Refresh token has expired.");
            }

            var user = refreshToken.User;

            refreshToken.IsRevoked = true;
            await _refreshTokenRepository.UpdateAsync(refreshToken);

            return await GenerateAuthResponseAsync(user);
        }

        public async Task LogoutAsync(string refreshToken)
        {
            var token =
                await _refreshTokenRepository
                    .GetByTokenAsync(refreshToken);

            if (token == null)
                return;

            token.IsRevoked = true;
            token.UpdatedAt = DateTime.UtcNow;

            await _refreshTokenRepository.UpdateAsync(token);
        }

        private async Task<AuthResponseDto> GenerateAuthResponseAsync(User user)
        {
            var accessToken = _jwtService.GenerateAccessToken(user);

            var refreshTokenValue = _jwtService.GenerateRefreshToken();

            var accessTokenExpiresAt =
                DateTime.UtcNow.AddMinutes(15);

            var refreshTokenExpiresAt =
                DateTime.UtcNow.AddDays(7);

            var refreshToken = new RefreshToken
            {
                Token = refreshTokenValue,
                UserId = user.Id,
                ExpiresAt = refreshTokenExpiresAt
            };

            await _refreshTokenRepository.AddAsync(refreshToken);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue,

              

                User = new UserResponseDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    Role = user.Role
                }
            };
        }
    }
}
