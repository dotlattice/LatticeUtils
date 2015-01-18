using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace LatticeUtils.Core
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

        /// <summary>
        /// A class that handles parsing for a specified culture.
        /// </summary>
        private abstract class Parser
        {
            #region Static

            private static readonly Parser invariantParser = new CultureParser(CultureInfo.InvariantCulture);
            private static readonly Parser currentParser = new CurrentCultureParser();

            /// <summary>
            /// A parser for the invariant culture.
            /// </summary>
            public static Parser InvariantCulture { get { return invariantParser; } }

            /// <summary>
            /// A parser for the current culture.
            /// </summary>
            public static Parser CurrentCulture { get { return currentParser; } }

            /// <summary>
            /// Returns a parser for the specified culture.
            /// </summary>
            /// <param name="culture">the culture for which to parse</param>
            /// <returns>a parser (never null)</returns>
            /// <exception cref="ArgumentNullException">if the culture is null</exception>
            public static Parser GetParser(CultureInfo culture)
            {
                Parser result;
                if (culture == invariantParser.Culture)
                {
                    result = invariantParser;
                }
                else
                {
                    result = new CultureParser(culture);
                }
                return result;
            }

            #endregion

            public abstract CultureInfo Culture { get; }

            public T TryParse<T>(string text)
            {
                try
                {
                    return (T)Parse(text, typeof(T));
                }
                catch (FormatException)
                {
                    return default(T);
                }
            }

            public object TryParse(string text, Type destinationType)
            {
                try
                {
                    return Parse(text, destinationType);
                }
                catch (FormatException)
                {
                    return TypeUtils.Default(destinationType);
                }
            }

            public T Parse<T>(string text)
            {
                return (T)Parse(text, typeof(T));
            }

            public object Parse(string text, Type destinationType)
            {
                var typeConverter = TypeDescriptor.GetConverter(destinationType);
                try
                {
                    return typeConverter.ConvertFromString(null, Culture, text);
                }
                catch (Exception ex)
                {
                    // Some (most? all?) type converters throw Exception objects, so we have to catch everything.
                    // When we throw, we always want to throw a FormatException, and we want it to contain enough information to figure out what went wrong 
                    // (the original value and the destination type).
                    throw new FormatException(string.Format("Failed to parse string \"{0}\" to type \"{1}\"", text, destinationType), ex.InnerException);
                }
            }

            public int? TryParseNullableInt(string s)
            {
                int temp;
                return int.TryParse(s, NumberStyles.Integer, Culture, out temp) ? temp : default(int?);
            }

            public uint? TryParseNullableUnsignedInt(string s)
            {
                uint temp;
                return uint.TryParse(s, NumberStyles.Integer, Culture, out temp) ? temp : default(uint?);
            }

            public long? TryParseNullableLong(string s)
            {
                long temp;
                return long.TryParse(s, NumberStyles.Integer, Culture, out temp) ? temp : default(long?);
            }

            public ulong? TryParseNullableUnsignedLong(string s)
            {
                ulong temp;
                return ulong.TryParse(s, NumberStyles.Integer, Culture, out temp) ? temp : default(ulong?);
            }

            public byte? TryParseNullableByte(string s)
            {
                byte temp;
                return byte.TryParse(s, NumberStyles.Integer, Culture, out temp) ? temp : default(byte?);
            }

            public sbyte? TryParseNullableSignedByte(string s)
            {
                sbyte temp;
                return sbyte.TryParse(s, NumberStyles.Integer, Culture, out temp) ? temp : default(sbyte?);
            }

            public short? TryParseNullableShort(string s)
            {
                short temp;
                return short.TryParse(s, NumberStyles.Integer, Culture, out temp) ? temp : default(short?);
            }

            public ushort? TryParseNullableUnsignedShort(string s)
            {
                ushort temp;
                return ushort.TryParse(s, NumberStyles.Integer, Culture, out temp) ? temp : default(ushort?);
            }

            public decimal? TryParseNullableDecimal(string s)
            {
                decimal temp;
                return decimal.TryParse(s, NumberStyles.Number, Culture, out temp) ? temp : default(decimal?);
            }

            public double? TryParseNullableDouble(string s)
            {
                double temp;
                return double.TryParse(s, NumberStyles.Number, Culture, out temp) ? temp : default(double?);
            }

            public float? TryParseNullableFloat(string s)
            {
                float temp;
                return float.TryParse(s, NumberStyles.Number, Culture, out temp) ? temp : default(float?);
            }

            public DateTime? TryParseNullableDateTime(string s, params string[] formats)
            {
                DateTime temp;
                if (formats != null && formats.Any())
                {
                    if (DateTime.TryParseExact(s, formats, Culture, DateTimeStyles.None, out temp))
                    {
                        return temp;
                    }
                }
                return DateTime.TryParse(s, Culture, DateTimeStyles.None, out temp) ? temp : default(DateTime?);
            }

            public DateTime? TryParseNullableDateTimeUtc(string s, params string[] formats)
            {
                DateTime temp;
                if (formats != null && formats.Any())
                {
                    if (DateTime.TryParseExact(s, formats, Culture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out temp))
                    {
                        return temp;
                    }
                }
                return DateTime.TryParse(s, Culture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out temp) ? temp : default(DateTime?);
            }

            public DateTimeOffset? TryParseNullableDateTimeOffset(string s, params string[] formats)
            {
                DateTimeOffset temp;
                if (formats != null && formats.Any())
                {
                    if (DateTimeOffset.TryParseExact(s, formats, Culture, DateTimeStyles.None, out temp))
                    {
                        return temp;
                    }
                }
                return DateTimeOffset.TryParse(s, Culture, DateTimeStyles.None, out temp) ? temp : default(DateTimeOffset?);
            }

            public DateTimeOffset? TryParseNullableDateTimeOffsetAssumeUtc(string s, params string[] formats)
            {
                DateTimeOffset temp;
                if (formats != null && formats.Any())
                {
                    if (DateTimeOffset.TryParseExact(s, formats, Culture, DateTimeStyles.AssumeUniversal, out temp))
                    {
                        return temp;
                    }
                }
                return DateTimeOffset.TryParse(s, Culture, DateTimeStyles.AssumeUniversal, out temp) ? temp : default(DateTimeOffset?);
            }

            public TimeSpan? TryParseNullableTimeSpan(string s, params string[] formats)
            {
                TimeSpan temp;
                if (formats != null && formats.Any())
                {
                    if (TimeSpan.TryParseExact(s, formats, Culture, out temp))
                    {
                        return temp;
                    }
                }
                return TimeSpan.TryParse(s, Culture, out temp) ? temp : default(TimeSpan?);
            }

            /// <summary>
            /// A Parser implementation for the current culture.
            /// </summary>
            /// <remarks>
            /// This is a separate subclass because the current culture can change (and can be different per thread).
            /// So we can't store our own reference to the current culture.
            /// </remarks>
            private class CurrentCultureParser : Parser
            {
                public override CultureInfo Culture { get { return CultureInfo.CurrentCulture; } }
            }

            /// <summary>
            /// A Parser implementation for a specified culture.
            /// </summary>
            private class CultureParser : Parser
            {
                private readonly CultureInfo culture;

                /// <summary>
                /// Creates a parser for the specified culture.
                /// </summary>
                /// <param name="culture"></param>
                public CultureParser(CultureInfo culture)
                {
                    if (culture == null) throw new ArgumentNullException("culture");
                    this.culture = culture;
                }

                public override CultureInfo Culture { get { return culture; } }
            }
        }
    }
}
