using NUnit.Framework;
using PeriodParser.RegexParser;
using System.Collections.Generic;

namespace PeriodParser.Tests
{
    [TestFixture]
    public class MonthlyParserTest
    {
        private MonthlyParser parser;
        Dictionary<string, object> parserResult;
        const int CurrentYear = 2020;
        const int CurrentMonth = 5;

        [SetUp]
        public void SetUp()
        {
            parser = MonthlyParser.GetInstance();
        }

        [TestCase("This month for last 2 years")]
        [TestCase("May for last 2 years")]
        [TestCase("May for last 2 years")]
        public void MonthsEachYearType_LastDefitinion_Parser(string text)
        {
            parser.PeriodText = text;
            parser.TryParse();
            parserResult = parser.Result;

            AssertDictionaryValue("Period", ProfitAndLossPeriod.Monthly);
            AssertDictionaryValue("Type", "EachYear");
            //AssertDictionaryValue("YearlyPeriod", "Calendar");
            AssertDictionaryValue("Month1", CurrentMonth);
            AssertDictionaryValue("Year1", CurrentYear - 2);
            AssertDictionaryValue("Year2", CurrentYear);
        }

        [TestCase("april 2018 - 2020")]
        [TestCase("apr 18-20 months")]
        [TestCase("April,2018 -2020")]
        [TestCase("4.18- 20 monthly")]
        [TestCase("04/2018 - 2020")]
        public void MonthsEachYearType_WithDateRange_Parser(string text)
        {
            parser.PeriodText = text;
            parser.TryParse();
            parserResult = parser.Result;

            AssertDictionaryValue("Period", ProfitAndLossPeriod.Monthly);
            AssertDictionaryValue("Type", "EachYear");
            AssertDictionaryValue("Month1", 4);
            AssertDictionaryValue("Year1", 2018);
            AssertDictionaryValue("Year2", 2020);
        }

        [TestCase("Last 2 months", CurrentYear, 4)]
        [TestCase("Last 7 months", CurrentYear - 1, 11)]
        [TestCase("Last 8 months", CurrentYear - 1, 10)]
        [TestCase("Last 16 months", CurrentYear - 1, 2)]
        [TestCase("Last 20 months", CurrentYear - 2, 10)]
        [TestCase("Last 25 months", CurrentYear - 2, 5)]
        [TestCase("Last 36 months", CurrentYear - 3, 6)]
        public void MonthsConsecutiveType_LastDefitinion_Parser(string text, int beginYear, int beginMonth)
        {
            parser.PeriodText = text;
            parser.TryParse();
            parserResult = parser.Result;

            AssertDictionaryValue("Period", ProfitAndLossPeriod.Monthly);
            AssertDictionaryValue("Type", "Consecutive");
            AssertDictionaryValue("Month1", beginMonth);
            AssertDictionaryValue("Year1", beginYear);
            AssertDictionaryValue("Month2", CurrentMonth);
            AssertDictionaryValue("Year2", CurrentYear);
        }

        [TestCase("April 2018 - November 2020 monthly")]
        [TestCase("apr 18- nov 20")]
        [TestCase("04,2018-11,2020")]
        [TestCase("4.18 -11.20 months")]
        [TestCase("04/2018 - 11/2020")]
        public void MonthsConsecutive_WithDateRange_Parser(string text)
        {
            parser.PeriodText = text;
            parser.TryParse();
            parserResult = parser.Result;

            AssertDictionaryValue("Period", ProfitAndLossPeriod.Monthly);
            AssertDictionaryValue("Type", "Consecutive");
            //AssertDictionaryValue("YearlyPeriod", "Calendar");
            AssertDictionaryValue("Month1", 4);
            AssertDictionaryValue("Month2", 11);
            AssertDictionaryValue("Year1", 2018);
            AssertDictionaryValue("Year2", 2020);
        }

        [TestCase("Apr - Nov monthly", 4, 11, CurrentYear, CurrentYear)]
        [TestCase("nov-apr monthly", 11, 4, CurrentYear - 1, CurrentYear)]
        public void MonthsConsecutive_BeginAndEndingMonthesOnly_Parser(string text, int beginMonth, int endingMonth, int beginYear, int endingYear)
        {
            parser.PeriodText = text;
            parser.TryParse();
            parserResult = parser.Result;

            AssertDictionaryValue("Period", ProfitAndLossPeriod.Monthly);
            AssertDictionaryValue("Type", "Consecutive");
            //AssertDictionaryValue("YearlyPeriod", "Calendar");
            AssertDictionaryValue("Month1", beginMonth);
            AssertDictionaryValue("Month2", endingMonth);
            AssertDictionaryValue("Year1", beginYear);
            AssertDictionaryValue("Year2", endingYear);
        }

        public void MonthsConsecutive_OnlyOneMonth_Parser()
        {
            parser.PeriodText = "november";
            parser.TryParse();
            parserResult = parser.Result;

            AssertDictionaryValue("Period", ProfitAndLossPeriod.Monthly);
            AssertDictionaryValue("Type", "EachYear");
            //AssertDictionaryValue("YearlyPeriod", "Calendar");
            AssertDictionaryValue("Month1", 11);
            AssertDictionaryValue("Year1", CurrentYear);
            AssertDictionaryValue("Year2", CurrentYear);
        }

        [TestCase("18-20 monthly", 2018, 2020)]
        [TestCase("2020 monthly", 2020, 2020)]
        public void MonthsConsecutive_YearsOnly_Parser(string text, int beginYear, int endingYear)
        {
            parser.PeriodText = text;
            parser.TryParse();
            parserResult = parser.Result;

            AssertDictionaryValue("Period", ProfitAndLossPeriod.Monthly);
            AssertDictionaryValue("Type", "Consecutive");
            //AssertDictionaryValue("YearlyPeriod", "Calendar");
            AssertDictionaryValue("Month1", 1);
            AssertDictionaryValue("Month2", 12);
            AssertDictionaryValue("Year1", beginYear);
            AssertDictionaryValue("Year2", endingYear);
        }

        void AssertDictionaryValue(string key, object value)
        {
            Assert.IsTrue(parserResult.ContainsKey(key) && parserResult[key].ToString() == value.ToString());
        }
    }
}
