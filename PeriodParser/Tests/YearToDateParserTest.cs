using NUnit.Framework;
using PeriodParser.RegexParser;
using System.Collections.Generic;

namespace PeriodParser.Tests
{
    [TestFixture]
    public class YearToDateParserTest
    {
        private YearToDateParser parser;
        Dictionary<string, object> parserResult;
        int currentYear;
        int endingMonth;

        [SetUp]
        public void SetUp()
        {
            currentYear = 2020;
            endingMonth = 10;
            parser = YearToDateParser.GetInstance();
            parser.CurrentYear = currentYear;
            parser.EndingMonth = endingMonth;
        }

        [Test]
        public void YTD_LastDefitinion_Parser()
        {
            parser.PeriodText = "Last 2 years YTD";
            parser.TryParse();
            parserResult = parser.Result;

            AssertDictionaryValue("Period", ProfitAndLossPeriod.Yearly);
            AssertDictionaryValue("Type", "YTD");
            AssertDictionaryValue("Year1", currentYear - 2);
            AssertDictionaryValue("Year2", currentYear);
        }

        [TestCase("April 2018 - 2020 YTD")]
        [TestCase("Apr 18 - 20 YTD")]
        [TestCase("04 2018-2020 ytd")]
        [TestCase("4 18-20 YTD")]
        [TestCase("April,2018 - 2020 YTD")]
        [TestCase("Apr.18 - 20 YTD")]
        [TestCase("04/2018 - 2020 YTD")]
        [TestCase("4.18-20 YTD")]
        public void YTD_WithFullDateRange_Parser(string text)
        {
            parser.PeriodText = text;
            parser.TryParse();
            parserResult = parser.Result;

            AssertDictionaryValue("Period", ProfitAndLossPeriod.Yearly);
            AssertDictionaryValue("Type", "YTD");
            AssertDictionaryValue("Month1", 4);
            AssertDictionaryValue("Year1", 2018);
            AssertDictionaryValue("Year2", 2020);
        }

        [TestCase("2018 - 2020 YTD")]
        [TestCase(" 18 - 20 YTD")]
        [TestCase(" 2018-2020 ytd")]
        [TestCase("18-20 YTD")]
        [TestCase("2018 - 2020 YTD")]
        [TestCase("18 - 20 YTD")]
        [TestCase("2018 - 2020 YTD")]
        [TestCase("2018-20 YTD")]
        public void YTD_WithlDateRangeWithoutMonth_Parser(string text)
        {
            parser.PeriodText = text;
            parser.TryParse();
            parserResult = parser.Result;

            AssertDictionaryValue("Period", ProfitAndLossPeriod.Yearly);
            AssertDictionaryValue("Type", "YTD");
            AssertDictionaryValue("Year1", 2018);
            AssertDictionaryValue("Year2", 2020);
        }

        [TestCase("YTD")]
        [TestCase(" ytd")]
        public void YTD_OnlyText_Parser(string text)
        {
            parser.PeriodText = text;
            parser.TryParse();
            parserResult = parser.Result;

            AssertDictionaryValue("Period", ProfitAndLossPeriod.Yearly);
            AssertDictionaryValue("Type", "YTD");
            AssertDictionaryValue("Year1", currentYear);
            AssertDictionaryValue("Month1", endingMonth);
        }

        [TestCase("september YTD")]
        public void YTD_OnlyMonthName_Parser(string text)
        {
            parser.PeriodText = text;
            parser.TryParse();
            parserResult = parser.Result;

            AssertDictionaryValue("Period", ProfitAndLossPeriod.Yearly);
            AssertDictionaryValue("Type", "YTD");
            AssertDictionaryValue("Year1", currentYear);
            AssertDictionaryValue("Month1", 9);
        }

        [TestCase("2011 YTD")]
        [TestCase("11 YTD")]
        public void YTD_OnlyYear_Parser(string text)
        {
            parser.PeriodText = text;
            parser.TryParse();
            parserResult = parser.Result;

            AssertDictionaryValue("Period", ProfitAndLossPeriod.Yearly);
            AssertDictionaryValue("Type", "YTD");
            AssertDictionaryValue("Year1", 2011);
        }

        void AssertDictionaryValue(string key, object value)
        {
            Assert.IsTrue(parserResult.ContainsKey(key), $"There isn't '{key}' key");
            Assert.AreEqual(value.ToString(), parserResult[key].ToString());
        }
    }
}
