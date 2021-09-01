using NUnit.Framework;
using PeriodParser.RegexParser;
using System.Collections.Generic;

namespace PeriodParser.Tests
{
    [TestFixture]
    public class EntireYearParserTest
    {
        private YearlyParserRegex parser;
        Dictionary<string, object> parserResult;
        const int CurrentYear = 2020;
        const int CurrentMonth = 5;

        [SetUp]
        public void SetUp()
        {
            parser = YearlyParserRegex.GetInstance("EntireYear");
        }

        [Test]
        public void EntireYear_LastDefitinion_Parser()
        {
            parser.PeriodText = "Last 2 years";
            parser.TryParse();
            parserResult = parser.Result;

            AssertDictionaryValue("Period", ProfitAndLossPeriod.Yearly);
            AssertDictionaryValue("Type", "EntireYear");
            AssertDictionaryValue("Year1", CurrentYear - 2);
            AssertDictionaryValue("Year2", CurrentYear);
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
            parser.PeriodText = text;
            parser.TryParse();
            parserResult = parser.Result;

            AssertDictionaryValue("Period", ProfitAndLossPeriod.Yearly);
            AssertDictionaryValue("Type", "EntireYear");
            AssertDictionaryValue("Year1", 2018);
            AssertDictionaryValue("Year2", 2020);
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
            parser.PeriodText = text;
            parser.TryParse();
            parserResult = parser.Result;

            AssertDictionaryValue("Period", ProfitAndLossPeriod.Yearly);
            AssertDictionaryValue("Type", "EntireYear");
            AssertDictionaryValue("Month1", 4);
            AssertDictionaryValue("Year1", 2018);
            AssertDictionaryValue("Year2", 2020);
        }

        [TestCase("April 2018 Yearly")]
        [TestCase("Apr 18")]
        [TestCase("04 2018 years")]
        [TestCase("4 18")]
        [TestCase("April,2018")]
        [TestCase("Apr.18 yearly")]
        [TestCase("04/2018 yearly")]
        [TestCase("4.18")]
        public void EntireYear_WithMonthAndYear_Parser(string text)
        {
            parser.PeriodText = text;
            parser.TryParse();
            parserResult = parser.Result;

            AssertDictionaryValue("Period", ProfitAndLossPeriod.Yearly);
            AssertDictionaryValue("Type", "EntireYear");
            AssertDictionaryValue("Month1", 4);
            AssertDictionaryValue("Year1", 2018);
        }

        [TestCase(" 2018 Yearly")]
        [TestCase("18")]
        [TestCase("2018 years")]
        [TestCase(" 18 ")]
        [TestCase(" 2018  yearly")]
        public void EntireYear_WithYear_Parser(string text)
        {
            parser.PeriodText = text;
            parser.TryParse();
            parserResult = parser.Result;

            AssertDictionaryValue("Period", ProfitAndLossPeriod.Yearly);
            AssertDictionaryValue("Type", "EntireYear");
            AssertDictionaryValue("Year1", 2018);
        }

        void AssertDictionaryValue(string key, object value)
        {
            Assert.IsTrue(parserResult.ContainsKey(key) && parserResult[key].ToString() == value.ToString());
        }
    }
}
