using FluentAssertions;
using InvenTrack.DTOs.User;
using InvenTrack.Entities;
using InvenTrack.Enums;
using InvenTrack.Exceptions;
using InvenTrack.Repositories.Interfaces;
using InvenTrack.Services.Implementation;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace InvenTrack.Tests.Services
{
    public class UserServiceTests
    {

        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly UserService _userService;


        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userService = new UserService(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnsAllUsers_WhenUsersExist()
        {
            var users = new List<User>
            {
                new User { Id = 1, UserName = "user1", Email = "user1@example.com", Role = UserRole.Staff },
                new User { Id = 2, UserName = "user2", Email = "user2@example.com", Role = UserRole.Staff }
            };

            _userRepositoryMock
                .Setup(x => x.GetAllUsersAsync())
                .ReturnsAsync(users);

            var result = await _userService.GetAllUsersAsync();

            result.Should().NotBeNull();
            result.Should().HaveCount(2);

            result.Should().ContainEquivalentOf(new UserResponseDto
            {
                Id = 1,
                Username = "user1",
                Email = "user1@example.com",
                Role = UserRole.Staff
            });

            result.Should().ContainEquivalentOf(new UserResponseDto
            {
                Id = 2,
                Username = "user2",
                Email = "user2@example.com",
                Role = UserRole.Staff
            });

            _userRepositoryMock.Verify(x => x.GetAllUsersAsync(), Times.Once);
        }

        [Fact]

        public async Task GetAllUsersAsync_ShouldReturnEmptyCollection_WhenNoUsersExist()
        {
            _userRepositoryMock
                .Setup(x => x.GetAllUsersAsync())
                .ReturnsAsync(new List<Entities.User>());


            var result = await _userService.GetAllUsersAsync();


            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _userRepositoryMock.Verify(x => x.GetAllUsersAsync(), Times.Once);
        }


        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            var user = new User
            {
                Id = 1,
                UserName = "user1",
                Email = "user1@example.com",
                Role = UserRole.Staff,


            };

            _userRepositoryMock
                .Setup(x => x.GetUserByIdAsync(1))
                .ReturnsAsync(user);

            var result = await _userService.GetUserByIdAsync(1);

            result.Should().NotBeNull();

            result.Id.Should().Be(1);
            result.Username.Should().Be(user.UserName);
            result.Email.Should().Be(user.Email);
            result.Role.Should().Be(UserRole.Staff);

            _userRepositoryMock.Verify(
                x => x.GetUserByIdAsync(1),
                Times.Once);

        }


        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            _userRepositoryMock
                .Setup(x => x.GetUserByIdAsync(1))
                .ReturnsAsync((Entities.User)null);
            var result = await _userService.GetUserByIdAsync(1);
            result.Should().BeNull();


         

            _userRepositoryMock.Verify(
                x => x.GetUserByIdAsync(1),
                Times.Once);
        }

        [Fact]
        public async Task CreateStaffAsync_ShouldCreateStaffUser_WhenValidInput()
        {
            var username = "teststaff";
            var email = "teststaff@example.com";
            var password = "Password123!";

            _userRepositoryMock
                .Setup(x => x.GetByEmailAsync(email))
                .ReturnsAsync((User?)null);

            _userRepositoryMock
                .Setup(x => x.CreateUserAsync(It.IsAny<User>()))
                .ReturnsAsync((User user) =>
                {
                    user.Id = 1;
                    return user;
                });

            var result = await _userService.CreateStaffAsync(
                username,
                email,
                password);

            result.Should().NotBeNull();

            result.Id.Should().Be(1);
            result.Username.Should().Be(username);
            result.Email.Should().Be(email);
            result.Role.Should().Be(UserRole.Staff);

            _userRepositoryMock.Verify(
                x => x.GetByEmailAsync(email),
                Times.Once);

            _userRepositoryMock.Verify(
                x => x.CreateUserAsync(It.Is<User>(user =>
                    user.UserName == username &&
                    user.Email == email &&
                    user.Role == UserRole.Staff &&
                    BCrypt.Net.BCrypt.Verify(
                        password,
                        user.PasswordHash))),
                Times.Once);
        }

        [Fact]
        public async Task CreateStaffAsync_ShouldThrowException_WhenEmailAlreadyExists()
        {
            var username = "teststaff";
            var email = "existing@example.com";
            var password = "Password123!";

            var existingUser = new User
            {
                Id = 1,
                UserName = "existinguser",
                Email = email,
                Role = UserRole.Staff
            };

            _userRepositoryMock
                .Setup(x => x.GetByEmailAsync(email))
                .ReturnsAsync(existingUser);

            Func<Task> act = async () =>
                await _userService.CreateStaffAsync(
                    username,
                    email,
                    password);

            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Email is already registered.");

            _userRepositoryMock.Verify(
                x => x.GetByEmailAsync(email),
                Times.Once);

            _userRepositoryMock.Verify(
                x => x.CreateUserAsync(It.IsAny<User>()),
                Times.Never);
        }

    }
}
