using BankReportingSystem.Common.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BankReportingSystem.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public TransactionDirection Direction { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public required string Currency { get; set; }

        [Required]
        public required string DebtorIBAN { get; set; }

        [Required]
        public required string BeneficiaryIBAN { get; set; }

        [Required]
        public TransactionStatus Status { get; set; }

        [Required]
        public required string ExternalId { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }

        [Required]
        public int MerchantId { get; set; }

        // Navigation property

        [JsonIgnore]
        [ForeignKey("MerchantId")]
        public Merchant? Merchant { get; set; }
    }
}
