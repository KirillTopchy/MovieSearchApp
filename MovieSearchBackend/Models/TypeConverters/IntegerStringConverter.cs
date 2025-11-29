using System.Text.Json;
using System.Text.Json.Serialization;

namespace MovieSearchBackend.Models.TypeConverters;

public sealed class IntegerStringConverter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Number:
                if (reader.TryGetInt32(out var parsedNumber))
                {
                    return parsedNumber;
                }
                return 0;
            case JsonTokenType.String:
                var s = reader.GetString();
                if (int.TryParse(s, out var parsedValue))
                {
                    return parsedValue;
                }
                return 0;
            default:
                return 0;
        }
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

