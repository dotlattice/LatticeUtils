using LatticeUtils.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatticeUtils.Core.UnitTests
{
    public static class TestConvertUtils
    {
        [TestCase("1", typeof(int), 1)]
        [TestCase(1, typeof(int), 1)]
        [TestCase(1d, typeof(int), 1)]
        [TestCase(1L, typeof(int), 1)]
        [TestCase("1", typeof(int?), 1)]
        [TestCase(1, typeof(int?), 1)]
        [TestCase(1d, typeof(int?), 1)]
        [TestCase(1L, typeof(int?), 1)]
        [TestCase("1.0", typeof(double), 1d)]
        [TestCase(1.0d, typeof(double), 1d)]
        [TestCase("string", typeof(string), "string")]
        [TestCase("", typeof(string), "")]
        [TestCase(null, typeof(string), null)]
        [TestCase(null, typeof(int?), null)]
        [TestCase("OrdinalIgnoreCase", typeof(StringComparison), StringComparison.OrdinalIgnoreCase)]
        [TestCase("OrdinalIgnoreCase", typeof(StringComparison?), StringComparison.OrdinalIgnoreCase)]
        [TestCase("ordinalignorecase", typeof(StringComparison), StringComparison.OrdinalIgnoreCase)]
        [TestCase("ordinalignorecase", typeof(StringComparison?), StringComparison.OrdinalIgnoreCase)]
        [TestCase(5, typeof(StringComparison), StringComparison.OrdinalIgnoreCase)]
        [TestCase(5L, typeof(StringComparison), StringComparison.OrdinalIgnoreCase)]
        [TestCase(5d, typeof(StringComparison), StringComparison.OrdinalIgnoreCase)]
        public static void ChangeType(object value, Type type, object expected)
        {
            Assert.AreEqual(expected, ConvertUtils.ChangeType(value, type));
        }

        [TestCase("1", typeof(string))]
        [TestCase(1, typeof(int))]
        [TestCase(1, typeof(int?))]
        [TestCase(1d, typeof(double))]
        [TestCase(1d, typeof(double?))]
        [TestCase(StringComparison.OrdinalIgnoreCase, typeof(StringComparison))]
        [TestCase(StringComparison.OrdinalIgnoreCase, typeof(StringComparison?))]
        public static void ChangeType_SameType(object value, Type type)
        {
            Assert.AreSame(value, ConvertUtils.ChangeType(value, type));
        }

        [TestCase("string", typeof(int))]
        [TestCase("string", typeof(int?))]
        [TestCase("1.0", typeof(int))]
        [TestCase("1.0", typeof(int?))]
        [TestCase("asdfasdf", typeof(DateTime))]
        [TestCase("1", typeof(bool))]
        [TestCase("1", typeof(bool?))]
        [TestCase("OrdinalIgnoreCase2", typeof(StringComparison))]
        [TestCase("OrdinalIgnoreCase2", typeof(StringComparison?))]
        public static void ChangeType_Failures(object value, Type type)
        {
            var expectedException = Assert.Throws<InvalidCastException>(() => ConvertUtils.ChangeType(value, type));
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                StringAssert.Contains(value.ToString(), expectedException.Message);
            }
        }

        [TestCase("string", typeof(int), 0)]
        [TestCase("string", typeof(int?), null)]
        [TestCase("1.0", typeof(int), 0)]
        [TestCase("1.0", typeof(int?), null)]
        [TestCase("asdfasdf", typeof(DateTime?), null)]
        [TestCase("1", typeof(bool), false)]
        [TestCase("1", typeof(bool?), null)]
        [TestCase("OrdinalIgnoreCase2", typeof(StringComparison), default(StringComparison))]
        [TestCase("OrdinalIgnoreCase2", typeof(StringComparison?), null)]
        public static void TryChangeType_Failures(object value, Type type, object expected)
        {
            Assert.AreEqual(expected, ConvertUtils.TryChangeType(value, type));
        }

    }
}
