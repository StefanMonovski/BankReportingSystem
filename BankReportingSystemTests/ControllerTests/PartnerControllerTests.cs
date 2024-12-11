using BankReportingSystem.Common.Exceptions;
using BankReportingSystem.Contracts.Partner;
using BankReportingSystem.Controllers;
using BankReportingSystem.Models;
using BankReportingSystem.Models.Shared;
using BankReportingSystem.QueryParameters;
using BankReportingSystem.QueryParameters.Shared;
using BankReportingSystem.Services.Interfaces;
using BankReportingSystemTests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace BankReportingSystemTests.ControllerTests
{
    public class PartnerControllerTests
    {
        private readonly Mock<IPartnerService> _mockPartnerService;
        private readonly Mock<IWriterService> _mockWriterService;
        private readonly Mock<ILogger<PartnerController>> _mockLogger;
        private readonly PartnerController _controller;

        public PartnerControllerTests()
        {
            _mockPartnerService = new Mock<IPartnerService>();
            _mockWriterService = new Mock<IWriterService>();
            _mockLogger = new Mock<ILogger<PartnerController>>();

            _controller = new PartnerController(
                _mockPartnerService.Object,
                _mockWriterService.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task GetPartner_ReturnsOkResult_WhenPartnerExists()
        {
            // Arrange
            int partnerId = 1;
            var partner = DefaultModelsHelper.GetPartner();
            _mockPartnerService.Setup(s => s.GetPartnerByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var result = await _controller.GetPartner(partnerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.Equal(partner, okResult.Value);
        }

        [Fact]
        public async Task GetPartner_ReturnsNotFound_WhenPartnerDoesNotExist()
        {
            // Arrange
            int partnerId = 1;
            _mockPartnerService.Setup(s => s.GetPartnerByIdAsync(partnerId))
                .ThrowsAsync(new NotFoundException("Partner not found"));

            // Act
            var result = await _controller.GetPartner(partnerId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetPartner_ReturnsInternalServerError_OnException()
        {
            // Arrange
            int partnerId = 1;
            _mockPartnerService.Setup(s => s.GetPartnerByIdAsync(partnerId))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetPartner(partnerId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        }

        [Fact]
        public async Task GetPartners_ReturnsOkResult_WhenCalled()
        {
            // Arrange
            var filter = new PageFilter();
            var partners = new List<Partner> { DefaultModelsHelper.GetPartner() };
            var page = new Page<Partner> { Results = partners };
            _mockPartnerService.Setup(s => s.GetPartnersAsync(filter))
                .ReturnsAsync(page);

            // Act
            var result = await _controller.GetPartners(filter);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.Equal(page, okResult.Value);
        }

        [Fact]
        public async Task GetPartners_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var filter = new PageFilter();
            _mockPartnerService.Setup(s => s.GetPartnersAsync(filter))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetPartners(filter);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task CreatePartner_ReturnsCreatedResult_WhenPartnerIsCreated()
        {
            // Arrange
            var partner = new XmlPartner();

            // Act
            var result = await _controller.CreatePartner(partner);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result);
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
        }

        [Fact]
        public async Task CreatePartner_ReturnsConflict_WhenDuplicateEntityExceptionOccurs()
        {
            // Arrange
            var partner = new XmlPartner();
            _mockPartnerService.Setup(s => s.CreatePartnerAsync(partner))
                .ThrowsAsync(new DuplicateEntityException("Partner already exists"));

            // Act
            var result = await _controller.CreatePartner(partner);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal(StatusCodes.Status409Conflict, conflictResult.StatusCode);
            Assert.Equal("Partner already exists", conflictResult.Value);
        }

        [Fact]
        public async Task CreatePartner_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var partner = new XmlPartner();
            _mockPartnerService.Setup(s => s.CreatePartnerAsync(partner))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.CreatePartner(partner);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        }

        [Fact]
        public void UpdatePartner_ReturnsMethodNotAllowed()
        {
            // Act
            var result = _controller.UpdatePartner();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status405MethodNotAllowed, statusCodeResult.StatusCode);
        }

        [Fact]
        public void DeletePartner_ReturnsMethodNotAllowed()
        {
            // Act
            var result = _controller.DeletePartner();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status405MethodNotAllowed, statusCodeResult.StatusCode);
        }
    }
}
