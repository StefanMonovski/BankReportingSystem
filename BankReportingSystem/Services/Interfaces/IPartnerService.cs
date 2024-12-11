using BankReportingSystem.Contracts.Partner;
using BankReportingSystem.Models;
using BankReportingSystem.Models.Shared;
using BankReportingSystem.QueryParameters.Shared;

namespace BankReportingSystem.Services.Interfaces
{
    public interface IPartnerService
    {
        Task<Partner> GetPartnerByIdAsync(int id);

        Task<Page<Partner>> GetPartnersAsync(PageFilter filter);

        Task CreatePartnerAsync(XmlPartner xmlPartner);
    }
}
