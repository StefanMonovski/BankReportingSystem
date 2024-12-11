using BankReportingSystem.Common.Enums;
using BankReportingSystem.Models;
using BankReportingSystem.Services;

namespace BankReportingSystemTests.ServiceTests
{
    public class WriterServiceTests
    {
        private readonly WriterService _writerService;

        public WriterServiceTests()
        {
            _writerService = new WriterService();
        }

        [Fact]
        public void ConvertToCsv_ValidMerchantData_ReturnsCsvString()
        {
            // Arrange
            var merchants = new List<Merchant>
            {
                new()
                {
                    Id = 1,
                    Name = "Merchant One",
                    URL = "merchantone.com",
                    Country = "USA",
                    FirstAddress = "123 Main St",
                    SecondAddress = "Apt 101",
                    BoardingDate = DateTime.Parse("01/01/2024 12:00:00"),
                    PartnerId = 1
                },
                new()
                {
                    Id = 2,
                    Name = "Merchant Two",
                    URL = "merchanttwo.com",
                    Country = "Canada",
                    FirstAddress = "456 Oak St",
                    SecondAddress = "Suite 202",
                    BoardingDate = DateTime.Parse("01/01/2024 12:00:00"),
                    PartnerId = 2
                }
            };

            // Act
            var result = _writerService.ConvertToCsv(merchants);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("Id,Name,URL,Country,FirstAddress,SecondAddress,BoardingDate,PartnerId", result);  // Check if header is correct
            Assert.Contains("1,Merchant One,merchantone.com,USA,123 Main St,Apt 101,01/01/2024 12:00:00,1", result);  // Check first record
            Assert.Contains("2,Merchant Two,merchanttwo.com,Canada,456 Oak St,Suite 202,01/01/2024 12:00:00,2", result);  // Check second record
        }

        [Fact]
        public void ConvertToCsv_EmptyMerchantList_ReturnsOnlyHeader()
        {
            // Arrange
            var merchants = new List<Merchant>();

            // Act
            var result = _writerService.ConvertToCsv(merchants);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("Id,Name,URL,Country,FirstAddress,SecondAddress,BoardingDate,PartnerId", result);  // Check if header is present
            Assert.DoesNotContain("1,Merchant One,merchantone.com", result);  // Ensure no records are in the result
        }

        [Fact]
        public void ConvertToCsv_SingleMerchant_ReturnsCsvStringWithSingleRecord()
        {
            // Arrange
            var merchants = new List<Merchant>
            {
                new()
                {
                    Id = 1,
                    Name = "Merchant One",
                    URL = "merchantone.com",
                    Country = "USA",
                    FirstAddress = "123 Main St",
                    SecondAddress = "Apt 101",
                    BoardingDate = DateTime.Parse("01/01/2024 12:00:00"),
                    PartnerId = 1
                }
            };

            // Act
            var result = _writerService.ConvertToCsv(merchants);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("Id,Name,URL,Country,FirstAddress,SecondAddress,BoardingDate,PartnerId", result);  // Ensure the header is present
            Assert.Contains("1,Merchant One,merchantone.com,USA,123 Main St,Apt 101,01/01/2024 12:00:00,1", result);  // Ensure the merchant record is present
        }

        [Fact]
        public void ConvertToCsv_ValidPartnerData_ReturnsCsvString()
        {
            // Arrange
            var partners = new List<Partner>
            {
                new()
                {
                    Id = 1,
                    Name = "Partner One"
                },
                new()
                {
                    Id = 2,
                    Name = "Partner Two"
                }
            };

            // Act
            var result = _writerService.ConvertToCsv(partners);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("Id,Name", result);  // Check if header is correct
            Assert.Contains("1,Partner One", result);  // Check first record
            Assert.Contains("2,Partner Two", result);  // Check second record
        }

        [Fact]
        public void ConvertToCsv_EmptyPartnerList_ReturnsOnlyHeader()
        {
            // Arrange
            var partners = new List<Partner>();

            // Act
            var result = _writerService.ConvertToCsv(partners);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("Id,Name", result);  // Ensure header is present
            Assert.DoesNotContain("1,Partner One", result);  // Ensure no records are in the result
        }

