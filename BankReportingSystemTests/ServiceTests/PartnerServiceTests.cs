using BankReportingSystem.Common.Exceptions;
using BankReportingSystem.Common.Resources;
using BankReportingSystem.Contracts.Partner;
using BankReportingSystem.Data;
using BankReportingSystem.QueryParameters.Shared;
using BankReportingSystem.Services;
using BankReportingSystemTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace BankReportingSystemTests.ServiceTests
{
    public class PartnerServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly Mock<ILogger<PartnerService>> _mockLogger;
        private readonly PartnerService _service;

        public PartnerServiceTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "PartnerServiceTestDb")
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _mockLogger = new Mock<ILogger<PartnerService>>();
            _service = new PartnerService(_dbContext, _mockLogger.Object);
        }

        void IDisposable.Dispose()
        {
            // Clear the database
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();
        }

        [Fact]
        public async Task GetPartnerByIdAsync_PartnerFound_ReturnsPartner()
        {
            // Arrange
            var partnerId = 1;
            var expectedPartner = DefaultModelsHelper.GetPartner();
            _dbContext.Partners.Add(expectedPartner);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetPartnerByIdAsync(partnerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedPartner.Id, result.Id);
        }

        [Fact]
        public async Task GetPartnerByIdAsync_PartnerNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var partnerId = 99;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _service.GetPartnerByIdAsync(partnerId));
            Assert.Contains(string.Format(StringResources.PartnerNotFoundErrorMessage, partnerId), exception.Message);
        }

        [Fact]
        public async Task GetPartnersAsync_WithValidFilter_ReturnsPagedPartners()
        {
            // Arrange
            var filter = new PageFilter
            {
                PageNumber = 1,
                PageSize = 2
            };

            _dbContext.Partners.Add(DefaultModelsHelper.GetPartner());
            _dbContext.Partners.Add(DefaultModelsHelper.GetPartner());
            _dbContext.Partners.Add(DefaultModelsHelper.GetPartner());
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetPartnersAsync(filter);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Results.Count);
            Assert.Equal(3, result.TotalCount);
        }

        [Fact]
        public async Task CreatePartnerAsync_ValidPartner_CreatesPartner()
        {
            // Arrange
            var newPartner = new XmlPartner
            {
                Name = "New Partner"
            };

            // Act
            await _service.CreatePartnerAsync(newPartner);

            // Assert
            var createdPartner = await _dbContext.Partners.FirstOrDefaultAsync(m => m.Name == newPartner.Name);
            Assert.NotNull(createdPartner);
            Assert.Equal(newPartner.Name, createdPartner.Name);
        }
    }
}
