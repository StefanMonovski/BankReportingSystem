using System.Xml.Serialization;

namespace BankReportingSystem.Common.Enums
{
    public enum TransactionDirection
    {
        [XmlEnum("D")]
        Debit,
        [XmlEnum("C")]
        Credit
    }
}
