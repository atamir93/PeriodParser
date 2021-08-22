using NUnit.Framework;
using System.Collections.Generic;

namespace PeriodParser
{
    [TestFixture]
    public class SeasonsParserTest
    {
        private SeasonsParser parser;
        Dictionary<string, object> parserResult;

        [SetUp]
        public void SetUp()
        {
            parser = new SeasonsParser();
        }

        [TestCase("June-December 2018-2020 Season")]
        [TestCase("jun- dec, 18- 20")]
        [TestCase("06-12,2018-2020 seasons")]
        [TestCase("6-12 18 -20")]
        [TestCase("06-12 2018-20")]
        public void Seasons_WithDateRange_Parser(string text)
        {
            parser.Parse(text);
            parserResult = parser.Result;

            AssertDictionaryValue("Period", "Seasons");
            //AssertDictionaryValue("YearlyPeriod", "Calendar");
            AssertDictionaryValue("BeginMonth", 6);
            AssertDictionaryValue("EndingMonth", 12);
            AssertDictionaryValue("BeginYear", 2018);
            AssertDictionaryValue("EndingYear", 2020);
        }

        [TestCase("June-December 2018 Season")]
        [TestCase("jun- dec, 18")]
        [TestCase("06-12,2018 seasons")]
        [TestCase("6-12 18")]
        [TestCase("06-12 2018")]
        public void Seasons_WithSingleYear_Parser(string text)
        {
            parser.Parse(text);
            parserResult = parser.Result;

            AssertDictionaryValue("Period", "Seasons");
            //AssertDictionaryValue("YearlyPeriod", "Calendar");
            AssertDictionaryValue("BeginMonth", 6);
            AssertDictionaryValue("EndingMonth", 12);
            AssertDictionaryValue("BeginYear", 2018);
            AssertDictionaryValue("EndingYear", 2018);
        }

        void AssertDictionaryValue(string key, object value)
        {
            Assert.IsTrue(parserResult.ContainsKey(key) && parserResult[key].ToString() == value.ToString());
        }
    }
}
