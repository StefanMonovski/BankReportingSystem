using BankReportingSystem.Common.Enums;
using BankReportingSystem.Common.Exceptions;
using BankReportingSystem.Common.Extensions;
using BankReportingSystem.Common.Resources;
using BankReportingSystem.Contracts.Transaction;
using BankReportingSystem.Data;
using BankReportingSystem.Models;
using BankReportingSystem.Models.Shared;
using BankReportingSystem.QueryParameters;
using BankReportingSystem.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static BankReportingSystem.Common.Constants.Constants;

namespace BankReportingSystem.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(ApplicationDbContext dbContext, ILogger<TransactionService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Transaction> GetTransactionByIdAsync(int id)
        {
            _logger.LogInformation("Fetching transaction with id {id}.", id);

            var transaction = await _dbContext.Transactions
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null)
            {
                var errorMessage = string.Format(StringResources.TransactionNotFoundErrorMessage, id);
                throw new NotFoundException(errorMessage);
            }

            return transaction;
        }

        public async Task<Page<Transaction>> GetTransactionsAsync(GetTransactionsFilter filter)
        {
            _logger.LogInformation("Fetching filtered transactions.");

            var query = _dbContext.Transactions.AsQueryable();

            query = query.ApplyGetTransactionsFilter(filter);

            var totalCount = await query.CountAsync();
            var results = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new Page<Transaction>
            {
                TotalCount = totalCount,
                Results = results
            };
        }

        public async Task CreateTransactionsAsync(int merchantId, List<XmlTransaction> xmlTransactions)
        {
            _logger.LogInformation("Saving transaction to the database.");

            List<Transaction> transactions = new();

            foreach (var xmlTransaction in xmlTransactions)
            {
                transactions.Add(new Transaction()
                {
                    Direction = (TransactionDirection)xmlTransaction.Amount!.Direction!,
                    Amount = (decimal)xmlTransaction.Amount!.Value!,
                    Currency = xmlTransaction.Amount!.Currency!,
                    DebtorIBAN = xmlTransaction.Debtor!.IBAN!,
                    BeneficiaryIBAN = xmlTransaction.Beneficiary!.IBAN!,
                    Status = (TransactionStatus)xmlTransaction.Status!,
                    ExternalId = xmlTransaction.ExternalId!,
                    CreateDate = (DateTime)xmlTransaction.CreateDate!,
                    MerchantId = merchantId
                });
            }

            try
            {
                _dbContext.Transactions.AddRange(transactions);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
            {
                _logger.LogError(ex, ex.Message);

                if (sqlEx.Number == ErrorCode.UniqueConstraintViolation)
                {
                    throw new DuplicateEntityException(StringResources.DuplicateTransactionsErrorMessage);
                }

                throw;
            }

            return;
        }
    }
}
