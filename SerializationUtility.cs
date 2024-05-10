using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;

namespace CodeName.Serialization
{
    // Mostly copied from https://github.com/jilleJr/Newtonsoft.Json-for-Unity.Converters/blob/master/Packages/Newtonsoft.Json-for-Unity.Converters/UnityConverters/Helpers/JsonHelperExtensions.cs
    public static class SerializationUtility
    {
        public static JsonSerializationException CreateSerializationException(this JsonReader reader, string message, [AllowNull] Exception innerException = null)
        {
            var builder = CreateStringBuilderWithSpaceAfter(message);

            builder.AppendFormat(CultureInfo.InvariantCulture, "Path '{0}'", reader.Path);

            var lineInfo = reader as IJsonLineInfo;
            int lineNumber = default;
            int linePosition = default;

            if (lineInfo?.HasLineInfo() == true)
            {
                lineNumber = lineInfo.LineNumber;
                linePosition = lineInfo.LinePosition;
                builder.AppendFormat(CultureInfo.InvariantCulture, ", line {0}, position {1}", lineNumber, linePosition);
            }

            builder.Append('.');

            return new JsonSerializationException(builder.ToString(), reader.Path, lineNumber, linePosition, innerException);
        }

        public static JsonWriterException CreateWriterException(this JsonWriter writer, string message, [AllowNull] Exception innerException = null)
        {
            var builder = CreateStringBuilderWithSpaceAfter(message);

            builder.AppendFormat(CultureInfo.InvariantCulture, "Path '{0}'.", writer.Path);

            return new JsonWriterException(
                message: builder.ToString(), writer.Path, innerException);
        }

        private static StringBuilder CreateStringBuilderWithSpaceAfter(string message)
        {
            var builder = new StringBuilder(message);

            if (message.EndsWith("."))
            {
                builder.Append(' ');
            }
            else if (!message.EndsWith(". "))
            {
                builder.Append(". ");
            }

            return builder;
        }
    }
}
