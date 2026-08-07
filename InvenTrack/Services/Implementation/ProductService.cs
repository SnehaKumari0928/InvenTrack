using InvenTrack.DTOs.Product;
using InvenTrack.Entities;
using InvenTrack.Repositories.Interfaces;
using InvenTrack.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InvenTrack.Services.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ISupplierRepository _supplierRepository;

        public ProductService(IProductRepository productRepository, ISupplierRepository supplierRepository)
        {
            _productRepository = productRepository;
            _supplierRepository = supplierRepository;
        }

        public async Task<ICollection<ProductResponseDto>> GetAllProductsAsync(int? supplierId,
            int? lowStock,
            string? search)
        {
            var products = await _productRepository.GetAllProductsAsync(supplierId, lowStock, search);

            return products.Select(product => new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.SKU,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                SupplierName = product.Supplier?.Name
            }).ToList();
        }

        public async Task<ProductResponseDto> CreateProductAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                SKU = dto.SKU,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                SupplierId = dto.SupplierId
            };

            var supplier = await _supplierRepository.GetSupplierByIdAsync(dto.SupplierId);
            if (supplier == null)
                throw new Exceptions.NotFoundException("Supplier not found");

            var createdProduct = await _productRepository.CreateProductAsync(product);

            return new ProductResponseDto
            {
                Id = createdProduct.Id,
                Name = createdProduct.Name,
                SKU = createdProduct.SKU,
                Price = createdProduct.Price,
                StockQuantity = createdProduct.StockQuantity,
                SupplierName = createdProduct.Supplier?.Name
            };
        }


        public async Task UpdateProductAsync(int Id ,UpdateProductDto dto)
        {
            var product = await _productRepository.GetProductByIdAsync(Id);
            if (product == null)
                throw new Exceptions.NotFoundException("Product not found");

            var supplier = await _supplierRepository.GetSupplierByIdAsync(dto.SupplierId);
            if (supplier == null)
                throw new Exceptions.NotFoundException("Supplier not found");

            product.Name = dto.Name;
            product.SKU = dto.SKU;
            product.Price = dto.Price;
            product.StockQuantity = dto.StockQuantity;
            product.SupplierId = dto.SupplierId;

            await _productRepository.UpdateProductAsync(product);
        }

        public async Task DeleteProductAsync(int ProductId)
        {
            var product = await _productRepository.GetProductByIdAsync(ProductId);
            if (product == null)
                throw new Exceptions.NotFoundException("Product not found");

            await _productRepository.DeleteProductAsync(product);
        }
        public async Task<ProductResponseDto?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null) return null;

            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.SKU,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                SupplierName = product.Supplier?.Name
            };
        }
    }
}