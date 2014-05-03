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
        /// Parses the specified string, or returns the type's default value if the parse fails.
        /// </summary>
        /// <typeparam name="T">the desired type</typeparam>
        /// <param name="text">the string to parse</param>
        /// <returns>the value or the type's default value if the parse fails</returns>
        public static T TryParse<T>(string text)
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
        public static object TryParse(string text, Type destinationType)
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
        public static T Parse<T>(string text)
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
        public static object Parse(string text, Type destinationType)
        {
            var typeConverter = TypeDescriptor.GetConverter(destinationType);
            try
            {
                return typeConverter.ConvertFromInvariantString(text);
            }
            catch (Exception ex)
            {
                // Some (most? all?) type converters throw Exception objects, so we have to catch everything.
                // When we throw, we always want to throw a FormatException, and we want it to contain enough information to figure out what went wrong 
                // (the original value and the destination type).
                throw new FormatException(string.Format("Failed to parse string \"{0}\" to type \"{1}\"", text, destinationType), ex.InnerException);
            }
        }

        /// <summary>
        /// Attempts to parse the specified string to an integer, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the integer or null if the parse failed</returns>
        public static int? TryParseNullableInt(string s)
        {
            int temp;
            return int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out temp) ? temp : default(int?);
        }

        /// <summary>
        /// Attempts to parse the specified string to an unsigned integer, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the unsigned integer or null if the parse failed</returns>
        public static uint? TryParseNullableUnsignedInt(string s)
        {
            uint temp;
            return uint.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out temp) ? temp : default(uint?);
        }

        /// <summary>
        /// Attempts to parse the specified string to a long, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the long or null if the parse failed</returns>
        public static long? TryParseNullableLong(string s)
        {
            long temp;
            return long.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out temp) ? temp : default(long?);
        }

        /// <summary>
        /// Attempts to parse the specified string to an unsigned long, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the unsigned long or null if the parse failed</returns>
        public static ulong? TryParseNullableUnsignedLong(string s)
        {
            ulong temp;
            return ulong.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out temp) ? temp : default(ulong?);
        }

        /// <summary>
        /// Attempts to parse the specified string to a byte, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the byte or null if the parse failed</returns>
        public static byte? TryParseNullableByte(string s)
        {
            byte temp;
            return byte.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out temp) ? temp : default(byte?);
        }

        /// <summary>
        /// Attempts to parse the specified string to a signed byte, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the signed byte or null if the parse failed</returns>
        public static sbyte? TryParseNullableSignedByte(string s)
        {
            sbyte temp;
            return sbyte.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out temp) ? temp : default(sbyte?);
        }

        /// <summary>
        /// Attempts to parse the specified string to a short, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the short or null if the parse failed</returns>
        public static short? TryParseNullableShort(string s)
        {
            short temp;
            return short.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out temp) ? temp : default(short?);
        }

        /// <summary>
        /// Attempts to parse the specified string to an unsigned short, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the unsigned short or null if the parse failed</returns>
        public static ushort? TryParseNullableUnsignedShort(string s)
        {
            ushort temp;
            return ushort.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out temp) ? temp : default(ushort?);
        }

        /// <summary>
        /// Attempts to parse the specified string to an integer, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the integer or null if the parse failed</returns>
        public static decimal? TryParseNullableDecimal(string s)
        {
            decimal temp;
            return decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out temp) ? temp : default(decimal?);
        }

        /// <summary>
        /// Attempts to parse the specified string to a double, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the double or null if the parse failed</returns>
        public static double? TryParseNullableDouble(string s)
        {
            double temp;
            return double.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out temp) ? temp : default(double?);
        }

        /// <summary>
        /// Attempts to parse the specified string to a float, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the float or null if the parse failed</returns>
        public static float? TryParseNullableFloat(string s)
        {
            float temp;
            return float.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out temp) ? temp : default(float?);
        }

        /// <summary>
        /// Attempts to parse the specified string to a float, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the float or null if the parse failed</returns>
        public static bool? TryParseNullableBool(string s)
        {
            bool temp;
            return bool.TryParse(s, out temp) ? temp : default(bool?);
        }

        /// <summary>
        /// Attempts to parse the specified string to a DateTime, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the DateTime or null if the parse failed</returns>
        public static DateTime? TryParseNullableDateTime(string s)
        {
            DateTime temp;
            var result = DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out temp) ? temp : default(DateTime?);
            if (result.HasValue)
            {
                result = DateTime.SpecifyKind(result.Value, DateTimeKind.Unspecified);
            }
            return result;
        }

        /// <summary>
        /// Attempts to parse the specified string to a DateTimeOffset, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the DateTimeOffset or null if the parse failed</returns>
        public static DateTimeOffset? TryParseNullableDateTimeOffset(string s)
        {
            DateTimeOffset temp;
            return DateTimeOffset.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out temp) ? temp : default(DateTimeOffset?);
        }

        /// <summary>
        /// Attempts to parse the specified string to a TimeSpan, returning null if the parse fails.
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the TimeSpan or null if the parse failed</returns>
        public static TimeSpan? TryParseNullableTimeSpan(string s)
        {
            TimeSpan result;
            return TimeSpan.TryParse(s, out result) ? result : default(TimeSpan?);
        }
    }
}
