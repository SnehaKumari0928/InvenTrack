using FluentAssertions;
using InvenTrack.DTOs.Supplier;
using InvenTrack.Entities;
using InvenTrack.Exceptions;
using InvenTrack.Repositories.Interfaces;
using InvenTrack.Services.Implementation;
using InvenTrack.Tests.Data;
using Moq;

namespace InvenTrack.Tests.Services
{
    public class SupplierServiceTests
    {

        private readonly Mock<ISupplierRepository> _repositoryMock;
        private readonly SupplierService _supplierService;

        public SupplierServiceTests()
        {
            _repositoryMock = new Mock<ISupplierRepository>();
            _supplierService = new SupplierService(
                _repositoryMock.Object);

        }


        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task DeleteSupplierAsync_WhenSupplierExists_ShouldDeleteSupplier(int SupplierId)
        {

            // Arrange
            var supplier = new Supplier
            {
                Id = SupplierId,
                Name = "ABC Supplier",
                Email = "abc@gmail.com",
                Phone = "9999999999",
                Address = "Delhi"
            };

            _repositoryMock.Setup(x => x.GetSupplierByIdAsync(SupplierId))
                .ReturnsAsync(supplier);

            // Act

            await _supplierService.DeleteSupplierAsync(SupplierId);

            // Assert

            _repositoryMock.Verify(
                x => x.DeleteSupplierAsync(supplier), Times.Once);
        }


        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(9999)]
        public async Task DeleteSupplierAsync_WhenSupplierDoesNotExist_ShouldThrowNotFoundException(int SupplierId)
        {
            // Arrange
            var supplier = new Supplier
            {
                Id = SupplierId,
                Name = "ABC Supplier",
                Email = "abc@gmail.com",
                Phone = "9999999999",
                Address = "Delhi"
            };

            _repositoryMock.Setup(x => x.GetSupplierByIdAsync(SupplierId))
                .ReturnsAsync((Supplier?)null);


            Func<Task> act = () => _supplierService.DeleteSupplierAsync(SupplierId);

            await act.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage("Supplier not found");


            _repositoryMock.Verify(
                x => x.DeleteSupplierAsync(It.IsAny<Supplier>()), Times.Never);
        }

        public static IEnumerable<object[]> SupplierData => new List<object[]>
        {
            new object[]
            {
                new UpdateSupplierDto
                {
                 Name = "ABC Supplier",
                Email = "abc@gmail.com",
                Phone = "9999999999",
                Address = "Delhi"
                }
            },
            new object[] {
                new UpdateSupplierDto
                {
                Name = "XYZ Supplier",
                Email = "xyz@gmail.com",
                Phone = "8888888888",
                Address = "Mumbai"
                }
            },

             new object[]
        {
            new UpdateSupplierDto
            {
                Name = "PQR Supplier",
                Email = "pqr@gmail.com",
                Phone = "7777777777",
                Address = "Pune"
            }
        }

        };


        [Theory]
        [MemberData(nameof(SupplierData))]
        public async Task UpdateSupplierAsync_ShouldUpdateSupplier(UpdateSupplierDto dto)
        {
            // Arrange

            var supplier = new Supplier
            {
                Id = 1,
                Name = "Old Name",
                Email = "old@gmail.com",
                Phone = "0000000000",
                Address = "Old Address"
            };

            var SupplierId = 1;

            _repositoryMock.Setup(x => x.GetSupplierByIdAsync(SupplierId))
                .ReturnsAsync(supplier);


            await _supplierService.UpdateSupplierAsync(SupplierId, dto);


            supplier.Name.Should().Be(dto.Name);
            supplier.Email.Should().Be(dto.Email);
            supplier.Phone.Should().Be(dto.Phone);
            supplier.Address.Should().Be(dto.Address);

            _repositoryMock.Verify(
                x => x.UpdateSupplierAsync(supplier),
                Times.Once);
        }

        [Theory]
        [ClassData(typeof(SupplierTestData))]
        public async Task UpdateSupplierAsync_ShouldUpdateSuppliers(
    UpdateSupplierDto dto)
        {
            // Arrange
            var supplier = new Supplier
            {
                Id = 1,
                Name = "Old Supplier",
                Email = "old@gmail.com",
                Phone = "0000000000",
                Address = "Old Address"
            };

            _repositoryMock
                .Setup(x => x.GetSupplierByIdAsync(1))
                .ReturnsAsync(supplier);

            // Act
            await _supplierService.UpdateSupplierAsync(1, dto);

            // Assert
            supplier.Name.Should().Be(dto.Name);
            supplier.Email.Should().Be(dto.Email);
            supplier.Phone.Should().Be(dto.Phone);
            supplier.Address.Should().Be(dto.Address);

            _repositoryMock.Verify(
                x => x.UpdateSupplierAsync(supplier),
                Times.Once);
        }
    }
}
