using System.ComponentModel.DataAnnotations;

namespace ForaLending.API.Models
{
    public class IncomeRecord
    {
        [Key]
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string? Form { get; set; }
        public string? Frame { get; set; }
        public decimal Val { get; set; }
        public required Company Company { get; set; }
    }
}
