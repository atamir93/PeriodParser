using NUnit.Framework;
using PeriodParser.RegexParser;
using System.Collections.Generic;

namespace PeriodParser.Tests
{
    [TestFixture]
    public class QuarterParserTest
    {
        private QuarterParser parser;
        Dictionary<string, object> parserResult;
        const int CurrentYear = 2020;
        const int CurrentQuarter = 2;

        [SetUp]
        public void SetUp()
        {
            parser = QuarterParser.GetInstance();
        }

        [TestCase("This quarter for last 2 years")]
        [TestCase("Q2 for last 2 years")]
        public void QuartersEachYearType_LastDefitinion_Parser(string text)
        {
            parser.PeriodText = text;
            parser.TryParse();
            parserResult = parser.Result;

            AssertDictionaryValue("Period", ProfitAndLossPeriod.Quarterly);
            AssertDictionaryValue("Type", "EachYear");
            //AssertDictionaryValue("YearlyPeriod", "Calendar");
            AssertDictionaryValue("Quarter1", CurrentQuarter);
            AssertDictionaryValue("Year1", CurrentYear - 2);
            AssertDictionaryValue("Year2", CurrentYear);
        }

        [TestCase("Q2 2018 - 2020")]
        [TestCase("q2 18-20")]
        [TestCase("Q2,2018 -2020")]
        [TestCase("Q2.18- 20")]
        [TestCase("Q2/2018 - 2020")]
        public void QuartersEachYearType_WithDateRange_Parser(string text)
        {
            parser.PeriodText = text;
            parser.TryParse();
            parserResult = parser.Result;

            AssertDictionaryValue("Period", ProfitAndLossPeriod.Quarterly);
            AssertDictionaryValue("Type", "EachYear");
            //AssertDictionaryValue("YearlyPeriod", "Calendar");
            AssertDictionaryValue("Quarter1", 2);
            AssertDictionaryValue("Year1", 2018);
            AssertDictionaryValue("Year2", 2020);
        }

        [TestCase("Last 2 quarters", CurrentYear, 1)]
        [TestCase("Last 4 quarters", CurrentYear - 1, 3)]
        [TestCase("Last 7 quarters", CurrentYear - 2, 4)]
        [TestCase("Last 10 quarters", CurrentYear - 2, 1)]
        [TestCase("Last 13 quarters", CurrentYear - 3, 2)]
        [TestCase("Last 19 quarters", CurrentYear - 5, 4)]
        public void QuartersConsecutiveType_LastDefitinion_Parser(string text, int beginYear, int beginQuarter)
        {
            parser.PeriodText = text;
            parser.TryParse();
            parserResult = parser.Result;

            AssertDictionaryValue("Period", ProfitAndLossPeriod.Quarterly);
            AssertDictionaryValue("Type", "Consecutive");
            //AssertDictionaryValue("YearlyPeriod", "beginQuarter");
            AssertDictionaryValue("Quarter1", beginQuarter);
            AssertDictionaryValue("Year1", beginYear);
            AssertDictionaryValue("Quarter2", CurrentQuarter);
            AssertDictionaryValue("Year2", CurrentYear);
        }

        [TestCase("Q2 2018 - Q1 2020")]
        [TestCase("q2 18- q1 20")]
        [TestCase("Q2,2018-Q1,2020")]
        [TestCase("Q2.18 -Q1.20")]
        [TestCase("Q2/2018 - Q1/2020")]
        public void QuartersConsecutive_WithDateRange_Parser(string text)
        {
            parser.PeriodText = text;
            parser.TryParse();
            parserResult = parser.Result;

            AssertDictionaryValue("Period", ProfitAndLossPeriod.Quarterly);
            AssertDictionaryValue("Type", "Consecutive");
            //AssertDictionaryValue("YearlyPeriod", "Calendar");
            AssertDictionaryValue("Quarter1", 2);
            AssertDictionaryValue("Quarter2", 1);
            AssertDictionaryValue("Year1", 2018);
            AssertDictionaryValue("Year2", 2020);
        }

        void AssertDictionaryValue(string key, object value)
        {
            Assert.IsTrue(parserResult.ContainsKey(key) && parserResult[key].ToString() == value.ToString());
        }
    }
}
