using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ForaFinacial.API.Models
{
    using ForaLending.API.JsonConverters;
    using Humanizer;
    using Microsoft.VisualBasic;
    using System.Text.Json.Serialization;
    using static System.Runtime.InteropServices.JavaScript.JSType;

    public class EdgarCompanyInfo
    {
        /// <summary>
        /// This needs to be a string instead of an integer because the values returned by the api sometimes contain the leading zeros.
        /// </summary>
        [JsonPropertyName("cik")]
        [JsonConverter(typeof(LeadingZeroIntConverter))]
        public int Cik { get; set; }
       

        [JsonPropertyName("entityName")]
        public string EntityName { get; set; } = "";

        [JsonPropertyName("facts")]
        public InfoFact? Facts { get; set; }

        public class InfoFact
        {
            [JsonPropertyName("us-gaap")]
            public InfoFactUsGaap? UsGaap { get; set; }
        }

        public class InfoFactUsGaap
        {
            [JsonPropertyName("NetIncomeLoss")]
            public InfoFactUsGaapNetIncomeLoss? NetIncomeLoss { get; set; }
        }

        public class InfoFactUsGaapNetIncomeLoss
        {
            [JsonPropertyName("units")]
            public InfoFactUsGaapIncomeLossUnits? Units { get; set; }
        }

        public class InfoFactUsGaapIncomeLossUnits
        {
            [JsonPropertyName("USD")]
            public InfoFactUsGaapIncomeLossUnitsUsd[]? Usd { get; set; }
        }

        public class InfoFactUsGaapIncomeLossUnitsUsd
        {

            /// <summary>
            /// Possibilities include 10-Q, 10-K,8-K, 20-F, 40-F, 6-K, and
            ///their variants.YOU ARE INTERESTED ONLY IN 10-K DATA!
            /// </summary>
            [JsonPropertyName("form")]
            public string? Form { get; set; } 

            [JsonPropertyName("frame")]
            /// <summary>
            /// For yearly information, the format is CY followed by the year
            /// number.For example: CY2021.YOU ARE INTERESTED ONLY IN YEARLY INFORMATION
            ///WHICH FOLLOWS THIS FORMAT!
            /// </summary>
            public string? Frame { get; set; } 

            [JsonPropertyName("val")]
            /// <summary>
            /// The income/loss amount.
            /// </summary>
            public decimal Val { get; set; }
        }
    }
}
