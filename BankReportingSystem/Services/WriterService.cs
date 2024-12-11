using BankReportingSystem.Services.Interfaces;
using CsvHelper;
using System.Globalization;

namespace BankReportingSystem.Services
{
    public class WriterService : IWriterService
    {
        public string ConvertToCsv<T>(IEnumerable<T> records)
        {
            using var csv = new StringWriter();
            using var csvWriter = new CsvWriter(csv, CultureInfo.InvariantCulture);

            // Write the header (property names) for the type T
            csvWriter.WriteHeader<T>();
            csvWriter.NextRecord();

            // Write the records (data)
            csvWriter.WriteRecords(records);

            return csv.ToString();
        }
    }
}
