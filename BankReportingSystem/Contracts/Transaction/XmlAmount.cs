using BankReportingSystem.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace BankReportingSystem.Contracts.Transaction
{
    public class XmlAmount
    {
        [Required]
        public TransactionDirection? Direction { get; set; }

        [Required]
        public decimal? Value { get; set; }

        [Required]
        public string? Currency { get; set; }
    }
}
