using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace LatticeUtils
{
    /// <summary>
    /// A class that handles parsing for a specified culture.
    /// </summary>
    public abstract class Parser
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

        private Parser() { }

        /// <summary>
        /// The culture used by this parser.
        /// </summary>
        public abstract CultureInfo Culture { get; }

        /// <summary>
        /// Parses the specified string, or returns the type's default value if the parse fails.
        /// </summary>
        /// <typeparam name="T">the desired type</typeparam>
        /// <param name="text">the string to parse</param>
        /// <returns>the value or the type's default value if the parse fails</returns>
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

        /// <summary>
        /// Parses the specified string to the specified type, or returns the type's default value if the parse fails.
        /// </summary>
        /// <param name="text">the string to parse</param>
        /// <param name="destinationType">the desired type</param>
        /// <returns>the value or the type's default value if the parse fails</returns>
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

        /// <summary>
        /// Parses the specified string.
        /// </summary>
        /// <typeparam name="T">the desired type</typeparam>
        /// <param name="text">the string to parse</param>
        /// <returns>the value or the default value if the parse fails</returns>
        /// <exception cref="FormatException">if the parse fails</exception>
        public T Parse<T>(string text)
        {
            return (T)Parse(text, typeof(T));
        }

        /// <summary>
        /// Parses the specified string into an object of the specified type.
        /// </summary>
        /// <param name="text">the string to parse</param>
        /// <param name="destinationType">the desired type</param>
        /// <returns>the parsed object</returns>
        /// <exception cref="FormatException">if the parse fails</exception>
        public object Parse(string text, Type destinationType)
        {
            var typeConverter = TypeDescriptor.GetConverter(destinationType);
            try
            {
                return typeConverter.ConvertFromString(null, Culture, text);
            }
            catch (Exception ex)
            {
                // Some (most? all?) type converters throw plain Exception objects, so we have to catch everything.
                // When we throw, we always want to throw a FormatException, and we want it to contain enough information to figure out what went wrong 
                // (the original value and the destination type).
                throw new FormatException(string.Format("Failed to parse string \"{0}\" to type \"{1}\"", text, destinationType), ex);
            }
        }

        /// <summary>
        /// Attempts to parse the specified string to an integer, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the integer or null if the parse failed</returns>
        public int? TryParseNullableInt(string s)
        {
            int temp;
            return int.TryParse(s, NumberStyles.Integer, Culture, out temp) ? temp : default(int?);
        }

        /// <summary>
        /// Attempts to parse the specified string to an unsigned integer, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the unsigned integer or null if the parse failed</returns>
        public uint? TryParseNullableUnsignedInt(string s)
        {
            uint temp;
            return uint.TryParse(s, NumberStyles.Integer, Culture, out temp) ? temp : default(uint?);
        }

        /// <summary>
        /// Attempts to parse the specified string to a long, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the long or null if the parse failed</returns>
        public long? TryParseNullableLong(string s)
        {
            long temp;
            return long.TryParse(s, NumberStyles.Integer, Culture, out temp) ? temp : default(long?);
        }

        /// <summary>
        /// Attempts to parse the specified string to an unsigned long, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the unsigned long or null if the parse failed</returns>
        public ulong? TryParseNullableUnsignedLong(string s)
        {
            ulong temp;
            return ulong.TryParse(s, NumberStyles.Integer, Culture, out temp) ? temp : default(ulong?);
        }

        /// <summary>
        /// Attempts to parse the specified string to a byte, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the byte or null if the parse failed</returns>
        public byte? TryParseNullableByte(string s)
        {
            byte temp;
            return byte.TryParse(s, NumberStyles.Integer, Culture, out temp) ? temp : default(byte?);
        }

        /// <summary>
        /// Attempts to parse the specified string to a signed byte, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the signed byte or null if the parse failed</returns>
        public sbyte? TryParseNullableSignedByte(string s)
        {
            sbyte temp;
            return sbyte.TryParse(s, NumberStyles.Integer, Culture, out temp) ? temp : default(sbyte?);
        }

        /// <summary>
        /// Attempts to parse the specified string to a short, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the short or null if the parse failed</returns>
        public short? TryParseNullableShort(string s)
        {
            short temp;
            return short.TryParse(s, NumberStyles.Integer, Culture, out temp) ? temp : default(short?);
        }

        /// <summary>
        /// Attempts to parse the specified string to an unsigned short, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the unsigned short or null if the parse failed</returns>
        public ushort? TryParseNullableUnsignedShort(string s)
        {
            ushort temp;
            return ushort.TryParse(s, NumberStyles.Integer, Culture, out temp) ? temp : default(ushort?);
        }

        /// <summary>
        /// Attempts to parse the specified string to a decimal, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the integer or null if the parse failed</returns>
        public decimal? TryParseNullableDecimal(string s)
        {
            decimal temp;
            return decimal.TryParse(s, NumberStyles.Number, Culture, out temp) ? temp : default(decimal?);
        }

        /// <summary>
        /// Attempts to parse the specified string to a double, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the double or null if the parse failed</returns>
        public double? TryParseNullableDouble(string s)
        {
            double temp;
            return double.TryParse(s, NumberStyles.Number, Culture, out temp) ? temp : default(double?);
        }

        /// <summary>
        /// Attempts to parse the specified string to a float, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the float or null if the parse failed</returns>
        public float? TryParseNullableFloat(string s)
        {
            float temp;
            return float.TryParse(s, NumberStyles.Number, Culture, out temp) ? temp : default(float?);
        }

        /// <summary>
        /// Attempts to parse the specified string to a boolean, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the float or null if the parse failed</returns>
        public bool? TryParseNullableBool(string s)
        {
            bool temp;
            return bool.TryParse(s, out temp) ? temp : default(bool?);
        }

        /// <summary>
        /// Attempts to parse the specified string to a date, returning null if the parse fails.
        /// </summary>
        /// <remarks>
        /// What distinguishes this from the <c>TryParseNullableDateTime</c> method is that the returned date will never have a time component 
        /// and the date will never be adjusted by any timezone offset in the parsed string.
        /// </remarks>
        /// <param name="s">the string to parse</param>
        /// <param name="formats">any exact date formats to attempt before falling back to the default formats</param>
        /// <returns>the date or null if the parse failed</returns>
        public DateTime? TryParseNullableDate(string s, params string[] formats)
        {
            // Parse as a DateTimeOffset to make sure that the time isn't auto-adjusted to either the local or UTC timezone, which could change the date.
            var dateTimeOffset = TryParseNullableDateTimeOffset(s, formats);
            return dateTimeOffset.HasValue ? dateTimeOffset.Value.Date : default(DateTime?);
        }

        /// <summary>
        /// Attempts to parse the specified string to a DateTime, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <param name="formats">any exact date formats to attempt before falling back to the default formats</param>
        /// <returns>the DateTime or null if the parse failed</returns>
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

        /// <summary>
        /// Attempts to parse the specified string to a UTC DateTime, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <param name="formats">any exact date formats to attempt before falling back to the default formats</param>
        /// <returns>the UTC DateTime or null if the parse failed</returns>
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

        /// <summary>
        /// Attempts to parse the specified string to a DateTimeOffset, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <param name="formats">any exact date formats to attempt before falling back to the default formats</param>
        /// <returns>the DateTimeOffset or null if the parse failed</returns>
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

        /// <summary>
        /// Attempts to parse the specified string to a DateTimeOffset that defaults to a UTC offset, 
        /// returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <param name="formats">any exact date formats to attempt before falling back to the default formats</param>
        /// <returns>the DateTimeOffset or null if the parse failed</returns>
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

        /// <summary>
        /// Attempts to parse the specified string to a TimeSpan, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <param name="formats">any exact date formats to attempt before falling back to the default formats</param>
        /// <returns>the TimeSpan or null if the parse failed</returns>
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
