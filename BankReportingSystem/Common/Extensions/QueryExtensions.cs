using BankReportingSystem.Models;
using BankReportingSystem.QueryParameters;

namespace BankReportingSystem.Common.Extensions
{
    public static class QueryExtensions
    {
        public static IQueryable<Merchant> ApplyGetMerchantsFilter(this IQueryable<Merchant> query, GetMerchantsFilter filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.Country))
                query = query.Where(m => m.Country == filter.Country);

            if (filter.PartnerId.HasValue)
                query = query.Where(m => m.PartnerId == filter.PartnerId);

            return query;
        }

        public static IQueryable<Transaction> ApplyGetTransactionsFilter(this IQueryable<Transaction> query, GetTransactionsFilter filter)
        {
            if (filter.StartDate.HasValue)
                query = query.Where(t => t.CreateDate >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                query = query.Where(t => t.CreateDate <= filter.EndDate.Value);

            if (filter.Direction.HasValue)
                query = query.Where(t => t.Direction == filter.Direction);

            if (filter.MinAmount.HasValue)
                query = query.Where(t => t.Amount >= filter.MinAmount.Value);

            if (filter.MaxAmount.HasValue)
                query = query.Where(t => t.Amount <= filter.MaxAmount.Value);

            if (filter.Status.HasValue)
                query = query.Where(t => t.Status == filter.Status);

            if (filter.MerchantId.HasValue)
                query = query.Where(t => t.MerchantId == filter.MerchantId);

            return query;
        }
    }
}
