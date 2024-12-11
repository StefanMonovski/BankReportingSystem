using BankReportingSystem.Common.Exceptions;
using BankReportingSystem.Common.Extensions;
using BankReportingSystem.Common.Resources;
using BankReportingSystem.Contracts.Merchant;
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
    public class MerchantService : IMerchantService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<MerchantService> _logger;

        public MerchantService(ApplicationDbContext dbContext, ILogger<MerchantService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Merchant> GetMerchantByIdAsync(int id)
        {
            _logger.LogInformation("Fetching merchant with id {id}.", id);

            var merchant = await _dbContext.Merchants
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (merchant == null)
            {
                var errorMessage = string.Format(StringResources.MerchantNotFoundErrorMessage, id);
                throw new NotFoundException(errorMessage);
            }

            return merchant;
        }

        public async Task<Page<Merchant>> GetMerchantsAsync(GetMerchantsFilter filter)
        {
            _logger.LogInformation("Fetching filtered transactions.");

            var query = _dbContext.Merchants.AsQueryable();

            query = query.ApplyGetMerchantsFilter(filter);

            var totalCount = await query.CountAsync();
            var results = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new Page<Merchant>
            {
                TotalCount = totalCount,
                Results = results
            };
        }

        public async Task CreateMerchantAsync(int partnerId, XmlMerchant xmlMerchant)
        {
            _logger.LogInformation("Saving merchant to the database.");

            var merchant = new Merchant()
            {
                Name = xmlMerchant.Name!,
                URL = xmlMerchant.URL!,
                Country = xmlMerchant.Country!,
                FirstAddress = xmlMerchant.FirstAddress!,
                SecondAddress = xmlMerchant.SecondAddress!,
                BoardingDate = (DateTime)xmlMerchant.BoardingDate!,
                PartnerId = partnerId,
            };

            try
            {
                _dbContext.Merchants.Add(merchant);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
            {
                _logger.LogError(ex, ex.Message);

                if (sqlEx.Number == ErrorCode.UniqueConstraintViolation)
                {
                    var errorMessage = string.Format(StringResources.DuplicateMerchantErrorMessage, merchant.Name);
                    throw new DuplicateEntityException(errorMessage);
                }

                throw;
            }

            return;
        }
    }
}
