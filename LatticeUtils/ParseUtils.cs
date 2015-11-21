using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace LatticeUtils
{
    /// <summary>
    /// Methods for parsing strings.
    /// </summary>
    public static class ParseUtils
    {
        /// <summary>
        /// Parses the specified string for the current culture, or returns the type's default value if the parse fails.
        /// </summary>
        /// <typeparam name="T">the desired type</typeparam>
        /// <param name="text">the string to parse</param>
        /// <returns>the value or the type's default value if the parse fails</returns>
        public static T TryParse<T>(string text)
        {
            return Parser.CurrentCulture.TryParse<T>(text);
        }

        /// <summary>
        /// Parses the specified string for the invariant culture, or returns the type's default value if the parse fails.
        /// </summary>
        /// <typeparam name="T">the desired type</typeparam>
        /// <param name="text">the string to parse</param>
        /// <returns>the value or the type's default value if the parse fails</returns>
        public static T TryParseInvariant<T>(string text)
        {
            return Parser.InvariantCulture.TryParse<T>(text);
        }

        /// <summary>
        /// Parses the specified string to the specified type for the current culture, or returns the type's default value if the parse fails.
        /// </summary>
        /// <param name="text">the string to parse</param>
        /// <param name="destinationType">the desired type</param>
        /// <returns>the value or the type's default value if the parse fails</returns>
        public static object TryParse(string text, Type destinationType)
        {
            return Parser.CurrentCulture.TryParse(text, destinationType);
        }

        /// <summary>
        /// Parses the specified string to the specified type for the invariant culture, or returns the type's default value if the parse fails.
        /// </summary>
        /// <param name="text">the string to parse</param>
        /// <param name="destinationType">the desired type</param>
        /// <returns>the value or the type's default value if the parse fails</returns>
        public static object TryParseInvariant(string text, Type destinationType)
        {
            return Parser.InvariantCulture.TryParse(text, destinationType);
        }

        /// <summary>
        /// Parses the specified string for the current culture.
        /// </summary>
        /// <typeparam name="T">the desired type</typeparam>
        /// <param name="text">the string to parse</param>
        /// <returns>the value or the default value if the parse fails</returns>
        /// <exception cref="FormatException">if the parse fails</exception>
        public static T Parse<T>(string text)
        {
            return Parser.CurrentCulture.Parse<T>(text);
        }

        /// <summary>
        /// Parses the specified string for the invariant culture.
        /// </summary>
        /// <typeparam name="T">the desired type</typeparam>
        /// <param name="text">the string to parse</param>
        /// <returns>the value or the default value if the parse fails</returns>
        /// <exception cref="FormatException">if the parse fails</exception>
        public static T ParseInvariant<T>(string text)
        {
            return Parser.InvariantCulture.Parse<T>(text);
        }

        /// <summary>
        /// Parses the specified string into an object of the specified type for the current culture.
        /// </summary>
        /// <param name="text">the string to parse</param>
        /// <param name="destinationType">the desired type</param>
        /// <returns>the parsed object</returns>
        /// <exception cref="FormatException">if the parse fails</exception>
        public static object Parse(string text, Type destinationType)
        {
            return Parser.CurrentCulture.Parse(text, destinationType);
        }

        /// <summary>
        /// Parses the specified string into an object of the specified type for the invariant culture.
        /// </summary>
        /// <param name="text">the string to parse</param>
        /// <param name="destinationType">the desired type</param>
        /// <returns>the parsed object</returns>
        /// <exception cref="FormatException">if the parse fails</exception>
        public static object ParseInvariant(string text, Type destinationType)
        {
            return Parser.InvariantCulture.Parse(text, destinationType);
        }

        /// <summary>
        /// Attempts to parse the specified string to an integer for the invariant culture, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the integer or null if the parse failed</returns>
        public static int? TryParseNullableInt(string s)
        {
            return Parser.InvariantCulture.TryParseNullableInt(s);
        }

        /// <summary>
        /// Attempts to parse the specified string to an unsigned integer for the invariant culture, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the unsigned integer or null if the parse failed</returns>
        public static uint? TryParseNullableUnsignedInt(string s)
        {
            return Parser.InvariantCulture.TryParseNullableUnsignedInt(s);
        }

        /// <summary>
        /// Attempts to parse the specified string to a long for the invariant culture, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the long or null if the parse failed</returns>
        public static long? TryParseNullableLong(string s)
        {
            long temp;
            return long.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out temp) ? temp : default(long?);
        }

        /// <summary>
        /// Attempts to parse the specified string to an unsigned long for the invariant culture, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the unsigned long or null if the parse failed</returns>
        public static ulong? TryParseNullableUnsignedLong(string s)
        {
            ulong temp;
            return ulong.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out temp) ? temp : default(ulong?);
        }

        /// <summary>
        /// Attempts to parse the specified string to a byte for the invariant culture, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the byte or null if the parse failed</returns>
        public static byte? TryParseNullableByte(string s)
        {
            return Parser.InvariantCulture.TryParseNullableByte(s);
        }

        /// <summary>
        /// Attempts to parse the specified string to a signed byte for the invariant culture, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the signed byte or null if the parse failed</returns>
        public static sbyte? TryParseNullableSignedByte(string s)
        {
            return Parser.InvariantCulture.TryParseNullableSignedByte(s);
        }

        /// <summary>
        /// Attempts to parse the specified string to a short for the invariant culture, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the short or null if the parse failed</returns>
        public static short? TryParseNullableShort(string s)
        {
            return Parser.InvariantCulture.TryParseNullableShort(s);
        }

        /// <summary>
        /// Attempts to parse the specified string to an unsigned short for the invariant culture, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the unsigned short or null if the parse failed</returns>
        public static ushort? TryParseNullableUnsignedShort(string s)
        {
            return Parser.InvariantCulture.TryParseNullableUnsignedShort(s);
        }

        /// <summary>
        /// Attempts to parse the specified string to a decimal for the current culture, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the integer or null if the parse failed</returns>
        public static decimal? TryParseNullableDecimal(string s)
        {
            return Parser.CurrentCulture.TryParseNullableDecimal(s);
        }

        /// <summary>
        /// Attempts to parse the specified string to a decimal for the invariant culture, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the integer or null if the parse failed</returns>
        public static decimal? TryParseNullableDecimalInvariant(string s)
        {
            return Parser.InvariantCulture.TryParseNullableDecimal(s);
        }

        /// <summary>
        /// Attempts to parse the specified string to a double for the current culture, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the double or null if the parse failed</returns>
        public static double? TryParseNullableDouble(string s)
        {
            return Parser.CurrentCulture.TryParseNullableDouble(s);
        }

        /// <summary>
        /// Attempts to parse the specified string to a double for the invariant culture, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the double or null if the parse failed</returns>
        public static double? TryParseNullableDoubleInvariant(string s)
        {
            return Parser.InvariantCulture.TryParseNullableDouble(s);
        }

        /// <summary>
        /// Attempts to parse the specified string to a float for the current culture, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the float or null if the parse failed</returns>
        public static float? TryParseNullableFloat(string s)
        {
            return Parser.CurrentCulture.TryParseNullableFloat(s);
        }

        /// <summary>
        /// Attempts to parse the specified string to a float for the invariant culture, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the float or null if the parse failed</returns>
        public static float? TryParseNullableFloatInvariant(string s)
        {
            return Parser.InvariantCulture.TryParseNullableFloat(s);
        }

        /// <summary>
        /// Attempts to parse the specified string to a boolean, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the float or null if the parse failed</returns>
        public static bool? TryParseNullableBool(string s)
        {
            bool temp;
            return bool.TryParse(s, out temp) ? temp : default(bool?);
        }

        /// <summary>
        /// Attempts to parse the specified string to a date for the current culture, returning null if the parse fails.
        /// </summary>
        /// <remarks>
        /// What distinguishes this from the <c>TryParseNullableDateTime</c> method is that the returned date will never have a time component 
        /// and the date will never be adjusted by any timezone offset in the parsed string.
        /// </remarks>
        /// <param name="s">the string to parse</param>
        /// <param name="formats">any exact date formats to attempt before falling back to the default formats</param>
        /// <returns>the date or null if the parse failed</returns>
        public static DateTime? TryParseNullableDate(string s, params string[] formats)
        {
            // Parse as a DateTimeOffset to make sure that the time isn't auto-adjusted to either the local or UTC timezone, which could change the date.
            var dateTimeOffset = Parser.CurrentCulture.TryParseNullableDateTimeOffset(s, formats);
            return dateTimeOffset.HasValue ? dateTimeOffset.Value.Date : default(DateTime?);
        }

        /// <summary>
        /// Attempts to parse the specified string to a date for the invariant culture, returning null if the parse fails.
        /// </summary>
        /// <remarks>
        /// What distinguishes this from the <c>TryParseNullableDateTimeUtcInvariant</c> method is that the returned date will never have a time component 
        /// and the date will never be adjusted by any timezone offset in the parsed string.
        /// </remarks>
        /// <param name="s">the string to parse</param>
        /// <param name="formats">any exact date formats to attempt before falling back to the default formats</param>
        /// <returns>the date or null if the parse failed</returns>
        public static DateTime? TryParseNullableDateInvariant(string s, params string[] formats)
        {
            // Parse as a DateTimeOffset to make sure that the time isn't auto-adjusted to either the local or UTC timezone, which could change the date.
            var dateTimeOffset = Parser.InvariantCulture.TryParseNullableDateTimeOffsetAssumeUtc(s, formats);
            return dateTimeOffset.HasValue ? dateTimeOffset.Value.Date : default(DateTime?);
        }

        /// <summary>
        /// Attempts to parse the specified string to a DateTime for the current culture, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <param name="formats">any exact date formats to attempt before falling back to the default formats</param>
        /// <returns>the DateTime or null if the parse failed</returns>
        public static DateTime? TryParseNullableDateTime(string s, params string[] formats)
        {
            return Parser.CurrentCulture.TryParseNullableDateTime(s, formats);
        }

        /// <summary>
        /// Attempts to parse the specified string to a UTC DateTime for the invariant culture, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <param name="formats">any exact date formats to attempt before falling back to the default formats</param>
        /// <returns>the UTC DateTime or null if the parse failed</returns>
        public static DateTime? TryParseNullableDateTimeUtcInvariant(string s, params string[] formats)
        {
            return Parser.InvariantCulture.TryParseNullableDateTimeUtc(s, formats);
        }

        /// <summary>
        /// Attempts to parse the specified string to a DateTimeOffset for the current culture, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <param name="formats">any exact date formats to attempt before falling back to the default formats</param>
        /// <returns>the DateTimeOffset or null if the parse failed</returns>
        public static DateTimeOffset? TryParseNullableDateTimeOffset(string s, params string[] formats)
        {
            return Parser.CurrentCulture.TryParseNullableDateTimeOffset(s, formats);
        }

        /// <summary>
        /// Attempts to parse the specified string to a DateTimeOffset that defaults to a UTC offset for the invariant culture, 
        /// returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <param name="formats">any exact date formats to attempt before falling back to the default formats</param>
        /// <returns>the DateTimeOffset or null if the parse failed</returns>
        public static DateTimeOffset? TryParseNullableDateTimeOffsetAssumeUtcInvariant(string s, params string[] formats)
        {
            return Parser.InvariantCulture.TryParseNullableDateTimeOffsetAssumeUtc(s, formats);
        }

        /// <summary>
        /// Attempts to parse the specified string to a TimeSpan for the current culture, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <param name="formats">any exact date formats to attempt before falling back to the default formats</param>
        /// <returns>the TimeSpan or null if the parse failed</returns>
        public static TimeSpan? TryParseNullableTimeSpan(string s, params string[] formats)
        {
            return Parser.CurrentCulture.TryParseNullableTimeSpan(s, formats);
        }

        /// <summary>
        /// Attempts to parse the specified string to a TimeSpan for the invariant culture, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <param name="formats">any exact date formats to attempt before falling back to the default formats</param>
        /// <returns>the TimeSpan or null if the parse failed</returns>
        public static TimeSpan? TryParseNullableTimeSpanInvariant(string s, params string[] formats)
        {
            return Parser.InvariantCulture.TryParseNullableTimeSpan(s, formats);
        }
    }
}
