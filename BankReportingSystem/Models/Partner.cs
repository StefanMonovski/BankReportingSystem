using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BankReportingSystem.Models
{
    public class Partner
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        // Navigation property

        [JsonIgnore]
        public List<Merchant> Merchants { get; set; } = [];
    }
}
