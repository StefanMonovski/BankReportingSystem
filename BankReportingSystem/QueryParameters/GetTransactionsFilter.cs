using BankReportingSystem.Common.Enums;
using BankReportingSystem.QueryParameters.Shared;

namespace BankReportingSystem.QueryParameters
{
    public class GetTransactionsFilter : PageFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TransactionDirection? Direction { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public TransactionStatus? Status { get; set; }
        public int? MerchantId { get; set; }
    }
}
