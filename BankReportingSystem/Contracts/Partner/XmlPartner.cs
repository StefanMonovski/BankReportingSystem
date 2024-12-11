using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace BankReportingSystem.Contracts.Partner
{
    [XmlRoot("Partner")]
    public class XmlPartner
    {
        [Required]
        public string? Name { get; set; }
    }
}
