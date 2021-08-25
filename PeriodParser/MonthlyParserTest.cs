﻿using NUnit.Framework;
using System.Collections.Generic;

namespace PeriodParser
{
    [TestFixture]
    public class MonthlyParserTest
    {
        private MonthlyParser parser;
        Dictionary<string, object> parserResult;
        const int CurrentYear = 2021;
        const int CurrentMonth = 8;

        [SetUp]
        public void SetUp()
        {
            parser = new MonthlyParser();
        }

        [TestCase("This month for last 2 years")]
        [TestCase("August for last 2 years")]
        [TestCase("Aug for last 2 years")]
        public void MonthsEachYearType_LastDefitinion_Parser(string text)
        {
            parser.Parse(text);
            parserResult = parser.Result;

            AssertDictionaryValue("Period", "Months");
            AssertDictionaryValue("Type", "EachYear");
            //AssertDictionaryValue("YearlyPeriod", "Calendar");
            AssertDictionaryValue("BeginMonth", CurrentMonth);
            AssertDictionaryValue("BeginYear", CurrentYear - 2);
            AssertDictionaryValue("EndingYear", CurrentYear);
        }

        [TestCase("april 2018 - 2020")]
        [TestCase("apr 18-20 months")]
        [TestCase("April,2018 -2020")]
        [TestCase("4.18- 20 monthly")]
        [TestCase("04/2018 - 2020")]
        public void MonthsEachYearType_WithDateRange_Parser(string text)
        {
            parser.Parse(text);
            parserResult = parser.Result;

            AssertDictionaryValue("Period", "Months");
            AssertDictionaryValue("Type", "EachYear");
            //AssertDictionaryValue("YearlyPeriod", "Calendar");
            AssertDictionaryValue("BeginMonth", 4);
            AssertDictionaryValue("BeginYear", 2018);
            AssertDictionaryValue("EndingYear", 2020);
        }

        [TestCase("Last 2 months", CurrentYear, CurrentMonth-2)]
        [TestCase("Last 7 months", CurrentYear, CurrentMonth-7)]
        [TestCase("Last 8 months", CurrentYear - 1, 12)]
        [TestCase("Last 16 months", CurrentYear - 1, CurrentMonth-4)]
        [TestCase("Last 20 months", CurrentYear - 2, 12)]
        [TestCase("Last 25 months", CurrentYear - 2, CurrentMonth-1)]
        [TestCase("Last 36 months", CurrentYear - 3, CurrentMonth)]
        public void MonthsConsecutiveType_LastDefitinion_Parser(string text, int beginYear, int beginQuarter)
        {
            parser.Parse(text);
            parserResult = parser.Result;

            AssertDictionaryValue("Period", "Months");
            AssertDictionaryValue("Type", "Consecutive");
            //AssertDictionaryValue("YearlyPeriod", "beginQuarter");
            AssertDictionaryValue("BeginMonth", beginQuarter);
            AssertDictionaryValue("BeginYear", beginYear);
            AssertDictionaryValue("EndingMonth", CurrentMonth);
            AssertDictionaryValue("EndingYear", CurrentYear);
        }

        [TestCase("April 2018 - November 2020 monthly")]
        [TestCase("apr 18- nov 20")]
        [TestCase("04,2018-11,2020")]
        [TestCase("4.18 -11.20 months")]
        [TestCase("04/2018 - 11/2020")]
        public void MonthsConsecutive_WithDateRange_Parser(string text)
        {
            parser.Parse(text);
            parserResult = parser.Result;

            AssertDictionaryValue("Period", "Months");
            AssertDictionaryValue("Type", "Consecutive");
            //AssertDictionaryValue("YearlyPeriod", "Calendar");
            AssertDictionaryValue("BeginMonth", 4);
            AssertDictionaryValue("EndingMonth", 11);
            AssertDictionaryValue("BeginYear", 2018);
            AssertDictionaryValue("EndingYear", 2020);
        }

        void AssertDictionaryValue(string key, object value)
        {
            Assert.IsTrue(parserResult.ContainsKey(key) && parserResult[key].ToString() == value.ToString());
        }
    }
}