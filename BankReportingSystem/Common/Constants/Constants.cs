namespace BankReportingSystem.Common.Constants
{
    public static class Constants
    {
        public static class ErrorCode
        {
            public const int UniqueConstraintViolation = 2601;
        }

        public static class ContentType
        {
            public const string Xml = "application/xml";
            public const string Csv = "text/csv";
        }

        public static class CsvFileName
        {
            public const string Merchants = "merchants.csv";
            public const string Partners = "partners.csv";
            public const string Transactions = "transactions.csv";
        }
    }
}
