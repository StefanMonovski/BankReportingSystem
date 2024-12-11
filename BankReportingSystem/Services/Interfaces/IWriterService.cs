namespace BankReportingSystem.Services.Interfaces
{
    public interface IWriterService
    {
        string ConvertToCsv<T>(IEnumerable<T> records);
    }
}
