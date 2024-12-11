using BankReportingSystem.Contracts.Merchant;
using BankReportingSystem.Models;
using BankReportingSystem.Models.Shared;
using BankReportingSystem.QueryParameters;

namespace BankReportingSystem.Services.Interfaces
{
    public interface IMerchantService
    {
        Task<Merchant> GetMerchantByIdAsync(int id);

        Task<Page<Merchant>> GetMerchantsAsync(GetMerchantsFilter filter);

        Task CreateMerchantAsync(int partnerId, XmlMerchant xmlMerchant);
    }
}
