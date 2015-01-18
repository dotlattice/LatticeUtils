using LatticeUtils.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LatticeUtils.Core.UnitTests
{
    public  class TestParseUtils
    {
        private CultureInfo enUsCulture;
        private CultureInfo enGbCulture;
        private CultureInfo frFrCulture;

        [SetUp]
        public void SetUp()
        {
            enUsCulture = CultureInfo.GetCultureInfo("en-US");
            enGbCulture = CultureInfo.GetCultureInfo("en-GB");
            frFrCulture = CultureInfo.GetCultureInfo("fr-FR");
        }

        [TestCase("1", typeof(int), 1)]
        [TestCase("1", typeof(int?), 1)]
        [TestCase("1", typeof(double), 1d)]
        [TestCase("1", typeof(double?), 1d)]
        [TestCase("1", typeof(float), 1d)]
        [TestCase("1", typeof(float?), 1d)]
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
        public void ParseInvariant(string s, Type type, object expected)
        {
            Assert.AreEqual(expected, ParseUtils.ParseInvariant(s, type));
            Assert.AreEqual(expected, RunForCulture(enUsCulture, () => ParseUtils.Parse(s, type)));
            Assert.AreEqual(expected, RunForCulture(enGbCulture, () => ParseUtils.Parse(s, type)));
            Assert.AreEqual(expected, RunForCulture(frFrCulture, () => ParseUtils.Parse(s, type)));
        }

        [TestCase("1.0", 1d)]
        [TestCase("1.000", 1d)]
        [TestCase("123.45", 123.45d)]
        public void Parse_NumbersAmerica(string s, double expected)
        {
            Assert.AreEqual(expected, RunForCulture(enUsCulture, () => ParseUtils.Parse(s, typeof(double))));
            Assert.AreEqual((float)expected, RunForCulture(enUsCulture, () => ParseUtils.Parse(s, typeof(float))));
            Assert.AreEqual((decimal)expected, RunForCulture(enUsCulture, () => ParseUtils.Parse(s, typeof(decimal))));
        }

        [TestCase("1,0", 1d)]
        [TestCase("1,000", 1d)]
        [TestCase("123,45", 123.45d)]
        public void Parse_NumbersFrance(string s, double expected)
        {
            Assert.AreEqual(expected, RunForCulture(frFrCulture, () => ParseUtils.Parse(s, typeof(double))));
            Assert.AreEqual((float)expected, RunForCulture(frFrCulture, () => ParseUtils.Parse(s, typeof(float))));
            Assert.AreEqual((decimal)expected, RunForCulture(frFrCulture, () => ParseUtils.Parse(s, typeof(decimal))));
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
        public void ParseInvariant_Failures(string s, Type type)
        {
            var expectedException = Assert.Throws<FormatException>(() => ParseUtils.ParseInvariant(s, type));
            if (!string.IsNullOrEmpty(s))
            {
                StringAssert.Contains(s, expectedException.Message);
            }

            Assert.Throws<FormatException>(() => RunForCulture(enUsCulture, () => ParseUtils.Parse(s, type)));
            Assert.Throws<FormatException>(() => RunForCulture(enGbCulture, () => ParseUtils.Parse(s, type)));
            Assert.Throws<FormatException>(() => RunForCulture(frFrCulture, () => ParseUtils.Parse(s, type)));
        }

        [TestCase("1,0")]
        [TestCase("1,000")]
        [TestCase("123,45")]
        public void Parse_Failures_NumbersAmerica(string s)
        {
            Assert.Throws<FormatException>(() => RunForCulture(enUsCulture, () => ParseUtils.Parse(s, typeof(double))));
            Assert.Throws<FormatException>(() => RunForCulture(enUsCulture, () => ParseUtils.Parse(s, typeof(float))));
            Assert.Throws<FormatException>(() => RunForCulture(enUsCulture, () => ParseUtils.Parse(s, typeof(decimal))));
        }

        [TestCase("1.0")]
        [TestCase("1.000")]
        [TestCase("123.45")]
        public void Parse_Failures_NumbersFrance(string s)
        {
            Assert.Throws<FormatException>(() => RunForCulture(frFrCulture, () => ParseUtils.Parse(s, typeof(double))));
            Assert.Throws<FormatException>(() => RunForCulture(frFrCulture, () => ParseUtils.Parse(s, typeof(float))));
            Assert.Throws<FormatException>(() => RunForCulture(frFrCulture, () => ParseUtils.Parse(s, typeof(decimal))));
        }

        [TestCase("1999-12-31")]
        [TestCase("Friday, 31 December 1999")]
        [TestCase("1999-12-31 00:00:00")]
        [TestCase("1999-12-31T00:00:00")]
        [TestCase("Friday, 31 December 1999 00:00:00")]
        public void ParseInvariant_Date_19991231(string s)
        {
            var expected = new DateTime(1999, 12, 31);
            var expectedWithOffset = new DateTimeOffset(expected, TimeZoneInfo.Local.GetUtcOffset(expected));

            Assert.AreEqual(expected, ParseUtils.ParseInvariant<DateTime>(s));
            Assert.AreEqual(expected, ParseUtils.ParseInvariant<DateTime?>(s));
            Assert.AreEqual(expectedWithOffset, ParseUtils.ParseInvariant<DateTimeOffset>(s));
            Assert.AreEqual(expectedWithOffset, ParseUtils.ParseInvariant<DateTimeOffset?>(s));

            Assert.AreEqual(expected, RunForCulture(enUsCulture, () =>ParseUtils.Parse<DateTime>(s)));
            Assert.AreEqual(expectedWithOffset, RunForCulture(enUsCulture, () => ParseUtils.Parse<DateTimeOffset>(s)));

            Assert.AreEqual(expected, RunForCulture(enGbCulture, () => ParseUtils.Parse<DateTime>(s)));
            Assert.AreEqual(expectedWithOffset, RunForCulture(enGbCulture, () => ParseUtils.Parse<DateTimeOffset>(s)));

            Assert.AreEqual(expected, RunForCulture(frFrCulture, () => ParseUtils.Parse<DateTime>(s)));
            Assert.AreEqual(expectedWithOffset, RunForCulture(frFrCulture, () => ParseUtils.Parse<DateTimeOffset>(s)));
        }

        [TestCase("12/31/1999")]
        [TestCase("12/31/1999 00:00:00")]
        public void Parse_Date_MonthBeforeDay_19991231(string s)
        {
            var expected = new DateTime(1999, 12, 31);
            var expectedWithOffset = new DateTimeOffset(expected, TimeZoneInfo.Local.GetUtcOffset(expected));

            Assert.AreEqual(expected, ParseUtils.ParseInvariant<DateTime>(s));
            Assert.AreEqual(expectedWithOffset, ParseUtils.ParseInvariant<DateTimeOffset>(s));

            Assert.AreEqual(expected, RunForCulture(enUsCulture, () => ParseUtils.Parse<DateTime>(s)));
            Assert.AreEqual(expectedWithOffset, RunForCulture(enUsCulture, () => ParseUtils.Parse<DateTimeOffset>(s)));

            Assert.Throws<FormatException>(() => RunForCulture(enGbCulture, () => ParseUtils.Parse<DateTime>(s)));
            Assert.Throws<FormatException>(() => RunForCulture(enGbCulture, () => ParseUtils.Parse<DateTimeOffset>(s)));

            Assert.Throws<FormatException>(() => RunForCulture(frFrCulture, () => ParseUtils.Parse<DateTime>(s)));
            Assert.Throws<FormatException>(() => RunForCulture(frFrCulture, () => ParseUtils.Parse<DateTimeOffset>(s)));
        }

        [TestCase("31/12/1999")]
        [TestCase("31/12/1999 00:00:00")]
        public void Parse_Date_DayBeforeMonth_19991231(string s)
        {
            var expected = new DateTime(1999, 12, 31);
            var expectedWithOffset = new DateTimeOffset(expected, TimeZoneInfo.Local.GetUtcOffset(expected));

            Assert.AreEqual(expected, RunForCulture(enGbCulture, () => ParseUtils.Parse<DateTime>(s)));
            Assert.AreEqual(expectedWithOffset, RunForCulture(enGbCulture, () => ParseUtils.Parse<DateTimeOffset>(s)));

            Assert.AreEqual(expected, RunForCulture(frFrCulture, () => ParseUtils.Parse<DateTime>(s)));
            Assert.AreEqual(expectedWithOffset, RunForCulture(frFrCulture, () => ParseUtils.Parse<DateTimeOffset>(s)));

            Assert.Throws<FormatException>(() => ParseUtils.ParseInvariant<DateTime>(s));
            Assert.Throws<FormatException>(() => ParseUtils.ParseInvariant<DateTimeOffset>(s));

            Assert.Throws<FormatException>(() => RunForCulture(enUsCulture, () => ParseUtils.Parse<DateTime>(s)));
            Assert.Throws<FormatException>(() => RunForCulture(enUsCulture, () => ParseUtils.Parse<DateTimeOffset>(s)));
        }

        [TestCase("1999-12-31 01:02:03")]
        [TestCase("1999-12-31 01:02:03 AM")]
        [TestCase("1999-12-31T01:02:03")]
        [TestCase("Friday, 31 December 1999 01:02:03")]
        [TestCase("Friday, 31 December 1999 01:02:03 AM")]
        public void ParseInvariant_Date_19991231T010203(string s)
        {
            var expected = new DateTime(1999, 12, 31, 1, 2, 3);
            var expectedWithOffset = new DateTimeOffset(expected, TimeZoneInfo.Local.GetUtcOffset(expected));

            Assert.AreEqual(expected, ParseUtils.ParseInvariant<DateTime>(s));
            Assert.AreEqual(expected, ParseUtils.ParseInvariant<DateTime?>(s));
            Assert.AreEqual(expectedWithOffset, ParseUtils.ParseInvariant<DateTimeOffset>(s));
            Assert.AreEqual(expectedWithOffset, ParseUtils.ParseInvariant<DateTimeOffset?>(s));

            Assert.AreEqual(expected, RunForCulture(enUsCulture, () => ParseUtils.Parse<DateTime>(s)));
            Assert.AreEqual(expectedWithOffset, RunForCulture(enUsCulture, () => ParseUtils.Parse<DateTimeOffset>(s)));

            Assert.AreEqual(expected, RunForCulture(enGbCulture, () => ParseUtils.Parse<DateTime>(s)));
            Assert.AreEqual(expectedWithOffset, RunForCulture(enGbCulture, () => ParseUtils.Parse<DateTimeOffset>(s)));

            Assert.AreEqual(expected, RunForCulture(frFrCulture, () => ParseUtils.Parse<DateTime>(s)));
            Assert.AreEqual(expectedWithOffset, RunForCulture(frFrCulture, () => ParseUtils.Parse<DateTimeOffset>(s)));
        }

        [TestCase("1999-12-31 13:02:03")]
        [TestCase("1999-12-31 01:02:03 PM")]
        [TestCase("1999-12-31T13:02:03")]
        [TestCase("Friday, 31 December 1999 13:02:03")]
        [TestCase("Friday, 31 December 1999 01:02:03 PM")]
        public void ParseInvariant_Date_19991231T130203(string s)
        {
            var expected = new DateTime(1999, 12, 31, 13, 2, 3);
            var expectedWithOffset = new DateTimeOffset(expected, TimeZoneInfo.Local.GetUtcOffset(expected));

            Assert.AreEqual(expected, ParseUtils.ParseInvariant<DateTime>(s));
            Assert.AreEqual(expected, ParseUtils.ParseInvariant<DateTime?>(s));
            Assert.AreEqual(expectedWithOffset, ParseUtils.ParseInvariant<DateTimeOffset>(s));
            Assert.AreEqual(expectedWithOffset, ParseUtils.ParseInvariant<DateTimeOffset?>(s));

            Assert.AreEqual(expected, RunForCulture(enUsCulture, () => ParseUtils.Parse<DateTime>(s)));
            Assert.AreEqual(expectedWithOffset, RunForCulture(enUsCulture, () => ParseUtils.Parse<DateTimeOffset>(s)));

            Assert.AreEqual(expected, RunForCulture(enGbCulture, () => ParseUtils.Parse<DateTime>(s)));
            Assert.AreEqual(expectedWithOffset, RunForCulture(enGbCulture, () => ParseUtils.Parse<DateTimeOffset>(s)));

            Assert.AreEqual(expected, RunForCulture(frFrCulture, () => ParseUtils.Parse<DateTime>(s)));
            Assert.AreEqual(expectedWithOffset, RunForCulture(frFrCulture, () => ParseUtils.Parse<DateTimeOffset>(s)));
        }

        [TestCase("1999-12-31 01:02:03 +00:00")]
        [TestCase("1999-12-31 01:02:03Z")]
        public void ParseInvariant_DateTimeOffset_19993112010203Z(string s)
        {
            var expectedWithOffset = new DateTimeOffset(1999, 12, 31, 1, 2, 3, TimeSpan.Zero);

            Assert.AreEqual(expectedWithOffset, ParseUtils.ParseInvariant<DateTimeOffset>(s));
            Assert.AreEqual(expectedWithOffset, ParseUtils.ParseInvariant<DateTimeOffset?>(s));

            Assert.AreEqual(expectedWithOffset, RunForCulture(enUsCulture, () => ParseUtils.Parse<DateTimeOffset>(s)));
            Assert.AreEqual(expectedWithOffset, RunForCulture(enGbCulture, () => ParseUtils.Parse<DateTimeOffset>(s)));
            Assert.AreEqual(expectedWithOffset, RunForCulture(frFrCulture, () => ParseUtils.Parse<DateTimeOffset>(s)));
        }

        [Test]
        public void ParseInvariant_TimeSpan()
        {
            Assert.AreEqual(new TimeSpan(1, 2, 3), ParseUtils.ParseInvariant<TimeSpan>("01:02:03"));
            Assert.AreEqual(new TimeSpan(1, 2, 3), ParseUtils.ParseInvariant<TimeSpan?>("01:02:03"));
            Assert.AreEqual(TimeSpan.FromHours(8), ParseUtils.ParseInvariant<TimeSpan>("08:00"));
            Assert.AreEqual(new TimeSpan(1, 2, 3, 4, 5), ParseUtils.ParseInvariant<TimeSpan>("1.02:03:04.0050000"));
        }

        [Test]
        public void Parse_Guid()
        {
            Assert.AreEqual(new Guid("2FD3E8A1-C209-4FB4-9243-DB63D6B1A0AD"), ParseUtils.ParseInvariant<Guid>("2FD3E8A1-C209-4FB4-9243-DB63D6B1A0AD"));
            Assert.AreEqual(new Guid("2FD3E8A1-C209-4FB4-9243-DB63D6B1A0AD"), ParseUtils.ParseInvariant<Guid?>("2FD3E8A1-C209-4FB4-9243-DB63D6B1A0AD"));
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
        public void TryParseNullableIntTypes(string s, int? expected)
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
        public void TryParseNullableUnsignedIntTypes(string s, int? expected)
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
        public void TryParseNullableDoubleTypes(string s, double? expectedDouble)
        {
            Assert.AreEqual(expectedDouble, ParseUtils.TryParseNullableDoubleInvariant(s));

            var expectedFloat = (float?)expectedDouble;
            Assert.AreEqual(expectedFloat, ParseUtils.TryParseNullableFloatInvariant(s));

            var expectedDecimal = expectedDouble.HasValue ? Convert.ChangeType(expectedDouble.Value, typeof(decimal)) : default(decimal?);
            Assert.AreEqual(expectedDecimal, ParseUtils.TryParseNullableDecimalInvariant(s));
        }

        #region Dates/Times

        [TestCase("1999-12-31")]
        [TestCase("Friday, 31 December 1999")]
        [TestCase("1999-12-31 00:00:00")]
        [TestCase("1999-12-31T00:00:00")]
        [TestCase("Friday, 31 December 1999 00:00:00")]
        public void TryParseNullableDateTypes_Invariant_19991231(string s)
        {
            var expected = new DateTime(1999, 12, 31);
            var expectedWithUtcOffset = new DateTimeOffset(expected, TimeSpan.Zero);
            var expectedWithLocalOffset = new DateTimeOffset(expected, TimeZoneInfo.Local.GetUtcOffset(expected));

            Assert.AreEqual(expected, ParseUtils.TryParseNullableDateInvariant(s));
            Assert.AreEqual(DateTimeKind.Unspecified, ParseUtils.TryParseNullableDateInvariant(s).Value.Kind);
            Assert.AreEqual(expected, RunForCulture(enUsCulture, () => ParseUtils.TryParseNullableDate(s)));
            Assert.AreEqual(expected, RunForCulture(enGbCulture, () => ParseUtils.TryParseNullableDate(s)));
            Assert.AreEqual(expected, RunForCulture(frFrCulture, () => ParseUtils.TryParseNullableDate(s)));

            Assert.AreEqual(expected, ParseUtils.TryParseNullableDateTimeUtcInvariant(s));
            Assert.AreEqual(DateTimeKind.Utc, ParseUtils.TryParseNullableDateTimeUtcInvariant(s).Value.Kind);

            Assert.AreEqual(expected, RunForCulture(enUsCulture, () => ParseUtils.TryParseNullableDateTime(s)));
            Assert.AreEqual(expected, RunForCulture(enGbCulture, () => ParseUtils.TryParseNullableDateTime(s)));
            Assert.AreEqual(expected, RunForCulture(frFrCulture, () => ParseUtils.TryParseNullableDateTime(s)));

            Assert.AreEqual(expectedWithLocalOffset, RunForCulture(enUsCulture, () => ParseUtils.TryParseNullableDateTimeOffset(s)));
            Assert.AreEqual(expectedWithLocalOffset, RunForCulture(enGbCulture, () => ParseUtils.TryParseNullableDateTimeOffset(s)));
            Assert.AreEqual(expectedWithLocalOffset, RunForCulture(frFrCulture, () => ParseUtils.TryParseNullableDateTimeOffset(s)));

            Assert.AreEqual(expectedWithUtcOffset, ParseUtils.TryParseNullableDateTimeOffsetAssumeUtcInvariant(s));
        }

        [TestCase("12/31/1999")]
        [TestCase("12/31/1999 00:00:00")]
        public void TryParseNullableDateTypes_MonthBeforeDay_19991231(string s)
        {
            var expected = new DateTime(1999, 12, 31);
            var expectedWithUtcOffset = new DateTimeOffset(expected, TimeSpan.Zero);
            var expectedWithLocalOffset = new DateTimeOffset(expected, TimeZoneInfo.Local.GetUtcOffset(expected));

            Assert.AreEqual(expected, ParseUtils.TryParseNullableDateInvariant(s));
            Assert.AreEqual(expected, RunForCulture(enUsCulture, () => ParseUtils.TryParseNullableDate(s)));
            Assert.IsNull(RunForCulture(enGbCulture, () => ParseUtils.TryParseNullableDate(s)));
            Assert.IsNull(RunForCulture(frFrCulture, () => ParseUtils.TryParseNullableDate(s)));

            Assert.AreEqual(expected, ParseUtils.TryParseNullableDateTimeUtcInvariant(s));
            Assert.AreEqual(expected, RunForCulture(enUsCulture, () => ParseUtils.TryParseNullableDateTime(s)));
            Assert.IsNull(RunForCulture(enGbCulture, () => ParseUtils.TryParseNullableDateTime(s)));
            Assert.IsNull(RunForCulture(frFrCulture, () => ParseUtils.TryParseNullableDateTime(s)));

            Assert.AreEqual(expectedWithLocalOffset, RunForCulture(enUsCulture, () => ParseUtils.TryParseNullableDateTimeOffset(s)));
            Assert.IsNull(RunForCulture(enGbCulture, () => ParseUtils.TryParseNullableDateTimeOffset(s)));
            Assert.IsNull(RunForCulture(frFrCulture, () => ParseUtils.TryParseNullableDateTimeOffset(s)));

            Assert.AreEqual(expectedWithUtcOffset, ParseUtils.TryParseNullableDateTimeOffsetAssumeUtcInvariant(s));
        }

        [TestCase("31/12/1999")]
        [TestCase("31/12/1999 00:00:00")]
        public void TryParseNullableDateTypes_DayBeforeMonth_19991231(string s)
        {
            var expected = new DateTime(1999, 12, 31);
            var expectedWithUtcOffset = new DateTimeOffset(expected, TimeSpan.Zero);
            var expectedWithLocalOffset = new DateTimeOffset(expected, TimeZoneInfo.Local.GetUtcOffset(expected));

            Assert.IsNull(ParseUtils.TryParseNullableDateInvariant(s));
            Assert.IsNull(RunForCulture(enUsCulture, () => ParseUtils.TryParseNullableDate(s)));
            Assert.AreEqual(expected, RunForCulture(enGbCulture, () => ParseUtils.TryParseNullableDate(s)));
            Assert.AreEqual(expected, RunForCulture(frFrCulture, () => ParseUtils.TryParseNullableDate(s)));

            Assert.IsNull(ParseUtils.TryParseNullableDateTimeUtcInvariant(s));
            Assert.IsNull(RunForCulture(enUsCulture, () => ParseUtils.TryParseNullableDateTime(s)));
            Assert.AreEqual(expected, RunForCulture(enGbCulture, () => ParseUtils.TryParseNullableDateTime(s)));
            Assert.AreEqual(expected, RunForCulture(frFrCulture, () => ParseUtils.TryParseNullableDateTime(s)));

            Assert.IsNull(RunForCulture(enUsCulture, () => ParseUtils.TryParseNullableDateTimeOffset(s)));
            Assert.AreEqual(expectedWithLocalOffset, RunForCulture(enGbCulture, () => ParseUtils.TryParseNullableDateTimeOffset(s)));
            Assert.AreEqual(expectedWithLocalOffset, RunForCulture(frFrCulture, () => ParseUtils.TryParseNullableDateTimeOffset(s)));

            Assert.IsNull(ParseUtils.TryParseNullableDateTimeOffsetAssumeUtcInvariant(s));
        }

        [TestCase("1999-12-31 01:02:03Z")]
        [TestCase("1999-12-31 01:02:03 +00:00")]
        [TestCase("1999-12-31 23:02:03 +00:00")]
        [TestCase("1999-12-31 01:02:03 -06:00")]
        [TestCase("1999-12-31 23:02:03 -06:00")]
        [TestCase("1999-12-31 01:02:03 +06:00")]
        [TestCase("1999-12-31 23:02:03 +06:00")]
        [TestCase("1999-12-31 01:02:03 -12:00")]
        [TestCase("1999-12-31 23:02:03 -12:00")]
        [TestCase("1999-12-31 01:02:03 +12:00")]
        [TestCase("1999-12-31 23:02:03 +12:00")]
        [TestCase("1999-12-31 01:02:03 -14:00")]
        [TestCase("1999-12-31 23:02:03 -14:00")]
        [TestCase("1999-12-31 01:02:03 +14:00")]
        [TestCase("1999-12-31 23:02:03 +14:00")]
        public void TryParseNullableDateInvariant_OffsetDoesNotChangeDate(string s)
        {
            var expected = new DateTime(1999, 12, 31);
            Assert.AreEqual(expected, ParseUtils.TryParseNullableDateInvariant(s));
            Assert.AreEqual(DateTimeKind.Unspecified, ParseUtils.TryParseNullableDateInvariant(s).Value.Kind);
            Assert.AreEqual(expected, RunForCulture(enUsCulture, () => ParseUtils.TryParseNullableDate(s)));
            Assert.AreEqual(expected, RunForCulture(enGbCulture, () => ParseUtils.TryParseNullableDate(s)));
            Assert.AreEqual(expected, RunForCulture(frFrCulture, () => ParseUtils.TryParseNullableDate(s)));
        }

        [TestCase("1999-12-31 01:02:03")]
        [TestCase("1999-12-31 01:02:03 AM")]
        [TestCase("1999-12-31T01:02:03")]
        [TestCase("Friday, 31 December 1999 01:02:03")]
        [TestCase("Friday, 31 December 1999 01:02:03 AM")]
        public void TryParseNullableDateTimeTypes_19991231T010203(string s)
        {
            var expectedDate = new DateTime(1999, 12, 31);
            var expected = new DateTime(1999, 12, 31, 1, 2, 3);
            var expectedWithUtcOffset = new DateTimeOffset(expected, TimeSpan.Zero);
            var expectedWithLocalOffset = new DateTimeOffset(expected, TimeZoneInfo.Local.GetUtcOffset(expected));

            Assert.AreEqual(expectedDate, ParseUtils.TryParseNullableDateInvariant(s));
            Assert.AreEqual(DateTimeKind.Unspecified, ParseUtils.TryParseNullableDateInvariant(s).Value.Kind);
            Assert.AreEqual(expectedDate, RunForCulture(enUsCulture, () => ParseUtils.TryParseNullableDate(s)));
            Assert.AreEqual(expectedDate, RunForCulture(enGbCulture, () => ParseUtils.TryParseNullableDate(s)));
            Assert.AreEqual(expectedDate, RunForCulture(frFrCulture, () => ParseUtils.TryParseNullableDate(s)));

            Assert.AreEqual(expected, ParseUtils.TryParseNullableDateTimeUtcInvariant(s));
            Assert.AreEqual(DateTimeKind.Utc, ParseUtils.TryParseNullableDateTimeUtcInvariant(s).Value.Kind);

            Assert.AreEqual(expected, RunForCulture(enUsCulture, () => ParseUtils.TryParseNullableDateTime(s)));
            Assert.AreEqual(expected, RunForCulture(enGbCulture, () => ParseUtils.TryParseNullableDateTime(s)));
            Assert.AreEqual(expected, RunForCulture(frFrCulture, () => ParseUtils.TryParseNullableDateTime(s)));

            Assert.AreEqual(expectedWithLocalOffset, RunForCulture(enUsCulture, () => ParseUtils.TryParseNullableDateTimeOffset(s)));
            Assert.AreEqual(expectedWithLocalOffset, RunForCulture(enGbCulture, () => ParseUtils.TryParseNullableDateTimeOffset(s)));
            Assert.AreEqual(expectedWithLocalOffset, RunForCulture(frFrCulture, () => ParseUtils.TryParseNullableDateTimeOffset(s)));

            Assert.AreEqual(expectedWithUtcOffset, ParseUtils.TryParseNullableDateTimeOffsetAssumeUtcInvariant(s));
        }

        [TestCase("1999-12-31 13:02:03")]
        [TestCase("1999-12-31 01:02:03 PM")]
        [TestCase("1999-12-31T13:02:03")]
        [TestCase("Friday, 31 December 1999 13:02:03")]
        [TestCase("Friday, 31 December 1999 01:02:03 PM")]
        public void TryParseNullableDateTimeTypes_19991231T130203(string s)
        {
            var expectedDate = new DateTime(1999, 12, 31);
            var expected = new DateTime(1999, 12, 31, 13, 2, 3);
            var expectedWithUtcOffset = new DateTimeOffset(expected, TimeSpan.Zero);
            var expectedWithLocalOffset = new DateTimeOffset(expected, TimeZoneInfo.Local.GetUtcOffset(expected));

            Assert.AreEqual(expectedDate, ParseUtils.TryParseNullableDateInvariant(s));
            Assert.AreEqual(DateTimeKind.Unspecified, ParseUtils.TryParseNullableDateInvariant(s).Value.Kind);
            Assert.AreEqual(expectedDate, RunForCulture(enUsCulture, () => ParseUtils.TryParseNullableDate(s)));
            Assert.AreEqual(expectedDate, RunForCulture(enGbCulture, () => ParseUtils.TryParseNullableDate(s)));
            Assert.AreEqual(expectedDate, RunForCulture(frFrCulture, () => ParseUtils.TryParseNullableDate(s)));

            Assert.AreEqual(expected, ParseUtils.TryParseNullableDateTimeUtcInvariant(s));
            Assert.AreEqual(DateTimeKind.Utc, ParseUtils.TryParseNullableDateTimeUtcInvariant(s).Value.Kind);

            Assert.AreEqual(expected, RunForCulture(enUsCulture, () => ParseUtils.TryParseNullableDateTime(s)));
            Assert.AreEqual(expected, RunForCulture(enGbCulture, () => ParseUtils.TryParseNullableDateTime(s)));
            Assert.AreEqual(expected, RunForCulture(frFrCulture, () => ParseUtils.TryParseNullableDateTime(s)));

            Assert.AreEqual(expectedWithLocalOffset, RunForCulture(enUsCulture, () => ParseUtils.TryParseNullableDateTimeOffset(s)));
            Assert.AreEqual(expectedWithLocalOffset, RunForCulture(enGbCulture, () => ParseUtils.TryParseNullableDateTimeOffset(s)));
            Assert.AreEqual(expectedWithLocalOffset, RunForCulture(frFrCulture, () => ParseUtils.TryParseNullableDateTimeOffset(s)));

            Assert.AreEqual(expectedWithUtcOffset, ParseUtils.TryParseNullableDateTimeOffsetAssumeUtcInvariant(s));
        }

        [TestCase("1999-12-31 01:02:03 +00:00")]
        [TestCase("1999-12-31 01:02:03Z")]
        public void TryParseNullableDateTimeOffsetTypes_19991231T010203Z(string s)
        {
            var expectedDate = new DateTime(1999, 12, 31);
            var expected = new DateTime(1999, 12, 31, 1, 2, 3);
            var expectedOffset = new DateTimeOffset(expected, TimeSpan.Zero);

            Assert.AreEqual(expectedDate, ParseUtils.TryParseNullableDateInvariant(s));
            Assert.AreEqual(DateTimeKind.Unspecified, ParseUtils.TryParseNullableDateInvariant(s).Value.Kind);
            Assert.AreEqual(expectedDate, RunForCulture(enUsCulture, () => ParseUtils.TryParseNullableDate(s)));
            Assert.AreEqual(expectedDate, RunForCulture(enGbCulture, () => ParseUtils.TryParseNullableDate(s)));
            Assert.AreEqual(expectedDate, RunForCulture(frFrCulture, () => ParseUtils.TryParseNullableDate(s)));

            Assert.AreEqual(expected, ParseUtils.TryParseNullableDateTimeUtcInvariant(s));
            Assert.AreEqual(DateTimeKind.Utc, ParseUtils.TryParseNullableDateTimeUtcInvariant(s).Value.Kind);

            Assert.AreEqual(expectedOffset, RunForCulture(enUsCulture, () => ParseUtils.TryParseNullableDateTimeOffset(s)));
            Assert.AreEqual(expectedOffset, RunForCulture(enGbCulture, () => ParseUtils.TryParseNullableDateTimeOffset(s)));
            Assert.AreEqual(expectedOffset, RunForCulture(frFrCulture, () => ParseUtils.TryParseNullableDateTimeOffset(s)));

            Assert.AreEqual(expectedOffset, ParseUtils.TryParseNullableDateTimeOffsetAssumeUtcInvariant(s));
        }

        [TestCase("1999-12-31 01:02:03 -02:00")]
        public void TryParseNullableDateTimeOffsetTypes_19991231T010203_OffsetNegative2(string s)
        {
            var expectedDate = new DateTime(1999, 12, 31);
            var expected = new DateTime(1999, 12, 31, 1, 2, 3);
            var expectedUtc = new DateTime(1999, 12, 31, 3, 2, 3);
            var expectedOffset = new DateTimeOffset(expected, TimeSpan.FromHours(-2));

            Assert.AreEqual(expectedDate, ParseUtils.TryParseNullableDateInvariant(s));
            Assert.AreEqual(DateTimeKind.Unspecified, ParseUtils.TryParseNullableDateInvariant(s).Value.Kind);
            Assert.AreEqual(expectedDate, RunForCulture(enUsCulture, () => ParseUtils.TryParseNullableDate(s)));
            Assert.AreEqual(expectedDate, RunForCulture(enGbCulture, () => ParseUtils.TryParseNullableDate(s)));
            Assert.AreEqual(expectedDate, RunForCulture(frFrCulture, () => ParseUtils.TryParseNullableDate(s)));

            Assert.AreEqual(expectedUtc, ParseUtils.TryParseNullableDateTimeUtcInvariant(s));
            Assert.AreEqual(DateTimeKind.Utc, ParseUtils.TryParseNullableDateTimeUtcInvariant(s).Value.Kind);

            Assert.AreEqual(expectedOffset, RunForCulture(enUsCulture, () => ParseUtils.TryParseNullableDateTimeOffset(s)));
            Assert.AreEqual(expectedOffset, RunForCulture(enGbCulture, () => ParseUtils.TryParseNullableDateTimeOffset(s)));
            Assert.AreEqual(expectedOffset, RunForCulture(frFrCulture, () => ParseUtils.TryParseNullableDateTimeOffset(s)));

            Assert.AreEqual(expectedOffset, ParseUtils.TryParseNullableDateTimeOffsetAssumeUtcInvariant(s));
        }

        #endregion

        #region Helpers

        private static T RunForCulture<T>(CultureInfo culture, Func<T> func)
        {
            // Run in a separate thread so we don't make any changes to the main thread.
            var task = new TaskFactory().StartNew(() =>
            {
                Thread.CurrentThread.CurrentCulture = culture;
                return func();
            });
            try
            {
                return task.Result;
            }
            catch (AggregateException ex)
            {
                throw ex.InnerException;
            }
        }

        #endregion
    }
}
