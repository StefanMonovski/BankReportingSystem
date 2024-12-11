using BankReportingSystem.QueryParameters.Shared;

namespace BankReportingSystem.QueryParameters
{
    public class GetMerchantsFilter : PageFilter
    {
        public string? Country { get; set; }
        public int? PartnerId { get; set; }
    }
}
