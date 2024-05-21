using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ForaLending.API
{
    public class LeadingZeroIntConverter : JsonConverter<int>
    {
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string stringValue = reader.GetString() ?? "";
                if (int.TryParse(stringValue, out int intValue))
                {
                    return intValue;
                }
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetInt32();
            }
            throw new JsonException("Invalid CIK format");
        }

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
}
