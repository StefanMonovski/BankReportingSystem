namespace BankReportingSystem.Models.Shared
{
    public class Page<T>
    {
        public int TotalCount { get; set; }
        public List<T> Results { get; set; } = [];
    }
}
