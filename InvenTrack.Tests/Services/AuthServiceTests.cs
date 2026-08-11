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

            _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(registerRequest.Email))
                .ReturnsAsync((User?)null);
              
            _userRepositoryMock.Setup(repo => repo.CreateUserAsync(It.IsAny<User>()))
                .ReturnsAsync((User user) => user);

            _jwtServiceMock
                .Setup(x => x.GenerateAccessToken(It.IsAny<User>()))
                .Returns("access_token");

            _jwtServiceMock
                .Setup(x => x.GenerateRefreshToken())
                .Returns("refresh_token");

            _refreshTokenRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<RefreshToken>()))
                .Returns(Task.CompletedTask);

            // Act

            var result = await _authService.RegisterAsync(registerRequest);

            // Assert
            var expectedResponse = new AuthResponseDto
            {
                AccessToken = "access_token",
                RefreshToken = "refresh_token",
                User = new UserResponseDto
                {
                    Username = "testuser",
                    Email = "testuser@example.com",
                    Role = UserRole.Staff
                }
            };

            result.Should().BeEquivalentTo(expectedResponse);

            _userRepositoryMock.Verify(
                x => x.GetByEmailAsync(registerRequest.Email), Times.Once);
            _userRepositoryMock.Verify(
                x => x.CreateUserAsync(It.IsAny<User>()), Times.Once);
            _jwtServiceMock.Verify(
                x => x.GenerateAccessToken(It.IsAny<User>()), Times.Once);
            _jwtServiceMock.Verify(
                x => x.GenerateRefreshToken(), Times.Once);
            _refreshTokenRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<RefreshToken>()), Times.Once);
        }



        [Fact]
        public async Task RegisterAsync_ShouldThrowException_WhenEmailAlreadyExists()
        {

        }
    }
}
