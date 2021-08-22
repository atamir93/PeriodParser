using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeriodParser
{
    [TestFixture]
    public class EntireYearParserTest
    {
        private EntireYearParser parser;
        Dictionary<string, object> parserResult;
        const int CurrentYear = 2021;
        const int CurrentMonth = 8;

        [SetUp]
        public void SetUp()
        {
            parser = new EntireYearParser();
        }

        [Test]
        public void EntireYear_LastDefitinion_Parser()
        {
            var text = "Last 2 years";
            parser.Parse(text);
            parserResult = parser.Result;

            AssertDictionaryValue("Period", "Yearly");
            AssertDictionaryValue("YearlyType", "EntireYear");
            AssertDictionaryValue("YearlyPeriod", "Calendar");
            AssertDictionaryValue("EndingMonth", CurrentMonth);
            AssertDictionaryValue("BeginYear", CurrentYear - 2);
            AssertDictionaryValue("EndingYear", CurrentYear);
        }

        [TestCase("2018 - 2020 yearly")]
        [TestCase(" 18 - 20 years")]
        [TestCase(" 2018-2020 year")]
        [TestCase("18-20 yearly")]
        [TestCase("2018 - 2020 year")]
        [TestCase("18 - 20 years")]
        [TestCase("2018 - 2020 years")]
        [TestCase("2018-20 years")]
        public void EntireYear_WithlDateRangeWithoutMonth_Parser(string text)
        {
            parser.Parse(text);
            parserResult = parser.Result;

            AssertDictionaryValue("Period", "Yearly");
            AssertDictionaryValue("YearlyType", "EntireYear");
            //AssertDictionaryValue("YearlyPeriod", "Calendar");
            AssertDictionaryValue("EndingMonth", CurrentMonth);
            AssertDictionaryValue("BeginYear", 2018);
            AssertDictionaryValue("EndingYear", 2020);
        }

        [TestCase("April 2018 - 2020 Yearly")]
        [TestCase("Apr 18 - 20 year")]
        [TestCase("04 2018-2020 years")]
        [TestCase("4 18-20 yearly")]
        [TestCase("April,2018 - 2020 years")]
        [TestCase("Apr.18 - 20 yearly")]
        [TestCase("04/2018 - 2020 yearly")]
        [TestCase("4.18-20 years")]
        public void EntireYear_WithFullDateRange_Parser(string text)
        {
            parser.Parse(text);
            parserResult = parser.Result;

            AssertDictionaryValue("Period", "Yearly");
            AssertDictionaryValue("YearlyType", "EntireYear");
            //AssertDictionaryValue("YearlyPeriod", "Calendar");
            AssertDictionaryValue("EndingMonth", 4);
            AssertDictionaryValue("BeginYear", 2018);
            AssertDictionaryValue("EndingYear", 2020);
        }

        void AssertDictionaryValue(string key, object value)
        {
            Assert.IsTrue(parserResult.ContainsKey(key) && parserResult[key].ToString() == value.ToString());
        }
    }
}
