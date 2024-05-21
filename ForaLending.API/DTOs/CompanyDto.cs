
using ForaLending.API.JsonConverters;
using System.Text.Json.Serialization;

namespace ForaLending.API.DTOs
{

    public class CompanyDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        [JsonConverter(typeof(DecimalConverter))]
        public decimal StandardFundableAmount { get; set; }

        [JsonConverter(typeof(DecimalConverter))]
        public decimal SpecialFundableAmount { get; set; }
    }
}
