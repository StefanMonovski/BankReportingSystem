using System.ComponentModel.DataAnnotations;

namespace BankReportingSystem.Contracts.Transaction
{
    public class XmlAccount
    {
        public string? BankName { get; set; }

        public string? BIC { get; set; }

        [Required]
        public string? IBAN { get; set; }
    }
}
