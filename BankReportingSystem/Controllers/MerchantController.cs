using BankReportingSystem.Common.Exceptions;
using BankReportingSystem.Common.Resources;
using BankReportingSystem.Contracts.Merchant;
using BankReportingSystem.Models;
using BankReportingSystem.Models.Shared;
using BankReportingSystem.QueryParameters;
using BankReportingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static BankReportingSystem.Common.Constants.Constants;

namespace BankReportingSystem.Controllers
{
    [ApiController]
    [Route("merchants")]
    public class MerchantController : ControllerBase
    {
        private readonly IMerchantService _merchantService;
        private readonly IPartnerService _partnerService;
        private readonly IWriterService _writerService;
        private readonly ILogger<MerchantController> _logger;

        public MerchantController(IMerchantService merchantService, IPartnerService partnerService, IWriterService writerService, ILogger<MerchantController> logger)
        {
            _merchantService = merchantService;
            _partnerService = partnerService;
            _writerService = writerService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Merchant>> GetMerchant(int id)
        {
            try
            {
                var merchant = await _merchantService.GetMerchantByIdAsync(id);
                return Ok(merchant);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, StringResources.InternalServerErrorMessage);
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Page<Merchant>>> GetMerchants([FromQuery] GetMerchantsFilter filter, [FromQuery] bool asCsv = false)
        {
            try
            {
                var page = await _merchantService.GetMerchantsAsync(filter);

                if (asCsv)
                {
                    var csvContent = _writerService.ConvertToCsv(page.Results);
                    return File(Encoding.UTF8.GetBytes(csvContent), ContentType.Csv, CsvFileName.Merchants);
                }

                return Ok(page);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, StringResources.InternalServerErrorMessage);
            }
        }

        [HttpPost]
        [Consumes(ContentType.Xml)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateMerchant([Required, FromQuery] int partnerId, [FromBody] XmlMerchant merchant)
        {
            try
            {
                //Validate partner exists (NotFoundException will be thrown)
                await _partnerService.GetPartnerByIdAsync(partnerId);

                //Persist merchant to database
                await _merchantService.CreateMerchantAsync(partnerId, merchant);
                return Created();
            }
            catch (NotFoundException ex)
            {
                _logger.LogError(ex, ex.Message);
                return NotFound(ex.Message);
            }
            catch (DuplicateEntityException ex)
            {
                _logger.LogError(ex, ex.Message);
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, StringResources.InternalServerErrorMessage);
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        public IActionResult UpdateMerchant()
        {
            _logger.LogInformation(StringResources.UpdateNotAllowedErrorMessage);
            return StatusCode(StatusCodes.Status405MethodNotAllowed, StringResources.UpdateNotAllowedErrorMessage);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        public IActionResult DeleteMerchant()
        {
            _logger.LogInformation(StringResources.DeleteNotAllowedErrorMessage);
            return StatusCode(StatusCodes.Status405MethodNotAllowed, StringResources.DeleteNotAllowedErrorMessage);
        }
    }
}
