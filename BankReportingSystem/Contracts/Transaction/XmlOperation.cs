using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace BankReportingSystem.Contracts.Transaction
{
    [XmlRoot("Operation")]
    public class XmlOperation
    {
        public DateTime? FileDate { get; set; }

        [Required]
        [XmlArrayItem("Transaction")]
        public List<XmlTransaction>? Transactions { get; set; }
    }
}
