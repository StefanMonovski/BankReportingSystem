using BankReportingSystem.Common.Exceptions;
using BankReportingSystem.Common.Resources;
using BankReportingSystem.Contracts.Merchant;
using BankReportingSystem.Data;
using BankReportingSystem.QueryParameters;
using BankReportingSystem.Services;
using BankReportingSystemTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace BankReportingSystemTests.ServiceTests
{
    public class MerchantServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly Mock<ILogger<MerchantService>> _mockLogger;
        private readonly MerchantService _service;

        public MerchantServiceTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "MerchantServiceTestDb")
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _mockLogger = new Mock<ILogger<MerchantService>>();
            _service = new MerchantService(_dbContext, _mockLogger.Object);
        }

        void IDisposable.Dispose()
        {
            // Clear the database
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();
        }

        [Fact]
        public async Task GetMerchantByIdAsync_MerchantFound_ReturnsMerchant()
        {
            // Arrange
            var merchantId = 1;
            var expectedMerchant = DefaultModelsHelper.GetMerchant();
            _dbContext.Merchants.Add(expectedMerchant);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetMerchantByIdAsync(merchantId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedMerchant.Id, result.Id);
        }

        [Fact]
        public async Task GetMerchantByIdAsync_MerchantNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var merchantId = 99;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _service.GetMerchantByIdAsync(merchantId));
            Assert.Contains(string.Format(StringResources.MerchantNotFoundErrorMessage, merchantId), exception.Message);
        }

        [Fact]
        public async Task GetMerchantsAsync_WithValidFilter_ReturnsPagedMerchants()
        {
            // Arrange
            var filter = new GetMerchantsFilter
            {
                PageNumber = 1,
                PageSize = 2
            };

            _dbContext.Merchants.Add(DefaultModelsHelper.GetMerchant());
            _dbContext.Merchants.Add(DefaultModelsHelper.GetMerchant());
            _dbContext.Merchants.Add(DefaultModelsHelper.GetMerchant());
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetMerchantsAsync(filter);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Results.Count);
            Assert.Equal(3, result.TotalCount);
        }

        [Fact]
        public async Task CreateMerchantAsync_ValidMerchant_CreatesMerchant()
        {
            // Arrange
            var newMerchant = new XmlMerchant
            {
                Name = "New Merchant",
                URL = "newmerchant.com",
                Country = "Country",
                FirstAddress = "Address 1",
                SecondAddress = "Address 2",
                BoardingDate = DateTime.Now
            };

            // Act
            await _service.CreateMerchantAsync(1, newMerchant);

            // Assert
            var createdMerchant = await _dbContext.Merchants.FirstOrDefaultAsync(m => m.Name == newMerchant.Name);
            Assert.NotNull(createdMerchant);
            Assert.Equal(newMerchant.Name, createdMerchant.Name);
        }
    }
}
