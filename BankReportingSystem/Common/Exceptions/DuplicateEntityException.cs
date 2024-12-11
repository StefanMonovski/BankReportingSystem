namespace BankReportingSystem.Common.Exceptions
{
    public class DuplicateEntityException : Exception
    {
        public DuplicateEntityException() { }

        public DuplicateEntityException(string message) : base(message) { }
    }
}
