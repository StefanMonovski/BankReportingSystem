using BankReportingSystem.Common.Exceptions;
using BankReportingSystem.Common.Resources;
using BankReportingSystem.Contracts.Partner;
using BankReportingSystem.Models;
using BankReportingSystem.Models.Shared;
using BankReportingSystem.QueryParameters.Shared;
using BankReportingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using static BankReportingSystem.Common.Constants.Constants;

namespace BankReportingSystem.Controllers
{
    [ApiController]
    [Route("partners")]
    public class PartnerController : ControllerBase
    {
        private readonly IPartnerService _partnerService;
        private readonly IWriterService _writerService;
        private readonly ILogger<PartnerController> _logger;

        public PartnerController(IPartnerService partnerService, IWriterService writerService, ILogger<PartnerController> logger)
        {
            _partnerService = partnerService;
            _writerService = writerService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Partner>> GetPartner(int id)
        {
            try
            {
                var partner = await _partnerService.GetPartnerByIdAsync(id);
                return Ok(partner);
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
        public async Task<ActionResult<Page<Partner>>> GetPartners([FromQuery] PageFilter filter, [FromQuery] bool asCsv = false)
        {
            try
            {
                var page = await _partnerService.GetPartnersAsync(filter);

                if (asCsv)
                {
                    var csvContent = _writerService.ConvertToCsv(page.Results);
                    return File(Encoding.UTF8.GetBytes(csvContent), ContentType.Csv, CsvFileName.Partners);
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
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePartner([FromBody] XmlPartner partner)
        {
            try
            {
                await _partnerService.CreatePartnerAsync(partner);
                return Created();
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
        public IActionResult UpdatePartner()
        {
            _logger.LogInformation(StringResources.UpdateNotAllowedErrorMessage);
            return StatusCode(StatusCodes.Status405MethodNotAllowed, StringResources.UpdateNotAllowedErrorMessage);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        public IActionResult DeletePartner()
        {
            _logger.LogInformation(StringResources.DeleteNotAllowedErrorMessage);
            return StatusCode(StatusCodes.Status405MethodNotAllowed, StringResources.DeleteNotAllowedErrorMessage);
        }
    }
}
