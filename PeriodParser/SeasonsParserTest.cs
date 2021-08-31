using NUnit.Framework;
using System.Collections.Generic;

namespace PeriodParser
{
    [TestFixture]
    public class SeasonsParserTest
    {
        private SeasonsParserRegex parser;
        Dictionary<string, object> parserResult;

        [SetUp]
        public void SetUp()
        {
            parser = SeasonsParserRegex.GetInstance();
        }

        //[TestCase("June-December 2018-2020 Season")]
        //[TestCase("jun- dec, 18- 20")]
        [TestCase("06-12,2018-2020 seasons")]
        [TestCase("6-12 18 -20")]
        [TestCase("06-12 2018-20")]
        public void Seasons_WithDateRange_Parser(string text)
        {
            parser.PeriodText = text;
            parser.Parse();
            parserResult = parser.Result;

            AssertDictionaryValue("Period", ProfitAndLossPeriod.MonthRange);
            //AssertDictionaryValue("YearlyPeriod", "Calendar");
            AssertDictionaryValue("Month1", 6);
            AssertDictionaryValue("Month2", 12);
            AssertDictionaryValue("Year1", 2018);
            AssertDictionaryValue("Year2", 2020);
        }

        [TestCase("June-December 2018 Season")]
        [TestCase("jun- dec, 18")]
        [TestCase("06-12,2018 seasons")]
        [TestCase("6-12 18")]
        [TestCase("06-12 2018")]
        public void Seasons_WithSingleYear_Parser(string text)
        {
            parser.PeriodText = text;
            parser.Parse();
            parserResult = parser.Result;

            AssertDictionaryValue("Period", ProfitAndLossPeriod.MonthRange);
            //AssertDictionaryValue("YearlyPeriod", "Calendar");
            AssertDictionaryValue("Month1", 6);
            AssertDictionaryValue("Month2", 12);
            AssertDictionaryValue("Year1", 2018);
        }

        void AssertDictionaryValue(string key, object value)
        {
            Assert.IsTrue(parserResult.ContainsKey(key) && parserResult[key].ToString() == value.ToString());
        }
    }
}
