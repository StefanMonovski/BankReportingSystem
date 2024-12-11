using BankReportingSystem.Common.Exceptions;
using BankReportingSystem.Contracts.Transaction;
using BankReportingSystem.Controllers;
using BankReportingSystem.Models;
using BankReportingSystem.Models.Shared;
using BankReportingSystem.QueryParameters;
using BankReportingSystem.Services.Interfaces;
using BankReportingSystemTests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace BankReportingSystemTests.ControllerTests
{
    public class TransactionControllerTests
    {
        private readonly Mock<ITransactionService> _mockTransactionService;
        private readonly Mock<IMerchantService> _mockMerchantService;
        private readonly Mock<IWriterService> _mockWriterService;
        private readonly Mock<ILogger<TransactionController>> _mockLogger;
        private readonly TransactionController _controller;

        public TransactionControllerTests()
        {
            _mockTransactionService = new Mock<ITransactionService>();
            _mockMerchantService = new Mock<IMerchantService>();
            _mockWriterService = new Mock<IWriterService>();
            _mockLogger = new Mock<ILogger<TransactionController>>();

            _controller = new TransactionController(
                _mockTransactionService.Object,
                _mockMerchantService.Object,
                _mockWriterService.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task GetTransaction_ReturnsOkResult_WhenTransactionExists()
        {
            // Arrange
            int transactionId = 1;
            var transaction = DefaultModelsHelper.GetTransaction();
            _mockTransactionService.Setup(s => s.GetTransactionByIdAsync(transactionId))
                .ReturnsAsync(transaction);

            // Act
            var result = await _controller.GetTransaction(transactionId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.Equal(transaction, okResult.Value);
        }

        [Fact]
        public async Task GetTransaction_ReturnsNotFound_WhenTransactionDoesNotExist()
        {
            // Arrange
            int transactionId = 1;
            _mockTransactionService.Setup(s => s.GetTransactionByIdAsync(transactionId))
                .ThrowsAsync(new NotFoundException("Transaction not found"));

            // Act
            var result = await _controller.GetTransaction(transactionId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetTransaction_ReturnsInternalServerError_OnException()
        {
            // Arrange
            int transactionId = 1;
            _mockTransactionService.Setup(s => s.GetTransactionByIdAsync(transactionId))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetTransaction(transactionId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        }

        [Fact]
        public async Task GetTransactions_ReturnsOkResult_WhenCalled()
        {
            // Arrange
            var filter = new GetTransactionsFilter();
            var transactions = new List<Transaction> { DefaultModelsHelper.GetTransaction() };
            var page = new Page<Transaction> { Results = transactions };
            _mockTransactionService.Setup(s => s.GetTransactionsAsync(filter))
                .ReturnsAsync(page);

            // Act
            var result = await _controller.GetTransactions(filter);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.Equal(page, okResult.Value);
        }

        [Fact]
        public async Task GetTransactions_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var filter = new GetTransactionsFilter();
            _mockTransactionService.Setup(s => s.GetTransactionsAsync(filter))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetTransactions(filter);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task CreateTransactions_ReturnsCreatedResult_WhenTransactionsAreCreated()
        {
            // Arrange
            int merchantId = 1;
            var operation = new XmlOperation();
            _mockMerchantService.Setup(s => s.GetMerchantByIdAsync(merchantId))
                .ReturnsAsync(DefaultModelsHelper.GetMerchant());

            // Act
            var result = await _controller.CreateTransactions(merchantId, operation);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result);
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
        }

        [Fact]
        public async Task CreateTransactions_ReturnsNotFound_WhenMerchantDoesNotExist()
        {
            // Arrange
            int merchantId = 1;
            var operation = new XmlOperation();
            _mockMerchantService.Setup(s => s.GetMerchantByIdAsync(merchantId))
                .ThrowsAsync(new NotFoundException("Merchant does not exist"));

            // Act
            var result = await _controller.CreateTransactions(merchantId, operation);

            // Assert
            var conflictResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, conflictResult.StatusCode);
            Assert.Equal("Merchant does not exist", conflictResult.Value);
        }

        [Fact]
        public async Task CreateTransaction_ReturnsConflict_WhenDuplicateEntityExceptionOccurs()
        {
            // Arrange
            int merchantId = 1;
            var operation = new XmlOperation() { Transactions = new() };
            _mockTransactionService.Setup(s => s.CreateTransactionsAsync(It.IsAny<int>(), operation.Transactions))
                .ThrowsAsync(new DuplicateEntityException("Transaction already exists"));

            // Act
            var result = await _controller.CreateTransactions(merchantId, operation);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal(StatusCodes.Status409Conflict, conflictResult.StatusCode);
            Assert.Equal("Transaction already exists", conflictResult.Value);
        }

        [Fact]
        public async Task CreateTransaction_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            int merchantId = 1;
            var operation = new XmlOperation() { Transactions = new() };
            _mockTransactionService.Setup(s => s.CreateTransactionsAsync(It.IsAny<int>(), operation.Transactions))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.CreateTransactions(merchantId, operation);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        }

        [Fact]
        public void UpdateTransaction_ReturnsMethodNotAllowed()
        {
            // Act
            var result = _controller.UpdateTransaction();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status405MethodNotAllowed, statusCodeResult.StatusCode);
        }

        [Fact]
        public void DeleteTransaction_ReturnsMethodNotAllowed()
        {
            // Act
            var result = _controller.DeleteTransaction();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status405MethodNotAllowed, statusCodeResult.StatusCode);
        }
    }
}
