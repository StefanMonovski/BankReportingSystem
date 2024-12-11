using System.Xml.Serialization;

namespace BankReportingSystem.Common.Enums
{
    public enum TransactionStatus
    {
        [XmlEnum("0")]
        Failed,
        [XmlEnum("1")]
        Successful
    }
}
