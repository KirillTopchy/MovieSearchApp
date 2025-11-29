using System.Text.Json;
using System.Text.Json.Serialization;

namespace MovieSearchBackend.Models.TypeConverters;

public sealed class BooleanStringConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var s = reader.GetString();
            if (string.Equals(s, "True", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (string.Equals(s, "False", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        if (reader.TokenType == JsonTokenType.True)
        {
            return true;
        }

        if (reader.TokenType == JsonTokenType.False)
        {
            return false;
        }

        return false;
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value ? "True" : "False");
    }
}
