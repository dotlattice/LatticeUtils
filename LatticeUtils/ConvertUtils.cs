using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace LatticeUtils
{
    /// <summary>
    /// Methods for changing the type of an object.
    /// </summary>
    public static class ConvertUtils
    {
        /// <summary>
        /// Attempts to convert the value to a different type.
        /// </summary>
        /// <typeparam name="T">the desired type</typeparam>
        /// <param name="value">the value to convert</param>
        /// <returns>the converted value or the default for the type if the conversion fails</returns>
        public static T TryChangeType<T>(object value)
        {
            try
            {
                return (T)ChangeType(value, typeof(T));
            }
            catch (InvalidCastException)
            {
                return default(T);
            }
        }

        /// <summary>
        /// Attempts to convert the value to the specified type.
        /// </summary>
        /// <param name="value">the value to convert</param>
        /// <param name="conversionType">the desired type</param>
        /// <returns>the converted value or the default for the type if the conversion fails</returns>
        public static object TryChangeType(object value, Type conversionType)
        {
            try
            {
                return ChangeType(value, conversionType);
            }
            catch (InvalidCastException)
            {
                return TypeUtils.Default(conversionType);
            }
        }

        /// <summary>
        /// Converts the value to a different type.
        /// </summary>
        /// <typeparam name="T">the desired type</typeparam>
        /// <param name="value">the value to convert</param>
        /// <returns>the converted value</returns>
        /// <exception cref="InvalidCastException">if the conversion fails</exception>
        public static T ChangeType<T>(object value)
        {
            return (T)ChangeType(value, typeof(T));
        }

        /// <summary>
        /// Converts the value to the specified type.
        /// </summary>
        /// <param name="value">the value to convert</param>
        /// <param name="conversionType">the desired type</param>
        /// <returns>the converted value</returns>
        /// <exception cref="InvalidCastException">if the conversion fails</exception>
        public static object ChangeType(object value, Type conversionType)
        {
            if (value == null || Convert.IsDBNull(value))
            {
                return TypeUtils.Default(conversionType);
            }

            var valueType = value.GetType();
            var unwrappedValueType = TypeUtils.UnwrapNullable(valueType);
            var unwrappedConversionType = TypeUtils.UnwrapNullable(conversionType);
            if (unwrappedConversionType.IsAssignableFrom(unwrappedValueType))
            {
                return value;
            }

            if (unwrappedValueType == typeof(string))
            {
                try
                {
                    return ParseUtils.ParseInvariant((string)value, conversionType);
                }
                catch (FormatException ex)
                {
                    throw new InvalidCastException(string.Format("Cannot change type of value \"{0}\" (type {1}) to type \"{1}\"", value, unwrappedValueType.Name, unwrappedConversionType.Name), ex);
                }
            }

            if (unwrappedConversionType.IsEnum)
            {
                return Enum.Parse(conversionType, value.ToString(), ignoreCase: true);
            }

            if (typeof(IConvertible).IsAssignableFrom(unwrappedValueType) && IsConvertibleOutputType(unwrappedConversionType))
            {
                return Convert.ChangeType(value, unwrappedConversionType);
            }

            var typeConverter = TypeDescriptor.GetConverter(conversionType);
            try
            {
                return typeConverter.ConvertFrom(value);
            }
            catch (Exception ex)
            {
                throw new InvalidCastException(string.Format("Cannot change type of value \"{0}\" (type {1}) to type \"{1}\"", value, unwrappedValueType.Name, unwrappedConversionType.Name), ex);
            }
        }

        private static bool IsConvertibleOutputType(Type type)
        {
            return TypeUtils.IsNumeric(type) || type == typeof(bool);
        }
    }
}
