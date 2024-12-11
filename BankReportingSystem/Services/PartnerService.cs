using BankReportingSystem.Common.Exceptions;
using BankReportingSystem.Common.Resources;
using BankReportingSystem.Contracts.Partner;
using BankReportingSystem.Data;
using BankReportingSystem.Models;
using BankReportingSystem.Models.Shared;
using BankReportingSystem.QueryParameters.Shared;
using BankReportingSystem.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static BankReportingSystem.Common.Constants.Constants;

namespace BankReportingSystem.Services
{
    public class PartnerService : IPartnerService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<PartnerService> _logger;

        public PartnerService(ApplicationDbContext dbContext, ILogger<PartnerService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Partner> GetPartnerByIdAsync(int id)
        {
            _logger.LogInformation("Fetching partner with id {id}.", id);

            var partner = await _dbContext.Partners
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (partner == null)
            {
                var errorMessage = string.Format(StringResources.PartnerNotFoundErrorMessage, id);
                throw new NotFoundException(errorMessage);
            }

            return partner;
        }

        public async Task<Page<Partner>> GetPartnersAsync(PageFilter filter)
        {
            _logger.LogInformation("Fetching paged partners.");

            var query = _dbContext.Partners.AsQueryable();

            var totalCount = await query.CountAsync();
            var results = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new Page<Partner>
            {
                TotalCount = totalCount,
                Results = results
            };
        }

        public async Task CreatePartnerAsync(XmlPartner xmlPartner)
        {
            _logger.LogInformation("Saving partner to the database.");

            var partner = new Partner()
            {
                Name = xmlPartner.Name!
            };

            try
            {
                _dbContext.Partners.Add(partner);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
            {
                _logger.LogError(ex, ex.Message);

                if (sqlEx.Number == ErrorCode.UniqueConstraintViolation)
                {
                    var errorMessage = string.Format(StringResources.DuplicatePartnerErrorMessage, partner.Name);
                    throw new DuplicateEntityException(errorMessage);
                }

                throw;
            }

            return;
        }
    }
}
