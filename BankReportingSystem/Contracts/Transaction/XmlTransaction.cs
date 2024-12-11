using BankReportingSystem.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace BankReportingSystem.Contracts.Transaction
{
    public class XmlTransaction
    {
        [Required]
        public string? ExternalId { get; set; }
        
        [Required]
        public DateTime? CreateDate { get; set; }

        [Required]
        public XmlAmount? Amount { get; set; }

        [Required]
        public TransactionStatus? Status { get; set; }

        [Required]
        public XmlAccount? Debtor { get; set; }

        [Required]
        public XmlAccount? Beneficiary { get; set; }
    }
}
