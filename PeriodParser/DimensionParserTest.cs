using NUnit.Framework;
using System.Collections.Generic;

namespace PeriodParser
{
    [TestFixture]
    public class DimensionParserTest
    {
        private DimensionParser parser;
        Dictionary<string, object> parserResult;

        [SetUp]
        public void SetUp()
        {
            parser = new DimensionParser();
        }

        [TestCase("2018 by location")]
        [TestCase(" 18 by  location ")]
        public void DimensionParser_EntireYear_Parser(string text)
        {
            parser.PeriodText = text;
            parser.Parse();
            parserResult = parser.Result;

            AssertDictionaryValue("Period", "Dimension");
            AssertDictionaryValue("DimensionName", "location");
            AssertDictionaryValue("DimensionPeriod", "EntireYear");
            AssertDictionaryValue("Year1", 2018);
        }

        [TestCase("june 2018 YTD by some location")]
        [TestCase("06.18  YTD by Some Location ")]
        [TestCase("06/18 YTD by Some Location ")]
        [TestCase("june, 2018 YTD by some location ")]
        [TestCase("06.18 YTD by some location ")]
        public void DimensionParser_YearToDate_Parser(string text)
        {
            parser.PeriodText = text;
            parser.Parse();
            parserResult = parser.Result;

            AssertDictionaryValue("Period", "Dimension");
            AssertDictionaryValue("DimensionName", "some location");
            AssertDictionaryValue("DimensionPeriod", "YearToDate");
            AssertDictionaryValue("Month1", 6);
            AssertDictionaryValue("Year1", 2018);
        }

        [TestCase("june 2018 by Location")]
        [TestCase("06.18  by location ")]
        [TestCase("06/18  by location ")]
        [TestCase("june, 2018 by  location ")]
        [TestCase("06.18 by location ")]
        public void DimensionParser_Month_Parser(string text)
        {
            parser.PeriodText = text;
            parser.Parse();
            parserResult = parser.Result;

            AssertDictionaryValue("Period", "Dimension");
            AssertDictionaryValue("DimensionName", "location");
            AssertDictionaryValue("DimensionPeriod", "Month");
            AssertDictionaryValue("Month1", 6);
            AssertDictionaryValue("Year1", 2018);
        }

        [TestCase("Q2 2018 by Location")]
        [TestCase("q2.18  by location ")]
        [TestCase("Q2 2018  by location ")]
        public void DimensionParser_Quarter_Parser(string text)
        {
            parser.PeriodText = text;
            parser.Parse();
            parserResult = parser.Result;

            AssertDictionaryValue("Period", "Dimension");
            AssertDictionaryValue("DimensionName", "location");
            AssertDictionaryValue("DimensionPeriod", "Quarter");
            AssertDictionaryValue("Quarter1", 2);
            AssertDictionaryValue("Year1", 2018);
        }

        [TestCase("April 2018 - November 2020 by location")]
        [TestCase("apr 18- nov 20 by location ")]
        [TestCase("04,2018-11,2020 by location")]
        [TestCase("4.18 -11.20 by location")]
        [TestCase("04/2018 - 11/2020 by location")]
        public void DimensionParser_Range_Parser(string text)
        {
            parser.PeriodText = text;
            parser.Parse();
            parserResult = parser.Result;

            AssertDictionaryValue("Period", "Dimension");
            AssertDictionaryValue("DimensionName", "location");
            AssertDictionaryValue("DimensionPeriod", "Range");
            AssertDictionaryValue("Month1", 4);
            AssertDictionaryValue("Month2", 11);
            AssertDictionaryValue("Year1", 2018);
            AssertDictionaryValue("Year2", 2020);
        }

        void AssertDictionaryValue(string key, object value)
        {
            Assert.IsTrue(parserResult.ContainsKey(key) && parserResult[key].ToString() == value.ToString());
        }
    }
}
