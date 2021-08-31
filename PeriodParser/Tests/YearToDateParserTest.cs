using NUnit.Framework;
using PeriodParser.RegexParser;
using System.Collections.Generic;

namespace PeriodParser.Tests
{
    [TestFixture]
    public class YearToDateParserTest
    {
        private YearlyParserRegex parser;
        Dictionary<string, object> parserResult;
        const int CurrentYear = 2020;
        const int CurrentMonth = 5;

        [SetUp]
        public void SetUp()
        {
            parser = YearlyParserRegex.GetInstance("YTD");
        }

        [Test]
        public void YTD_LastDefitinion_Parser()
        {
            parser.PeriodText = "Last 2 years YTD";
            parser.Parse();
            parserResult = parser.Result;

            AssertDictionaryValue("Period", ProfitAndLossPeriod.Yearly);
            AssertDictionaryValue("Type", "YTD");
            AssertDictionaryValue("Year1", CurrentYear-2);
            AssertDictionaryValue("Year2", CurrentYear);
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
            parser.Parse();
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
            parser.Parse();
            parserResult = parser.Result;

            AssertDictionaryValue("Period", ProfitAndLossPeriod.Yearly);
            AssertDictionaryValue("Type", "YTD");
            AssertDictionaryValue("Year1", 2018);
            AssertDictionaryValue("Year2", 2020);
        }

        void AssertDictionaryValue(string key, object value)
        {
            Assert.IsTrue(parserResult.ContainsKey(key) && parserResult[key].ToString() == value.ToString());
        }
    }
}
