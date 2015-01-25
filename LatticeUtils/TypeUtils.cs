using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LatticeUtils
{
    /// <summary>
    /// Methods for inspecting types.
    /// </summary>
    public static class TypeUtils
    {
        /// <summary>
        /// Returns true if the specified type is nullable (either a class or a <c>System.Nullable</c> value type).
        /// </summary>
        /// <param name="type">the type to check</param>
        /// <returns>true if the type is nullable</returns>
        public static bool IsNullable(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            return !type.IsValueType || Nullable.GetUnderlyingType(type) != null;
        }

        /// <summary>
        /// If the type is <c>System.Nullable</c> then returns the underlying type, otherwise returns the specified type.
        /// </summary>
        /// <remarks>
        /// This is similar to <c>Systen.Nullable.GetUnderlyingType</c> except for the way it handles types that are not nullable.
        /// </remarks>
        /// <param name="type">the type to unwrap</param>
        /// <returns>the type or its underlying type if it is <c>System.Nullable</c></returns>
        public static Type UnwrapNullable(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            return Nullable.GetUnderlyingType(type) ?? type;
        }

        /// <summary>
        /// Returns the default value for the specified type (similar to default(type) but without having to know the type at compile time).
        /// </summary>
        /// <param name="type">the type for which </param>
        /// <returns>the default value for the specified type</returns>
        /// <exception cref="MissingMethodException">if the type has no default constructor</exception>
        public static object Default(Type type)
        {
            return IsNullable(type) ? null : Activator.CreateInstance(type);
        }

        /// <summary>
        /// Returns true if the type is numeric, which a numeric type is all of the <c>IsIntegral</c> types plus floats, doubles, and decimals.
        /// </summary>
        /// <param name="type">the type to check</param>
        /// <returns>true if the type is numeric</returns>
        public static bool IsNumeric(Type type)
        {
            var typeCode = Type.GetTypeCode(TypeUtils.UnwrapNullable(type));
            return IsNumeric(typeCode);
        }

        private static bool IsNumeric(TypeCode typeCode)
        {
            return IsIntegral(typeCode) || typeCode == TypeCode.Decimal || typeCode == TypeCode.Single || typeCode == TypeCode.Double;
        }

        /// <summary>
        /// Returns true if the type is integral, where an integral type is a signed or unsigned byte, short, int, or long.
        /// </summary>
        /// <param name="type">the type to check</param>
        /// <returns>true if the type is integral</returns>
        public static bool IsIntegral(Type type)
        {
            var typeCode = Type.GetTypeCode(TypeUtils.UnwrapNullable(type));
            return IsIntegral(typeCode);
        }

        private static bool IsIntegral(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.Int16:
                case TypeCode.UInt32:
                case TypeCode.Int32:
                case TypeCode.UInt64:
                case TypeCode.Int64:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns true if the specified type implements the specified generic interface.
        /// </summary>
        /// <param name="type">the type to check</param>
        /// <param name="interfaceGenericTypeDefinition">the generic type definition of the interface to check</param>
        /// <returns>true if the type is an instance of the interface</returns>
        public static bool ImplementsGenericInterface(Type type, Type interfaceGenericTypeDefinition)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (interfaceGenericTypeDefinition == null) throw new ArgumentNullException("interfaceGenericTypeDefinition");
            if (!interfaceGenericTypeDefinition.IsInterface) throw new ArgumentException(string.Format("Type \"{0}\" is not an interface", type.FullName));
            if (!interfaceGenericTypeDefinition.IsGenericTypeDefinition) throw new ArgumentException(string.Format("Type \"{0}\" is not an generic type definition", interfaceGenericTypeDefinition.FullName));
 
            return TypeUtils.GetInterfacesAndSelf(type).Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == interfaceGenericTypeDefinition);
        }

        /// <summary>
        /// Returns all interfaces for the specified type, including the type itself if it is an interface.
        /// </summary>
        /// <param name="type">the type from which to get interfaces</param>
        /// <returns>the interfaces implemented by the specified type</returns>
        public static IEnumerable<Type> GetInterfacesAndSelf(Type type)
        {
            IEnumerable<Type> interfaces = type.GetInterfaces();
            if (type.IsInterface)
            {
                interfaces = new[] { type }.Concat(interfaces);
            }
            return interfaces;
        }
    }
}
