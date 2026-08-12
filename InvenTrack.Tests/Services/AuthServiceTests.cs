using FluentAssertions;
using InvenTrack.DTOs.Auth;
using InvenTrack.DTOs.User;
using InvenTrack.Entities;
using InvenTrack.Enums;
using InvenTrack.Repositories.Interfaces;
using InvenTrack.Security;
using InvenTrack.Services.Implementation;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace InvenTrack.Tests.Services
{
    public class AuthServiceTests
    {

        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
            _jwtServiceMock = new Mock<IJwtService>();
            _authService = new AuthService(
       _userRepositoryMock.Object,
              _jwtServiceMock.Object,
       _refreshTokenRepositoryMock.Object
   );   
        }


        [Fact]
        public async Task RegisterAsync_ShouldReturnAuthResponseDto_WhenValidInput()
        {
            // Arrange
            var registerRequest = new RegisterRequestDto
            {
                Username = "testuser",
                Email = "testuser@example.com",
                Password = "Password123!"
            };


            // No existing user with this email
            _userRepositoryMock
                .Setup(x => x.GetByEmailAsync(registerRequest.Email))
                .ReturnsAsync((User?)null);

            // User is successfully created
            _userRepositoryMock
                .Setup(x => x.CreateUserAsync(It.IsAny<User>()))
                .ReturnsAsync((User user) => user);

            // JWT token setup
            _jwtServiceMock
                .Setup(x => x.GenerateAccessToken(It.IsAny<User>()))
                .Returns("access_token");

            _jwtServiceMock
                .Setup(x => x.GenerateRefreshToken())
                .Returns("refresh_token");

            // Refresh token is successfully saved
            _refreshTokenRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<RefreshToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.RegisterAsync(registerRequest);

            // Assert - Response
            result.Should().NotBeNull();

            result.AccessToken.Should().Be("access_token");
            result.RefreshToken.Should().Be("refresh_token");

            result.User.Should().NotBeNull();
            result.User.Username.Should().Be(registerRequest.Username);
            result.User.Email.Should().Be(registerRequest.Email);
            result.User.Role.Should().Be(UserRole.Staff);

            // Verify - User was checked
            _userRepositoryMock.Verify(
                x => x.GetByEmailAsync(registerRequest.Email),
                Times.Once);

            // Verify - Correct user was created
            _userRepositoryMock.Verify(
                x => x.CreateUserAsync(It.Is<User>(user =>
                    user.UserName == registerRequest.Username &&
                    user.Email == registerRequest.Email &&
                    user.Role == UserRole.Staff &&
                    BCrypt.Net.BCrypt.Verify(
                        registerRequest.Password,
                        user.PasswordHash))),
                Times.Once);

            // Verify - Access token generated
            _jwtServiceMock.Verify(
                x => x.GenerateAccessToken(It.IsAny<User>()),
                Times.Once);

            // Verify - Refresh token generated
            _jwtServiceMock.Verify(
                x => x.GenerateRefreshToken(),
                Times.Once);

            // Verify - Refresh token saved
            _refreshTokenRepositoryMock.Verify(
                x => x.AddAsync(It.Is<RefreshToken>(token =>
                    token.Token == "refresh_token")),
                Times.Once);
        }



        [Fact]
        public async Task RegisterAsync_ShouldThrowException_WhenEmailAlreadyExists()
        {
            // Arrange

            var registerRequest = new RegisterRequestDto
            {
                Username = "testuser",
                Email = "testuser@example.com",
                Password = "Password123!"
            };

            var existingUser = new User
            {
                UserName = "existinguser",
                Email = registerRequest.Email,
                Role
            }
        }
    }
}
