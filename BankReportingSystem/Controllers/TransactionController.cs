using BankReportingSystem.Common.Exceptions;
using BankReportingSystem.Common.Resources;
using BankReportingSystem.Contracts.Transaction;
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
    [Route("transactions")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IMerchantService _merchantService;
        private readonly IWriterService _writerService;
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(ITransactionService transactionService, IMerchantService merchantService, IWriterService writerService, ILogger<TransactionController> logger)
        {
            _transactionService = transactionService;
            _merchantService = merchantService;
            _writerService = writerService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
        {
            try
            {
                var transaction = await _transactionService.GetTransactionByIdAsync(id);
                return Ok(transaction);
            }
            catch (NotFoundException ex)
            {
                _logger.LogError(ex, ex.Message);
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
        public async Task<ActionResult<Page<Transaction>>> GetTransactions([FromQuery] GetTransactionsFilter filter, [FromQuery] bool asCsv = false)
        {
            try
            {
                var page = await _transactionService.GetTransactionsAsync(filter);

                if (asCsv)
                {
                    var csvContent = _writerService.ConvertToCsv(page.Results);
                    return File(Encoding.UTF8.GetBytes(csvContent), ContentType.Csv, CsvFileName.Transactions);
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
        public async Task<IActionResult> CreateTransactions([Required, FromQuery] int merchantId, [FromBody] XmlOperation operation)
        {
            try
            {
                //Validate merchant exists (NotFoundException will be thrown)
                await _merchantService.GetMerchantByIdAsync(merchantId);

                //Persist transactions to database
                await _transactionService.CreateTransactionsAsync(merchantId, operation.Transactions!);
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
        public IActionResult UpdateTransaction()
        {
            _logger.LogInformation(StringResources.UpdateNotAllowedErrorMessage);
            return StatusCode(StatusCodes.Status405MethodNotAllowed, StringResources.UpdateNotAllowedErrorMessage);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        public IActionResult DeleteTransaction()
        {
            _logger.LogInformation(StringResources.DeleteNotAllowedErrorMessage);
            return StatusCode(StatusCodes.Status405MethodNotAllowed, StringResources.DeleteNotAllowedErrorMessage);
        }
    }
}
