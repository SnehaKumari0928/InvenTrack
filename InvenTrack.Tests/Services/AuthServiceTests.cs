using FluentAssertions;
using InvenTrack.DTOs.Auth;
using InvenTrack.DTOs.User;
using InvenTrack.Entities;
using InvenTrack.Enums;
using InvenTrack.Exceptions;
using InvenTrack.Repositories.Interfaces;
using InvenTrack.Security;
using InvenTrack.Services.Implementation;
using Microsoft.AspNetCore.Http;
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

        //  ----LoginAsync Tests----
        [Fact]
        public async Task LoginAsync_ShouldReturnAuthResponseDto_WhenCredentialsAreValid()
        {
            // Arrange
            var request = new LoginRequestDto
            {
                Email = "testuser@example.com",
                Password = "Password123!"
            };

            var user = new User
            {
                Id = 1,
                UserName = "testuser",
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = UserRole.Staff
            };

            _userRepositoryMock
                .Setup(x => x.GetByEmailAsync(request.Email))
                .ReturnsAsync(user);

            _jwtServiceMock
                .Setup(x => x.GenerateAccessToken(user))
                .Returns("access_token");

            _jwtServiceMock
                .Setup(x => x.GenerateRefreshToken())
                .Returns("refresh_token");

            _refreshTokenRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<RefreshToken>()))
                .Returns(Task.CompletedTask);

            var result = await _authService.LoginAsync(request);

            result.Should().NotBeNull();

            result.AccessToken.Should().Be("access_token");
            result.RefreshToken.Should().Be("refresh_token");

            result.User.Should().NotBeNull();
            result.User.Id.Should().Be(user.Id);
            result.User.Username.Should().Be(user.UserName);
            result.User.Email.Should().Be(user.Email);
            result.User.Role.Should().Be(user.Role);

            _userRepositoryMock.Verify(
                x => x.GetByEmailAsync(request.Email),
                Times.Once);

            _jwtServiceMock.Verify(
                x => x.GenerateAccessToken(user),
                Times.Once);

            _jwtServiceMock.Verify(
                x => x.GenerateRefreshToken(),
                Times.Once);

            _refreshTokenRepositoryMock.Verify(
                x => x.AddAsync(It.Is<RefreshToken>(
                    token => token.Token == "refresh_token" &&
                              token.UserId == user.Id)),
                Times.Once);
        }


        [Fact]
        public async Task LoginAsync_ShouldThrowException_WhenUserDoesNotExist()
        {
            var request = new LoginRequestDto
            {
                Email = "nonexistent@example.com",
                Password = "Password123!"
            };

            _userRepositoryMock
                .Setup(x => x.GetByEmailAsync(request.Email))
                .ReturnsAsync((User)null);


            Func<Task> act = async () => await _authService.LoginAsync(request);

            await act.Should()
                .ThrowAsync<BadHttpRequestException>()
                .WithMessage("Invalid email or password.");


            _userRepositoryMock.Verify(
                x => x.GetByEmailAsync(request.Email),
                Times.Once);

            _jwtServiceMock.Verify(
                x => x.GenerateAccessToken(It.IsAny<User>()),
                Times.Never);


            _jwtServiceMock.Verify(
                x => x.GenerateRefreshToken(),
                Times.Never);

            _refreshTokenRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<RefreshToken>()),
                Times.Never);

        }

        [Fact]
        public async Task LoginAsync_ShouldThrowException_WhenPasswordIsIncorrect()
        {
            // Arrange
            var request = new LoginRequestDto
            {
                Email = "testuser@example.com",
                Password = "WrongPassword123!"

            };

            var user = new User
            {
                Id = 1,
                UserName = "testuser",
                Email = request.Email,
                PasswordHash =
                   BCrypt.Net.BCrypt.HashPassword("CorrectPassword123!"),
                Role = UserRole.Staff
            };

            _userRepositoryMock
                .Setup(x => x.GetByEmailAsync(request.Email))
                .ReturnsAsync(user);

            // Act & Assert

            Func<Task> act = async () => await _authService.LoginAsync(request);

            await act.Should()
                .ThrowAsync<BadHttpRequestException>()
                .WithMessage("Invalid email or password.");


            _userRepositoryMock.Verify(

                x => x.GetByEmailAsync(request.Email),
                Times.Once);

            _jwtServiceMock.Verify(
                x => x.GenerateAccessToken(It.IsAny<User>()),
                Times.Never);

            _refreshTokenRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<RefreshToken>()),
                Times.Never);
        }


        // ----RefreshTokenAsync Tests----

        [Fact]
        public async Task RefreshTokenAsync_ShouldThrowException_WhenRefreshTokenDoesNotExist()
        {
            // Arrange
            var request = new RefreshTokenRequestDto
            {
                RefreshToken = "nonexistent_refresh_token"
            };

            _refreshTokenRepositoryMock
                .Setup(x => x.GetByTokenAsync(request.RefreshToken))
                .ReturnsAsync((RefreshToken?)null);

            // Act & Assert

            Func<Task> act = async () => await _authService.RefreshTokenAsync(request);

            await act.Should()
                .ThrowAsync<UnauthorizedException>()
                .WithMessage("Invalid refresh token.");


            _refreshTokenRepositoryMock.Verify(
                x => x.GetByTokenAsync(request.RefreshToken),
                Times.Once);

            _refreshTokenRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<RefreshToken>()), Times.Never);

            _jwtServiceMock.Verify(
                x => x.GenerateAccessToken(It.IsAny<User>()),
                Times.Never);

            _jwtServiceMock.Verify(
                x => x.GenerateRefreshToken(),
                Times.Never);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldThrowException_WhenRefreshTokenIsRevoked()
        {
            // Arrange
            var request = new RefreshTokenRequestDto
            {
                RefreshToken = "revoked_refresh_token"
            };
            var revokedToken = new RefreshToken
            {
                Token = request.RefreshToken,
                IsRevoked = true,
                ExpiresAt = DateTime.UtcNow.AddDays(1),
            };

            _refreshTokenRepositoryMock
                .Setup(x => x.GetByTokenAsync(request.RefreshToken))
                .ReturnsAsync(revokedToken);

            // Act & Assert

            Func<Task> act = async () => await _authService.RefreshTokenAsync(request);


            await act.Should()
                .ThrowAsync<UnauthorizedException>()
                .WithMessage("Refresh token has been revoked.");


            _refreshTokenRepositoryMock.Verify(
                x => x.GetByTokenAsync(request.RefreshToken),
                Times.Once);

            _refreshTokenRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<RefreshToken>()),
                Times.Never);

            _jwtServiceMock.Verify(
                x => x.GenerateAccessToken(It.IsAny<User>()),
                Times.Never);

            _jwtServiceMock.Verify(
                x => x.GenerateRefreshToken(),
                Times.Never);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldThrowException_WhenRefreshTokenIsExpired()
        {
            // Arrange
            var request = new RefreshTokenRequestDto
            {
                RefreshToken = "expired_token"
            };

            var expiredToken = new RefreshToken
            {
                Token = request.RefreshToken,
                IsRevoked = false,
                ExpiresAt = DateTime.UtcNow.AddDays(-10),
            };

            _refreshTokenRepositoryMock
                .Setup(x => x.GetByTokenAsync(request.RefreshToken))
                .ReturnsAsync(expiredToken);

            // Act & Assert

            Func<Task> act = async () => await _authService.RefreshTokenAsync(request);

            await act.Should()
                .ThrowAsync<UnauthorizedException>()
                .WithMessage("Refresh token has expired.");


            _refreshTokenRepositoryMock.Verify(
                x => x.GetByTokenAsync(request.RefreshToken),
                Times.Once);

            _refreshTokenRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<RefreshToken>()),
                Times.Never);

            _jwtServiceMock.Verify(
                x => x.GenerateAccessToken(It.IsAny<User>()),
                Times.Never);

            _jwtServiceMock.Verify(
                x => x.GenerateRefreshToken(),
                Times.Never);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldReturnAuthResponseDto_WhenRefreshTokenIsValid()
        {
            // Arrange
            var request = new RefreshTokenRequestDto
            {
                RefreshToken = "valid_refresh_token"
            };
            var user = new User
            {
                Id = 1,
                UserName = "testuser",
                Email = "testuser@example.com",
                Role = UserRole.Staff
            };

            var validToken = new RefreshToken
            {
                Token = request.RefreshToken,
                UserId = user.Id,
                User = user,
                IsRevoked = false,
                ExpiresAt = DateTime.UtcNow.AddDays(1),
            };

            _refreshTokenRepositoryMock
                .Setup(x => x.GetByTokenAsync(request.RefreshToken))
                .ReturnsAsync(validToken);

            _refreshTokenRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<RefreshToken>()))
                .Returns(Task.CompletedTask);

            _jwtServiceMock
                .Setup(x => x.GenerateAccessToken(user))
                .Returns("new_access_token");

            _jwtServiceMock
                .Setup(x => x.GenerateRefreshToken())
                .Returns("new_refresh_token");

            // Act
            var result = await _authService.RefreshTokenAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.AccessToken.Should().Be("new_access_token");
            result.RefreshToken.Should().Be("new_refresh_token");
            result.User.Id.Should().Be(user.Id);
            result.User.Username.Should().Be(user.UserName);
            result.User.Email.Should().Be(user.Email);
            result.User.Role.Should().Be(user.Role);

            _refreshTokenRepositoryMock.Verify(
                x => x.GetByTokenAsync(request.RefreshToken),
                Times.Once);

            _refreshTokenRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<RefreshToken>()),
                Times.Once);

            _jwtServiceMock.Verify(
                x => x.GenerateAccessToken(It.IsAny<User>()),
                Times.Once);

            _jwtServiceMock.Verify(
                x => x.GenerateRefreshToken(),
                Times.Once);
        }

        // ---LogoutAsync Tests---
        [Fact]
        public async Task LogoutAsync_ShouldRetornWithoutUpdating_WhenRefreshTokenDoesNotExist()
        {
            var refreshToken = "nonexistent_refresh_token";

            _refreshTokenRepositoryMock
                .Setup(x => x.GetByTokenAsync(refreshToken))
                .ReturnsAsync((RefreshToken?)null);

            await _authService.LogoutAsync(refreshToken);

            _refreshTokenRepositoryMock.Verify(
                x => x.GetByTokenAsync(refreshToken),
                Times.Once);

            _refreshTokenRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<RefreshToken>()),
                Times.Never);

           
        }

        [Fact]

        public async Task LogoutAsync_ShouldUpdateRefreshToken_WhenRefreshTokenExists()
        {
            var refreshToken = "valid_refresh_token";
           
            var refreshTokens = new RefreshToken
            {
                Token = refreshToken,
              
                IsRevoked = false,
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            };

            _refreshTokenRepositoryMock
                .Setup(x => x.GetByTokenAsync(refreshToken))
                .ReturnsAsync(refreshTokens);

            _refreshTokenRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<RefreshToken>()))
                .Returns(Task.CompletedTask);

            await _authService.LogoutAsync(refreshToken);


            refreshTokens.IsRevoked.Should().BeTrue();

            refreshTokens.UpdatedAt.Should().BeCloseTo(
                DateTime.UtcNow,
                TimeSpan.FromSeconds(2));

            _refreshTokenRepositoryMock.Verify(
                x => x.GetByTokenAsync(refreshToken),
                Times.Once);

            _refreshTokenRepositoryMock.Verify(
                x => x.UpdateAsync(It.Is<RefreshToken>(
                    token => token.Token == refreshToken &&
                              token.IsRevoked)),
                Times.Once);
        }
    }
}

