using NUnit.Framework;
using System.Collections.Generic;

namespace PeriodParser
{
    [TestFixture]
    public class YearToDateParserTest
    {
        private YearToDateParser parser;
        Dictionary<string, object> parserResult;
        const int CurrentYear = 2021;
        const int CurrentMonth = 8;

        [SetUp]
        public void SetUp()
        {
            parser = new YearToDateParser();
        }

        [Test]
        public void YTD_LastDefitinion_Parser()
        {
            var text = "Last 2 years YTD";
            parser.Parse(text);
            parserResult = parser.Result;

            AssertDictionaryValue("Period", "Yearly");
            AssertDictionaryValue("YearlyType", "YTD");
            AssertDictionaryValue("YearlyPeriod", "Calendar");
            AssertDictionaryValue("EndingMonth", CurrentMonth);
            AssertDictionaryValue("BeginYear", CurrentYear-2);
            AssertDictionaryValue("EndingYear", CurrentYear);
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
            parser.Parse(text);
            parserResult = parser.Result;

            AssertDictionaryValue("Period", "Yearly");
            AssertDictionaryValue("YearlyType", "YTD");
            //AssertDictionaryValue("YearlyPeriod", "Calendar");
            AssertDictionaryValue("EndingMonth", 4);
            AssertDictionaryValue("BeginYear", 2018);
            AssertDictionaryValue("EndingYear", 2020);
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
            parser.Parse(text);
            parserResult = parser.Result;

            AssertDictionaryValue("Period", "Yearly");
            AssertDictionaryValue("YearlyType", "YTD");
            //AssertDictionaryValue("YearlyPeriod", "Calendar");
            AssertDictionaryValue("EndingMonth", CurrentMonth);
            AssertDictionaryValue("BeginYear", 2018);
            AssertDictionaryValue("EndingYear", 2020);
        }

        void AssertDictionaryValue(string key, object value)
        {
            Assert.IsTrue(parserResult.ContainsKey(key) && parserResult[key].ToString() == value.ToString());
        }
    }
}
