using BankReportingSystem.Contracts.Transaction;
using BankReportingSystem.Models;
using BankReportingSystem.Models.Shared;
using BankReportingSystem.QueryParameters;

namespace BankReportingSystem.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<Transaction> GetTransactionByIdAsync(int id);

        Task<Page<Transaction>> GetTransactionsAsync(GetTransactionsFilter filter);

        Task CreateTransactionsAsync(int merchantId, List<XmlTransaction> xmlTransactions);
    }
}
