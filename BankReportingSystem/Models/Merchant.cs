using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BankReportingSystem.Models
{
    public class Merchant
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public required string URL { get; set; }

        [Required]
        public required string Country { get; set; }

        [Required]
        public required string FirstAddress { get; set; }

        [Required]
        public required string SecondAddress { get; set; }

        [Required]
        public DateTime BoardingDate { get; set; }

        [Required]
        public int PartnerId { get; set; }

        // Navigation properties

        [JsonIgnore]
        [ForeignKey("PartnerId")]
        public Partner? Partner { get; set; }

        [JsonIgnore]
        public List<Transaction> Transactions { get; set; } = [];
    }
}