        [Fact]
        public void ConvertToCsv_SinglePartner_ReturnsCsvStringWithSingleRecord()
        {
            // Arrange
            var partners = new List<Partner>
            {
                new()
                {
                    Id = 1,
                    Name = "Partner One"
                }
            };

            // Act
            var result = _writerService.ConvertToCsv(partners);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("Id,Name", result);  // Ensure header is present
            Assert.Contains("1,Partner One", result);  // Ensure the partner record is present
        }

        [Fact]
        public void ConvertToCsv_ValidTransactionData_ReturnsCsvString()
        {
            // Arrange
            var transactions = new List<Transaction>
            {
                new() 
                {
                    Id = 1,
                    Direction = TransactionDirection.Credit,
                    Amount = 100.5m,
                    Currency = "USD",
                    DebtorIBAN = "DE89370400440532013000",
                    BeneficiaryIBAN = "GB29NWBK60161331926819",
                    Status = TransactionStatus.Successful,
                    ExternalId = "EXT12345",
                    CreateDate = DateTime.Parse("01/01/2024 12:00:00"),
                    MerchantId = 1
                },
                new() 
                {
                    Id = 2,
                    Direction = TransactionDirection.Debit,
                    Amount = 200.75m,
                    Currency = "EUR",
                    DebtorIBAN = "DE89370400440532013001",
                    BeneficiaryIBAN = "GB29NWBK60161331926820",
                    Status = TransactionStatus.Failed,
                    ExternalId = "EXT12346",
                    CreateDate = DateTime.Parse("01/01/2024 12:00:00"),
                    MerchantId = 2
                }
            };

            // Act
            var result = _writerService.ConvertToCsv(transactions);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("Id,Direction,Amount,Currency,DebtorIBAN,BeneficiaryIBAN,Status,ExternalId,CreateDate,MerchantId", result);  // Check if header is correct
            Assert.Contains("1,Credit,100.5,USD,DE89370400440532013000,GB29NWBK60161331926819,Successful,EXT12345,01/01/2024 12:00:00,1", result);  // Check first record
            Assert.Contains("2,Debit,200.75,EUR,DE89370400440532013001,GB29NWBK60161331926820,Failed,EXT12346,01/01/2024 12:00:00,2", result);  // Check second record
        }

        [Fact]
        public void ConvertToCsv_EmptyTransactionList_ReturnsOnlyHeader()
        {
            // Arrange
            var transactions = new List<Transaction>();

            // Act
            var result = _writerService.ConvertToCsv(transactions);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("Id,Direction,Amount,Currency,DebtorIBAN,BeneficiaryIBAN,Status,ExternalId,CreateDate,MerchantId", result);  // Check if header is present
            Assert.DoesNotContain("1,Credit,100.5,USD", result);  // Ensure no records are in the result
        }

        [Fact]
        public void ConvertToCsv_SingleTransaction_ReturnsCsvStringWithSingleRecord()
        {
            // Arrange
            var transactions = new List<Transaction>
            {
                new()
                {
                    Id = 1,
                    Direction = TransactionDirection.Credit,
                    Amount = 50.00m,
                    Currency = "USD",
                    DebtorIBAN = "DE89370400440532013000",
                    BeneficiaryIBAN = "GB29NWBK60161331926819",
                    Status = TransactionStatus.Successful,
                    ExternalId = "EXT12347",
                    CreateDate = DateTime.Parse("01/01/2024 12:00:00"),
                    MerchantId = 1
                }
            };

            // Act
            var result = _writerService.ConvertToCsv(transactions);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("Id,Direction,Amount,Currency,DebtorIBAN,BeneficiaryIBAN,Status,ExternalId,CreateDate,MerchantId", result);  // Ensure the header is present
            Assert.Contains("1,Credit,50.00,USD,DE89370400440532013000,GB29NWBK60161331926819,Successful,EXT12347,01/01/2024 12:00:00,1", result);  // Ensure the transaction record is present
        }
    }
}
