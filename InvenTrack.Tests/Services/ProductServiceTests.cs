using FluentAssertions;
using InvenTrack.DTOs.Product;
using InvenTrack.Entities;
using InvenTrack.Repositories.Interfaces;
using InvenTrack.Services.Implementation;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace InvenTrack.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<ISupplierRepository> _supplierRepositoryMock;
        private readonly ProductService _productService;


        public ProductServiceTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _supplierRepositoryMock = new Mock<ISupplierRepository>();
            _productService = new ProductService(_productRepositoryMock.Object, _supplierRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnProducts_WhenProductsExist()
        {
            // Arrange
            var products = new List<Product>
    {
        new Product
        {
            Id = 1,
            Name = "Laptop",
            SKU = "LAP001",
            Price = 50000,
            StockQuantity = 10,
            Supplier = new Supplier
            {
                Id = 1,
                Name = "ABC Suppliers"
            }
        },
        new Product
        {
            Id = 2,
            Name = "Mouse",
            SKU = "MOU001",
            Price = 1000,
            StockQuantity = 20,
            Supplier = new Supplier
            {
                Id = 2,
                Name = "XYZ Suppliers"
            }
        }
    };

            _productRepositoryMock
                .Setup(x => x.GetAllProductsAsync(
                    null,
                    null,
                    null))
                .ReturnsAsync(products);

            // Act
            var result = await _productService.GetAllProductsAsync(
                null,
                null,
                null);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);

            result.Should().ContainEquivalentOf(new ProductResponseDto
            {
                Id = 1,
                Name = "Laptop",
                SKU = "LAP001",
                Price = 50000,
                StockQuantity = 10,
                SupplierName = "ABC Suppliers"
            });

            result.Should().ContainEquivalentOf(new ProductResponseDto
            {
                Id = 2,
                Name = "Mouse",
                SKU = "MOU001",
                Price = 1000,
                StockQuantity = 20,
                SupplierName = "XYZ Suppliers"
            });

            _productRepositoryMock.Verify(
                x => x.GetAllProductsAsync(null, null, null),
                Times.Once);
        }


        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnEmptyCollection_WhenNoProductsExist()
        {
            // Arrange
            _productRepositoryMock
                .Setup(x => x.GetAllProductsAsync(
                    null,
                    null,
                    null))
                .ReturnsAsync(new List<Product>());

            // Act
            var result = await _productService.GetAllProductsAsync(
                null,
                null,
                null);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();

            _productRepositoryMock.Verify(
                x => x.GetAllProductsAsync(null, null, null),
                Times.Once);
        }


        [Fact]
        public async Task CreateProductAsync_ShouldCreateProduct_WhenSupplierExists()
        {
            // Arrange
            var dto = new CreateProductDto
            {
                Name = "Laptop",
                SKU = "LAP001",
                Price = 50000,
                StockQuantity = 10,
                SupplierId = 1
            };

            var supplier = new Supplier
            {
                Id = 1,
                Name = "ABC Suppliers"
            };

            _supplierRepositoryMock
                .Setup(x => x.GetSupplierByIdAsync(dto.SupplierId))
                .ReturnsAsync(supplier);

            _productRepositoryMock
                .Setup(x => x.CreateProductAsync(It.IsAny<Product>()))
                .ReturnsAsync((Product product) =>
                {
                    product.Id = 100;
                    product.Supplier = supplier;
                    return product;
                });

            // Act
            var result = await _productService.CreateProductAsync(dto);

            // Assert
            result.Should().NotBeNull();

            result.Id.Should().Be(100);
            result.Name.Should().Be(dto.Name);
            result.SKU.Should().Be(dto.SKU);
            result.Price.Should().Be(dto.Price);
            result.StockQuantity.Should().Be(dto.StockQuantity);
            result.SupplierName.Should().Be("ABC Suppliers");

            // Verify supplier lookup
            _supplierRepositoryMock.Verify(
                x => x.GetSupplierByIdAsync(dto.SupplierId),
                Times.Once);

            // Verify product creation
            _productRepositoryMock.Verify(
                x => x.CreateProductAsync(It.Is<Product>(product =>
                    product.Name == dto.Name &&
                    product.SKU == dto.SKU &&
                    product.Price == dto.Price &&
                    product.StockQuantity == dto.StockQuantity &&
                    product.SupplierId == dto.SupplierId)),
                Times.Once);
        }


        [Fact]
        public async Task CreateProductAsync_ShouldThrowException_WhenSupplierDoesNotExist()
        {
            // Arrange
            var dto = new CreateProductDto
            {
                Name = "Laptop",
                SKU = "LAP001",
                Price = 50000,
                StockQuantity = 10,
                SupplierId = 999
            };

            _supplierRepositoryMock
                .Setup(x => x.GetSupplierByIdAsync(dto.SupplierId))
                .ReturnsAsync((Supplier?)null);

            // Act
            Func<Task> act = async () =>
                await _productService.CreateProductAsync(dto);

            // Assert
            await act.Should()
                .ThrowAsync<InvenTrack.Exceptions.NotFoundException>()
                .WithMessage("Supplier not found");

            // Product must not be created
            _productRepositoryMock.Verify(
                x => x.CreateProductAsync(It.IsAny<Product>()),
                Times.Never);
        }


        [Fact]
        public async Task UpdateProductAsync_ShouldThrowException_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = 999;

            var dto = new UpdateProductDto
            {
                Name = "Updated Laptop",
                SKU = "LAP002",
                Price = 60000,
                StockQuantity = 15,
                SupplierId = 1
            };

            _productRepositoryMock
                .Setup(x => x.GetProductByIdAsync(productId))
                .ReturnsAsync((Product?)null);

            // Act
            Func<Task> act = async () =>
                await _productService.UpdateProductAsync(
                    productId,
                    dto);

            // Assert
            await act.Should()
                .ThrowAsync<InvenTrack.Exceptions.NotFoundException>()
                .WithMessage("Product not found");

            _supplierRepositoryMock.Verify(
                x => x.GetSupplierByIdAsync(It.IsAny<int>()),
                Times.Never);

            _productRepositoryMock.Verify(
                x => x.UpdateProductAsync(It.IsAny<Product>()),
                Times.Never);
        }
    }
}
