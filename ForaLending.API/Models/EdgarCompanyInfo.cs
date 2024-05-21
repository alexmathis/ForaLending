using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ForaFinacial.API.Models
{
    public class EdgarCompanyInfo
    {
        [JsonPropertyName("cik")]
        public int Cik { get; set; }

        [JsonPropertyName("entityName")]
        public string EntityName { get; set; }

        [JsonPropertyName("facts")]
        public Facts facts { get; set; }

        public class Facts
        {
            [JsonPropertyName("us-gaap")]
            public UsGaap UsGaap { get; set; }
        }

        public class UsGaap
        {
            [JsonPropertyName("NetIncomeLoss")]
            public NetIncomeLoss NetIncomeLoss { get; set; }
        }

        public class NetIncomeLoss
        {
            [JsonPropertyName("units")]
            public Units Units { get; set; }
        }

        public class Units
        {
            [JsonPropertyName("USD")]
            public List<IncomeRecord> Usd { get; set; }
        }

        public class IncomeRecord
        {
            [JsonPropertyName("start")]
            public DateTime Start { get; set; }

            [JsonPropertyName("end")]
            public DateTime End { get; set; }

            [JsonPropertyName("val")]
            public decimal Val { get; set; }

            [JsonPropertyName("accn")]
            public string Accn { get; set; }

            [JsonPropertyName("fy")]
            public int Fy { get; set; }

            [JsonPropertyName("fp")]
            public string Fp { get; set; }

            [JsonPropertyName("form")]
            public string Form { get; set; }

            [JsonPropertyName("filed")]
            public DateTime Filed { get; set; }

            [JsonPropertyName("frame")]
            public string Frame { get; set; }
        }
    }
}
