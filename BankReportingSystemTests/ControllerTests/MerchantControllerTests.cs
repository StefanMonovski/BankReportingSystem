using BankReportingSystem.Common.Exceptions;
using BankReportingSystem.Contracts.Merchant;
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
    public class MerchantControllerTests
    {
        private readonly Mock<IMerchantService> _mockMerchantService;
        private readonly Mock<IPartnerService> _mockPartnerService;
        private readonly Mock<IWriterService> _mockWriterService;
        private readonly Mock<ILogger<MerchantController>> _mockLogger;
        private readonly MerchantController _controller;

        public MerchantControllerTests()
        {
            _mockMerchantService = new Mock<IMerchantService>();
            _mockPartnerService = new Mock<IPartnerService>();
            _mockWriterService = new Mock<IWriterService>();
            _mockLogger = new Mock<ILogger<MerchantController>>();

            _controller = new MerchantController(
                _mockMerchantService.Object,
                _mockPartnerService.Object,
                _mockWriterService.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task GetMerchant_ReturnsOkResult_WhenMerchantExists()
        {
            // Arrange
            int merchantId = 1;
            var merchant = DefaultModelsHelper.GetMerchant();
            _mockMerchantService.Setup(s => s.GetMerchantByIdAsync(merchantId))
                .ReturnsAsync(merchant);

            // Act
            var result = await _controller.GetMerchant(merchantId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.Equal(merchant, okResult.Value);
        }

        [Fact]
        public async Task GetMerchant_ReturnsNotFound_WhenMerchantDoesNotExist()
        {
            // Arrange
            int merchantId = 1;
            _mockMerchantService.Setup(s => s.GetMerchantByIdAsync(merchantId))
                .ThrowsAsync(new NotFoundException("Merchant not found"));

            // Act
            var result = await _controller.GetMerchant(merchantId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetMerchant_ReturnsInternalServerError_OnException()
        {
            // Arrange
            int merchantId = 1;
            _mockMerchantService.Setup(s => s.GetMerchantByIdAsync(merchantId))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetMerchant(merchantId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        }

        [Fact]
        public async Task GetMerchants_ReturnsOkResult_WhenCalled()
        {
            // Arrange
            var filter = new GetMerchantsFilter();
            var merchants = new List<Merchant> { DefaultModelsHelper.GetMerchant() };
            var page = new Page<Merchant> { Results = merchants };
            _mockMerchantService.Setup(s => s.GetMerchantsAsync(filter))
                .ReturnsAsync(page);

            // Act
            var result = await _controller.GetMerchants(filter);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.Equal(page, okResult.Value);
        }

        [Fact]
        public async Task GetMerchants_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var filter = new GetMerchantsFilter();
            _mockMerchantService.Setup(s => s.GetMerchantsAsync(filter))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetMerchants(filter);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task CreateMerchant_ReturnsCreatedResult_WhenMerchantIsCreated()
        {
            // Arrange
            int partnerId = 1;
            var merchant = new XmlMerchant();
            _mockPartnerService.Setup(s => s.GetPartnerByIdAsync(partnerId))
                .ReturnsAsync(DefaultModelsHelper.GetPartner());

            // Act
            var result = await _controller.CreateMerchant(partnerId, merchant);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result);
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
        }

        [Fact]
        public async Task CreateMerchant_ReturnsNotFound_WhenPartnerDoesNotExist()
        {
            // Arrange
            int partnerId = 1;
            var merchant = new XmlMerchant();
            _mockPartnerService.Setup(s => s.GetPartnerByIdAsync(partnerId))
                .ThrowsAsync(new NotFoundException("Partner does not exist"));

            // Act
            var result = await _controller.CreateMerchant(partnerId, merchant);

            // Assert
            var conflictResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, conflictResult.StatusCode);
            Assert.Equal("Partner does not exist", conflictResult.Value);
        }

        [Fact]
        public async Task CreateMerchant_ReturnsConflict_WhenDuplicateEntityExceptionOccurs()
        {
            // Arrange
            int partnerId = 1;
            var merchant = new XmlMerchant();
            _mockMerchantService.Setup(s => s.CreateMerchantAsync(It.IsAny<int>(), merchant))
                .ThrowsAsync(new DuplicateEntityException("Merchant already exists"));

            // Act
            var result = await _controller.CreateMerchant(partnerId, merchant);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal(StatusCodes.Status409Conflict, conflictResult.StatusCode);
            Assert.Equal("Merchant already exists", conflictResult.Value);
        }

        [Fact]
        public async Task CreateMerchant_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            int partnerId = 1;
            var merchant = new XmlMerchant();
            _mockMerchantService.Setup(s => s.CreateMerchantAsync(It.IsAny<int>(), merchant))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.CreateMerchant(partnerId, merchant);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        }

        [Fact]
        public void UpdateMerchant_ReturnsMethodNotAllowed()
        {
            // Act
            var result = _controller.UpdateMerchant();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status405MethodNotAllowed, statusCodeResult.StatusCode);
        }

        [Fact]
        public void DeleteMerchant_ReturnsMethodNotAllowed()
        {
            // Act
            var result = _controller.DeleteMerchant();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status405MethodNotAllowed, statusCodeResult.StatusCode);
        }
    }
}
