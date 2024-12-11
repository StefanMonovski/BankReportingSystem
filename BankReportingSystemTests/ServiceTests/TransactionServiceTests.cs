using BankReportingSystem.Common.Enums;
using BankReportingSystem.Common.Exceptions;
using BankReportingSystem.Common.Resources;
using BankReportingSystem.Contracts.Transaction;
using BankReportingSystem.Data;
using BankReportingSystem.QueryParameters;
using BankReportingSystem.Services;
using BankReportingSystemTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace BankReportingSystemTests.ServiceTests
{
    public class TransactionServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly Mock<ILogger<TransactionService>> _mockLogger;
        private readonly TransactionService _service;

        public TransactionServiceTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TransactionServiceTestDb")
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _mockLogger = new Mock<ILogger<TransactionService>>();
            _service = new TransactionService(_dbContext, _mockLogger.Object);
        }

        void IDisposable.Dispose()
        {
            // Clear the database
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();
        }

        [Fact]
        public async Task GetTransactionByIdAsync_TransactionFound_ReturnsTransaction()
        {
            // Arrange
            var transactionId = 1;
            var expectedTransaction = DefaultModelsHelper.GetTransaction();
            _dbContext.Transactions.Add(expectedTransaction);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetTransactionByIdAsync(transactionId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedTransaction.Id, result.Id);
        }

        [Fact]
        public async Task GetTransactionByIdAsync_TransactionNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var transactionId = 99;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _service.GetTransactionByIdAsync(transactionId));
            Assert.Contains(string.Format(StringResources.TransactionNotFoundErrorMessage, transactionId), exception.Message);
        }

        [Fact]
        public async Task GetTransactionsAsync_WithValidFilter_ReturnsPagedTransactions()
        {
            // Arrange
            var filter = new GetTransactionsFilter
            {
                PageNumber = 1,
                PageSize = 2
            };

            _dbContext.Transactions.Add(DefaultModelsHelper.GetTransaction());
            _dbContext.Transactions.Add(DefaultModelsHelper.GetTransaction());
            _dbContext.Transactions.Add(DefaultModelsHelper.GetTransaction());
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetTransactionsAsync(filter);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Results.Count);
            Assert.Equal(3, result.TotalCount);
        }

        [Fact]
        public async Task CreateTransactionAsync_ValidTransaction_CreatesTransaction()
        {
            // Arrange
            var newTransactions = new List<XmlTransaction>()
            {
                new()
                {
                    Amount = new XmlAmount() 
                    { 
                        Direction = TransactionDirection.Debit, 
                        Currency = "BGN",
                        Value = 100.0m
                    },
                    Debtor = new XmlAccount() { IBAN = "BG12345678901234567890" },
                    Beneficiary = new XmlAccount() { IBAN = "BG09876543210987654321" },
                    Status = TransactionStatus.Successful,
                    ExternalId = "EXT123456",
                    CreateDate = DateTime.UtcNow
                }
            };

            // Act
            await _service.CreateTransactionsAsync(1, newTransactions);

            // Assert
            var createdTransaction = await _dbContext.Transactions.FirstOrDefaultAsync(m => m.ExternalId == newTransactions.First().ExternalId);
            Assert.NotNull(createdTransaction);
            Assert.Equal(newTransactions.First().ExternalId, createdTransaction.ExternalId);
        }
    }
}