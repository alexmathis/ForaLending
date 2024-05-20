using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ForaFinacial.API.Models
{
    public class EdgarCompanyInfo
    {
        [Key]
        public int Cik { get; set; }
        public required string EntityName { get; set; }
        public required InfoFact Facts { get; set; }

        public class InfoFact
        {
            [JsonPropertyName("us-gaap")]
            public required InfoFactUsGaap UsGaap { get; set; }
        }

        public class InfoFactUsGaap
        {
            public required InfoFactUsGaapNetIncomeLoss NetIncomeLoss { get; set; }
        }

        public class InfoFactUsGaapNetIncomeLoss
        {
            public required InfoFactUsGaapIncomeLossUnits Units { get; set; }
        }

        public class InfoFactUsGaapIncomeLossUnits
        {
            public required InfoFactUsGaapIncomeLossUnitsUsd[] Usd { get; set; }
        }

        public class InfoFactUsGaapIncomeLossUnitsUsd
        {
            /// <summary>
            /// Possibilities include 10-Q, 10-K,8-K, 20-F, 40-F, 6-K, and
            /// their variants. YOU ARE INTERESTED ONLY IN 10-K DATA!
            /// </summary>
            public required string Form { get; set; }
            /// <summary>
            /// For yearly information, the format is CY followed by the year
            /// number. For example: CY2021. YOU ARE INTERESTED ONLY IN YEARLY INFORMATION
            /// WHICH FOLLOWS THIS FORMAT!
            /// </summary>
            public required string Frame { get; set; }
            /// <summary>
            /// The income/loss amount.
            /// </summary>
            public decimal Val { get; set; }
        }
    }
}
