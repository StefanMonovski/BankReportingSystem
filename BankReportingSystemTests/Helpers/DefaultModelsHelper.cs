using BankReportingSystem.Common.Enums;
using BankReportingSystem.Models;

namespace BankReportingSystemTests.Helpers
{
    public static class DefaultModelsHelper
    {
        public static Merchant GetMerchant()
        {
            var merchant = new Merchant()
            {
                Name = "Default Merchant Name",
                URL = "defaultmerchant.com",
                Country = "Default Country",
                FirstAddress = "123 Default Street",
                SecondAddress = "Suite 456",
                BoardingDate = DateTime.UtcNow,
                PartnerId = 1,
            };

            return merchant;
        }

        public static Partner GetPartner()
        {
            var partner = new Partner()
            {
                Name = "Default Partner Name",
            };

            return partner;
        }

        public static Transaction GetTransaction()
        {
            var transaction = new Transaction()
            {
                Direction = TransactionDirection.Debit,
                Amount = 100.0m,
                Currency = "BGN",
                DebtorIBAN = "BG12345678901234567890",
                BeneficiaryIBAN = "BG09876543210987654321",
                Status = TransactionStatus.Successful,
                ExternalId = "EXT123456",
                CreateDate = DateTime.UtcNow,
                MerchantId = 1,
            };

            return transaction;
        }
    }
}
