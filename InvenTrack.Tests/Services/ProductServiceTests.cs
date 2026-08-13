using InvenTrack.DTOs.Product;
using InvenTrack.Entities;
using InvenTrack.Enums;
using InvenTrack.Repositories.Interfaces;
using InvenTrack.Services.Implementation;
using Moq;

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

            _productService = new ProductService(
                _productRepositoryMock.Object,
                _supplierRepositoryMock.Object);
        }

        // GetAllProducts when products exists

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
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            var firstProduct = result.First();

            Assert.Equal(1, firstProduct.Id);
            Assert.Equal("Laptop", firstProduct.Name);
            Assert.Equal("LAP001", firstProduct.SKU);
            Assert.Equal(50000, firstProduct.Price);
            Assert.Equal(10, firstProduct.StockQuantity);
            Assert.Equal("ABC Suppliers", firstProduct.SupplierName);

            var secondProduct = result.Last();

            Assert.Equal(2, secondProduct.Id);
            Assert.Equal("Mouse", secondProduct.Name);
            Assert.Equal("MOU001", secondProduct.SKU);
            Assert.Equal(1000, secondProduct.Price);
            Assert.Equal(20, secondProduct.StockQuantity);
            Assert.Equal("XYZ Suppliers", secondProduct.SupplierName);

            _productRepositoryMock.Verify(
                x => x.GetAllProductsAsync(
                    null,
                    null,
                    null),
                Times.Once);
        }



        // GetAllProducts when products doesn't exists
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
            Assert.NotNull(result);
            Assert.Empty(result);

            _productRepositoryMock.Verify(
                x => x.GetAllProductsAsync(
                    null,
                    null,
                    null),
                Times.Once);
        }


        // CreateProduct when supplier exists
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
                .Setup(x => x.CreateProductAsync(
                    It.IsAny<Product>()))
                .ReturnsAsync((Product product) =>
                {
                    product.Id = 100;
                    product.Supplier = supplier;

                    return product;
                });

            // Act
            var result = await _productService.CreateProductAsync(dto);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(100, result.Id);
            Assert.Equal(dto.Name, result.Name);
            Assert.Equal(dto.SKU, result.SKU);
            Assert.Equal(dto.Price, result.Price);
            Assert.Equal(dto.StockQuantity, result.StockQuantity);
            Assert.Equal("ABC Suppliers", result.SupplierName);

            _supplierRepositoryMock.Verify(
                x => x.GetSupplierByIdAsync(
                    dto.SupplierId),
                Times.Once);

            _productRepositoryMock.Verify(
                x => x.CreateProductAsync(
                    It.Is<Product>(product =>
                        product.Name == dto.Name &&
                        product.SKU == dto.SKU &&
                        product.Price == dto.Price &&
                        product.StockQuantity == dto.StockQuantity &&
                        product.SupplierId == dto.SupplierId)),
                Times.Once);
        }

        // CreateProduct when supplier doesn't exists
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
                .Setup(x => x.GetSupplierByIdAsync(
                    dto.SupplierId))
                .ReturnsAsync((Supplier?)null);

            // Act
            var exception = await Assert.ThrowsAsync<
                InvenTrack.Exceptions.NotFoundException>(
                () => _productService.CreateProductAsync(dto));

            // Assert
            Assert.Equal(
                "Supplier not found",
                exception.Message);

            _supplierRepositoryMock.Verify(
                x => x.GetSupplierByIdAsync(
                    dto.SupplierId),
                Times.Once);

            _productRepositoryMock.Verify(
                x => x.CreateProductAsync(
                    It.IsAny<Product>()),
                Times.Never);
        }


        // UpdateProduct when product doesn't exist
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
            var exception = await Assert.ThrowsAsync<
                InvenTrack.Exceptions.NotFoundException>(
                () => _productService.UpdateProductAsync(
                    productId,
                    dto));

            // Assert
            Assert.Equal(
                "Product not found",
                exception.Message);

            _productRepositoryMock.Verify(
                x => x.GetProductByIdAsync(productId),
                Times.Once);

            _supplierRepositoryMock.Verify(
                x => x.GetSupplierByIdAsync(
                    It.IsAny<int>()),
                Times.Never);

            _productRepositoryMock.Verify(
                x => x.UpdateProductAsync(
                    It.IsAny<Product>()),
                Times.Never);
        }

        // UpdateProduct when supplier doesn't exist throws exception
        [Fact]
        public async Task UpdateProductAsync_ShouldThrowException_WhenSupplierDoesNotExist()
        {
            // Arrange
            var productId = 1;

            var dto = new UpdateProductDto
            {
                Name = "Updated Laptop",
                SKU = "LAP002",
                Price = 60000,
                StockQuantity = 15,
                SupplierId = 999
            };

            var existingProduct = new Product
            {
                Id = productId,
                Name = "Laptop",
                SKU = "LAP001",
                Price = 50000,
                StockQuantity = 10,
                SupplierId = 1
            };

            _productRepositoryMock
                .Setup(x => x.GetProductByIdAsync(productId))
                .ReturnsAsync(existingProduct);

            _supplierRepositoryMock
                .Setup(x => x.GetSupplierByIdAsync(
                    dto.SupplierId))
                .ReturnsAsync((Supplier?)null);

            // Act
            var exception = await Assert.ThrowsAsync<
                InvenTrack.Exceptions.NotFoundException>(
                () => _productService.UpdateProductAsync(
                    productId,
                    dto));

            // Assert
            Assert.Equal(
                "Supplier not found",
                exception.Message);

            _productRepositoryMock.Verify(
                x => x.GetProductByIdAsync(productId),
                Times.Once);

            _supplierRepositoryMock.Verify(
                x => x.GetSupplierByIdAsync(
                    dto.SupplierId),
                Times.Once);

            _productRepositoryMock.Verify(
                x => x.UpdateProductAsync(
                    It.IsAny<Product>()),
                Times.Never);
        }

        // UpdateProduct when product and supplier exists

        [Fact]
        public async Task UpdateProductAsync_ShouldUpdateProduct_WhenProductAndSupplierExist()
        {
            // Arrange
            var productId = 1;

            var dto = new UpdateProductDto
            {
                Name = "Updated Laptop",
                SKU = "LAP002",
                Price = 60000,
                StockQuantity = 15,
                SupplierId = 2
            };

            var existingProduct = new Product
            {
                Id = productId,
                Name = "Laptop",
                SKU = "LAP001",
                Price = 50000,
                StockQuantity = 10,
                SupplierId = 1
            };

            var supplier = new Supplier
            {
                Id = 2,
                Name = "New Supplier"
            };

            _productRepositoryMock
                .Setup(x => x.GetProductByIdAsync(productId))
                .ReturnsAsync(existingProduct);

            _supplierRepositoryMock
                .Setup(x => x.GetSupplierByIdAsync(
                    dto.SupplierId))
                .ReturnsAsync(supplier);

            _productRepositoryMock
                .Setup(x => x.UpdateProductAsync(
                    It.IsAny<Product>()))
                .Returns(Task.CompletedTask);

            // Act
            await _productService.UpdateProductAsync(
                productId,
                dto);

            // Assert
            Assert.Equal(dto.Name, existingProduct.Name);
            Assert.Equal(dto.SKU, existingProduct.SKU);
            Assert.Equal(dto.Price, existingProduct.Price);
            Assert.Equal(dto.StockQuantity, existingProduct.StockQuantity);
            Assert.Equal(dto.SupplierId, existingProduct.SupplierId);

            _productRepositoryMock.Verify(
                x => x.GetProductByIdAsync(productId),
                Times.Once);

            _supplierRepositoryMock.Verify(
                x => x.GetSupplierByIdAsync(
                    dto.SupplierId),
                Times.Once);

            _productRepositoryMock.Verify(
                x => x.UpdateProductAsync(
                    It.Is<Product>(product =>
                        product.Id == productId &&
                        product.Name == dto.Name &&
                        product.SKU == dto.SKU &&
                        product.Price == dto.Price &&
                        product.StockQuantity == dto.StockQuantity &&
                        product.SupplierId == dto.SupplierId)),
                Times.Once);
        }


        // DeleteProduct when product doesn't exists throws exception
        [Fact]
        public async Task DeleteProductAsync_ShouldThrowException_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = 999;

            _productRepositoryMock
                .Setup(x => x.GetProductByIdAsync(productId))
                .ReturnsAsync((Product?)null);

            // Act
            var exception = await Assert.ThrowsAsync<
                InvenTrack.Exceptions.NotFoundException>(
                () => _productService.DeleteProductAsync(productId));

            // Assert
            Assert.Equal(
                "Product not found",
                exception.Message);

            _productRepositoryMock.Verify(
                x => x.GetProductByIdAsync(productId),
                Times.Once);

            _productRepositoryMock.Verify(
                x => x.DeleteProductAsync(
                    It.IsAny<Product>()),
                Times.Never);
        }

        // DeleteProduct when product when product exists
        [Fact]
        public async Task DeleteProductAsync_ShouldDeleteProduct_WhenProductExists()
        {
            // Arrange
            var productId = 1;

            var product = new Product
            {
                Id = productId,
                Name = "Laptop",
                SKU = "LAP001",
                Price = 50000,
                StockQuantity = 10,
                SupplierId = 1
            };

            _productRepositoryMock
                .Setup(x => x.GetProductByIdAsync(productId))
                .ReturnsAsync(product);

            _productRepositoryMock
                .Setup(x => x.DeleteProductAsync(product))
                .Returns(Task.CompletedTask);

            // Act
            await _productService.DeleteProductAsync(productId);

            // Assert
            _productRepositoryMock.Verify(
                x => x.GetProductByIdAsync(productId),
                Times.Once);

            _productRepositoryMock.Verify(
                x => x.DeleteProductAsync(product),
                Times.Once);
        }


        // GetProductById when product exists
        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var product = new Product
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
            };

            _productRepositoryMock
                .Setup(x => x.GetProductByIdAsync(1))
                .ReturnsAsync(product);

            // Act
            var result = await _productService.GetProductByIdAsync(1);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(product.Id, result.Id);
            Assert.Equal(product.Name, result.Name);
            Assert.Equal(product.SKU, result.SKU);
            Assert.Equal(product.Price, result.Price);
            Assert.Equal(product.StockQuantity, result.StockQuantity);
            Assert.Equal(
                "ABC Suppliers",
                result.SupplierName);

            _productRepositoryMock.Verify(
                x => x.GetProductByIdAsync(1),
                Times.Once);
        }

        // GetProductById when product doesn't exists
        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Arrange
            _productRepositoryMock
                .Setup(x => x.GetProductByIdAsync(999))
                .ReturnsAsync((Product?)null);

            // Act
            var result = await _productService.GetProductByIdAsync(999);

            // Assert
            Assert.Null(result);

            _productRepositoryMock.Verify(
                x => x.GetProductByIdAsync(999),
                Times.Once);
        }
    }
}