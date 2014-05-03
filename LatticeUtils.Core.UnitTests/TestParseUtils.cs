using LatticeUtils.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatticeUtils.Core.UnitTests
{
    public static class TestParseUtils
    {
        [TestCase("1", typeof(int), 1)]
        [TestCase("1", typeof(int?), 1)]
        [TestCase("1.0", typeof(double), 1d)]
        [TestCase("string", typeof(string), "string")]
        [TestCase("", typeof(string), "")]
        [TestCase(null, typeof(string), "")]
        [TestCase(null, typeof(int?), null)]
        [TestCase("true", typeof(bool), true)]
        [TestCase("true", typeof(bool?), true)]
        [TestCase("OrdinalIgnoreCase", typeof(StringComparison), StringComparison.OrdinalIgnoreCase)]
        [TestCase("OrdinalIgnoreCase", typeof(StringComparison?), StringComparison.OrdinalIgnoreCase)]
        [TestCase("ordinalignorecase", typeof(StringComparison), StringComparison.OrdinalIgnoreCase)]
        [TestCase("ordinalignorecase", typeof(StringComparison?), StringComparison.OrdinalIgnoreCase)]
        public static void Parse(string s, Type type, object expected)
        {
            Assert.AreEqual(expected, ParseUtils.Parse(s, type));
        }

        [TestCase("string", typeof(int))]
        [TestCase("string", typeof(int?))]
        [TestCase("1.0", typeof(int))]
        [TestCase("1.0", typeof(int?))]
        [TestCase(null, typeof(int))]
        [TestCase("asdfasdf", typeof(DateTime))]
        [TestCase("1", typeof(bool))]
        [TestCase("1", typeof(bool?))]
        [TestCase("OrdinalIgnoreCase2", typeof(StringComparison))]
        [TestCase("OrdinalIgnoreCase2", typeof(StringComparison?))]
        public static void Parse_Failures(string s, Type type)
        {
            var expectedException = Assert.Throws<FormatException>(() => ParseUtils.Parse(s, type));
            if (!string.IsNullOrEmpty(s))
            {
                StringAssert.Contains(s, expectedException.Message);
            }
        }

        [Test]
        public static void Parse_DateTime()
        {
            Assert.AreEqual(new DateTime(1999, 12, 31), ParseUtils.Parse<DateTime>("1999-12-31"));
            Assert.AreEqual(new DateTime(1999, 12, 31), ParseUtils.Parse<DateTime?>("1999-12-31"));
            Assert.AreEqual(new DateTime(1999, 12, 31, 1, 2, 3), ParseUtils.Parse<DateTime>("1999-12-31 01:02:03"));
            Assert.AreEqual(new DateTime(1999, 12, 31, 13, 2, 3), ParseUtils.Parse<DateTime>("12/31/1999 01:02:03 PM"));
        }

        [Test]
        public static void Parse_DateTimeOffset()
        {
            Assert.AreEqual(new DateTimeOffset(1999, 12, 31, 1, 2, 3, TimeSpan.Zero), ParseUtils.Parse<DateTimeOffset>("12/31/1999 01:02:03 +00:00"));
            Assert.AreEqual(new DateTimeOffset(1999, 12, 31, 1, 2, 3, TimeSpan.Zero), ParseUtils.Parse<DateTimeOffset?>("12/31/1999 01:02:03 +00:00"));

        }

        [Test]
        public static void Parse_TimeSpan()
        {
            Assert.AreEqual(new TimeSpan(1, 2, 3), ParseUtils.Parse<TimeSpan>("01:02:03"));
            Assert.AreEqual(new TimeSpan(1, 2, 3), ParseUtils.Parse<TimeSpan?>("01:02:03"));
            Assert.AreEqual(TimeSpan.FromHours(8), ParseUtils.Parse<TimeSpan>("08:00"));
            Assert.AreEqual(new TimeSpan(1, 2, 3, 4, 5), ParseUtils.Parse<TimeSpan>("1.02:03:04.0050000"));
        }

        [Test]
        public static void Parse_Guid()
        {
            Assert.AreEqual(new Guid("2FD3E8A1-C209-4FB4-9243-DB63D6B1A0AD"), ParseUtils.Parse<Guid>("2FD3E8A1-C209-4FB4-9243-DB63D6B1A0AD"));
            Assert.AreEqual(new Guid("2FD3E8A1-C209-4FB4-9243-DB63D6B1A0AD"), ParseUtils.Parse<Guid?>("2FD3E8A1-C209-4FB4-9243-DB63D6B1A0AD"));
        }

        [TestCase("1", 1)]
        [TestCase("-1", -1)]
        [TestCase("0", 0)]
        [TestCase("invalid", null)]
        [TestCase("1.0", null)]
        [TestCase("2.2", null)]
        [TestCase("0x1", null)]
        [TestCase("2.222E-02", null)]
        [TestCase("2,000", null)]
        [TestCase("2.000", null)]
        public static void TryParseNullableIntTypes(string s, int? expected)
        {
            Assert.AreEqual(expected, ParseUtils.TryParseNullableInt(s));
            Assert.AreEqual(expected, ParseUtils.TryParseNullableLong(s));
            Assert.AreEqual(expected, ParseUtils.TryParseNullableShort(s));
            Assert.AreEqual(expected, ParseUtils.TryParseNullableSignedByte(s));
        }

        [TestCase("1", 1)]
        [TestCase("-1", null)]
        [TestCase("0", 0)]
        [TestCase("invalid", null)]
        [TestCase("1.0", null)]
        [TestCase("2.2", null)]
        [TestCase("0x1", null)]
        [TestCase("2.222E-02", null)]
        [TestCase("2,000", null)]
        [TestCase("2.000", null)]
        public static void TryParseNullableUnsignedIntTypes(string s, int? expected)
        {
            Assert.AreEqual(expected, ParseUtils.TryParseNullableByte(s));
            Assert.AreEqual(expected, ParseUtils.TryParseNullableUnsignedInt(s));
            Assert.AreEqual(expected, ParseUtils.TryParseNullableUnsignedLong(s));
            Assert.AreEqual(expected, ParseUtils.TryParseNullableUnsignedShort(s));
        }

        [TestCase("1", 1d)]
        [TestCase("-1", -1d)]
        [TestCase("0", 0d)]
        [TestCase("invalid", null)]
        [TestCase("1.0", 1.0d)]
        [TestCase("2.2", 2.2d)]
        [TestCase("0x1", null)]
        [TestCase("2.222E-02", null)]
        [TestCase("2,000", 2000d)]
        [TestCase("2.000", 2d)]
        public static void TryParseNullableDoubleTypes(string s, double? expectedDouble)
        {
            Assert.AreEqual(expectedDouble, ParseUtils.TryParseNullableDouble(s));

            var expectedFloat = (float?)expectedDouble;
            Assert.AreEqual(expectedFloat, ParseUtils.TryParseNullableFloat(s));

            var expectedDecimal = expectedDouble.HasValue ? Convert.ChangeType(expectedDouble.Value, typeof(decimal)) : default(decimal?);
            Assert.AreEqual(expectedDecimal, ParseUtils.TryParseNullableDecimal(s));
        }
    }
}
