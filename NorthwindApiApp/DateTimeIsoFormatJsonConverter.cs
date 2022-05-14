using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NorthwindApiApp
{
    /// <summary>
    /// Provides a json converter for <see cref="DateTime"/> using ISO format.
    /// </summary>
    public class DateTimeIsoFormatJsonConverter : JsonConverter<DateTime>
    {
        private const string Format = "yyyy-MM-ddTHH:mm:ss.fffZ";

        /// <inheritdoc/>
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.ParseExact(reader.GetString(), Format, CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            writer.WriteStringValue(value.ToUniversalTime().ToString(Format, CultureInfo.InvariantCulture));
        }
    }
}
