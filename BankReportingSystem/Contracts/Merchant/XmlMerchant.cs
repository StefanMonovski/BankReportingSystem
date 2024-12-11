using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace BankReportingSystem.Contracts.Merchant
{
    [XmlRoot("Merchant")]
    public class XmlMerchant
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        public string? URL { get; set; }

        [Required]
        public string? Country { get; set; }

        [Required]
        public string? FirstAddress { get; set; }

        [Required]
        public string? SecondAddress { get; set; }

        [Required]
        public DateTime? BoardingDate { get; set; }
    }
}
