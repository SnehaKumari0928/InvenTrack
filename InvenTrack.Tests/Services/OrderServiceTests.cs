using InvenTrack.DTOs.Order;
using InvenTrack.Entities;
using InvenTrack.Repositories.Interfaces;
using InvenTrack.Services.Implementation;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;


namespace InvenTrack.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _orderService = new OrderService(_orderRepositoryMock.Object);
        }

        // CreateOrderAsync - Validation Tests

        [Fact]
        public async Task CreateOrderAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            // Arrange
            CreateOrderRequestDto request = null!;

            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(
                () => _orderService.CreateOrderAsync(request, 1));

            // Assert
            Assert.Equivalent(
                nameof(request),
                exception.ParamName);
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldThrowArgumentException_WhenItemsAreNull()
        {
            // Arrange
            var request = new CreateOrderRequestDto
            {
                Items = null!
            };


            // Act
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _orderService.CreateOrderAsync(request, 1));

            // Assert
            Assert.Equivalent(
                "Order must contain at least one item.",
                exception.Message);
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldThrowArgumentException_WhenItemsAreEmpty()
        {
            // Arrange
            var request = new CreateOrderRequestDto
            {
                Items = new List<CreateOrderItemDto>()
            };

            // Act
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _orderService.CreateOrderAsync(request, 1));

            // Assert
            Assert.Equivalent(
                "Order must contain at least one item.",
                exception.Message);
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldThrowArgumentException_WhenQuantityIsZero()
        {
            // Arrange
            var request = new CreateOrderRequestDto
            {
                Items = new List<CreateOrderItemDto>
                {
                    new CreateOrderItemDto
                    {
                        ProductId = 1,
                        Quantity = 0
                    }
                }
            };

            // Act
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _orderService.CreateOrderAsync(request, 1));

            // Assert
            Assert.Equivalent(
                "Quantity must be greater than zero.",
                exception.Message);
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldThrowArgumentException_WhenQuantityIsNegative()
        {
            // Arrange
            var request = new CreateOrderRequestDto
            {
                Items = new List<CreateOrderItemDto>
                {
                    new CreateOrderItemDto
                    {
                        ProductId = 1,
                        Quantity = -5
                    }
                }
            };

            // Act
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _orderService.CreateOrderAsync(request, 1));

            // Assert
            Assert.Equivalent(
                "Quantity must be greater than zero.",
                exception.Message);
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldThrowArgumentException_WhenDuplicateProductExists()
        {
            // Arrange
            var request = new CreateOrderRequestDto
            {
                Items = new List<CreateOrderItemDto>
                {
                    new CreateOrderItemDto
                    {
                        ProductId = 1,
                        Quantity = 2
                    },
                    new CreateOrderItemDto
                    {
                        ProductId = 1,
                        Quantity = 3
                    }
                }
            };

            // Act
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _orderService.CreateOrderAsync(request, 1));

            // Assert
            Assert.Equivalent(
                "Product 1 appears multiple times.",
                exception.Message);

            _orderRepositoryMock.Verify(
                x => x.CreateOrderAsync(
                    It.IsAny<CreateOrderRequestDto>(),
                    It.IsAny<int>()),
                Times.Never);
        }

        // CreateOrderAsync - Success

        [Fact]
        public async Task CreateOrderAsync_ShouldReturnOrderResponseDto_WhenRequestIsValid()
        {
            // Arrange
            var request = new CreateOrderRequestDto
            {
                Items = new List<CreateOrderItemDto>
                {
                    new CreateOrderItemDto
                    {
                        ProductId = 1,
                        Quantity = 2
                    },
                    new CreateOrderItemDto
                    {
                        ProductId = 2,
                        Quantity = 1
                    }
                }
            };

            var createdOrder = new Order
            {
                Id = 100,
                UserId = 10,
                CreatedAt = DateTime.UtcNow,
                TotalAmount = 70000,
                CustomerName = "Sneha",
                CustomerEmail = "sneha@test.com",
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = 1,
                        Quantity = 2,
                        UnitPrice = 30000,
                        SubTotal = 60000,
                        Product = new Product
                        {
                            Id = 1,
                            Name = "Laptop"
                        }
                    },
                    new OrderItem
                    {
                        ProductId = 2,
                        Quantity = 1,
                        UnitPrice = 10000,
                        SubTotal = 10000,
                        Product = new Product
                        {
                            Id = 2,
                            Name = "Mouse"
                        }
                    }
                }
            };

            _orderRepositoryMock
                .Setup(x => x.CreateOrderAsync(request, 10))
                .ReturnsAsync(createdOrder);

            var expected = new OrderResponseDto
            {
                Id = 100,
                UserId = 10,
                CreatedAt = createdOrder.CreatedAt,
                TotalAmount = 70000,
                CustomerName = "Sneha",
                CustomerEmail = "sneha@test.com",
                Status = createdOrder.Status.ToString(),

                Items = new List<OrderItemResponseDto>
                {
                    new OrderItemResponseDto
                    {
                        ProductId = 1,
                        ProductName = "Laptop",
                        Quantity = 2,
                        UnitPrice = 30000,
                        TotalPrice = 60000
                    },
                    new OrderItemResponseDto
                    {
                        ProductId = 2,
                        ProductName = "Mouse",
                        Quantity = 1,
                        UnitPrice = 10000,
                        TotalPrice = 10000
                    }
                }
            };

            // Act
            var result = await _orderService.CreateOrderAsync(request, 10);

            // Assert
            Assert.Equivalent(expected, result);

            _orderRepositoryMock.Verify(
                x => x.CreateOrderAsync(request, 10),
                Times.Once);
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldMapEmptyProductName_WhenProductIsNull()
        {
            // Arrange
            var request = new CreateOrderRequestDto
            {
                Items = new List<CreateOrderItemDto>
                {
                    new CreateOrderItemDto
                    {
                        ProductId = 1,
                        Quantity = 2
                    }
                }
            };

            var createdOrder = new Order
            {
                Id = 100,
                UserId = 10,
                CreatedAt = DateTime.UtcNow,
                TotalAmount = 2000,
                CustomerName = "Sneha",
                CustomerEmail = "sneha@test.com",

                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = 1,
                        Quantity = 2,
                        UnitPrice = 1000,
                        SubTotal = 2000,
                        Product = null
                    }
                }
            };

            _orderRepositoryMock
                .Setup(x => x.CreateOrderAsync(request, 10))
                .ReturnsAsync(createdOrder);

            var expected = new OrderResponseDto
            {
                Id = 100,
                UserId = 10,
                CreatedAt = createdOrder.CreatedAt,
                TotalAmount = 2000,
                CustomerName = "Sneha",
                CustomerEmail = "sneha@test.com",
                Status = createdOrder.Status.ToString(),

                Items = new List<OrderItemResponseDto>
                {
                    new OrderItemResponseDto
                    {
                        ProductId = 1,
                        ProductName = string.Empty,
                        Quantity = 2,
                        UnitPrice = 1000,
                        TotalPrice = 2000
                    }
                }
            };

            // Act
            var result = await _orderService.CreateOrderAsync(request, 10);

            // Assert
            Assert.Equivalent(expected, result);
        }

        // GetOrderByIdAsync

        [Fact]
        public async Task GetOrderByIdAsync_ShouldReturnOrderResponseDto_WhenOrderExists()
        {
            // Arrange
            var orderId = 100;

            var order = new Order
            {
                Id = 100,
                UserId = 10,
                CreatedAt = DateTime.UtcNow,
                TotalAmount = 50000,
                CustomerName = "Sneha",
                CustomerEmail = "sneha@test.com",

                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = 1,
                        Quantity = 2,
                        UnitPrice = 25000,
                        SubTotal = 50000,
                        Product = new Product
                        {
                            Id = 1,
                            Name = "Laptop"
                        }
                    }
                }
            };

            _orderRepositoryMock
                .Setup(x => x.GetOrderByIdAsync(orderId))
                .ReturnsAsync(order);

            var expected = new OrderResponseDto
            {
                Id = 100,
                UserId = 10,
                CreatedAt = order.CreatedAt,
                TotalAmount = 50000,
                CustomerName = "Sneha",
                CustomerEmail = "sneha@test.com",
                Status = order.Status.ToString(),

                Items = new List<OrderItemResponseDto>
                {
                    new OrderItemResponseDto
                    {
                        ProductId = 1,
                        ProductName = "Laptop",
                        Quantity = 2,
                        UnitPrice = 25000,
                        TotalPrice = 50000
                    }
                }
            };

            // Act
            var result = await _orderService.GetOrderByIdAsync(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equivalent(expected, result);

            _orderRepositoryMock.Verify(
                x => x.GetOrderByIdAsync(orderId),
                Times.Once);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ShouldReturnNull_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderId = 999;

            _orderRepositoryMock
                .Setup(x => x.GetOrderByIdAsync(orderId))
                .ReturnsAsync((Order?)null);

            // Act
            var result = await _orderService.GetOrderByIdAsync(orderId);

            // Assert
            Assert.Null(result);

            _orderRepositoryMock.Verify(
                x => x.GetOrderByIdAsync(orderId),
                Times.Once);
        }
    }
}
