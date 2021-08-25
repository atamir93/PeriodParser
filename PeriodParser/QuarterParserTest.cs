﻿using NUnit.Framework;
using System.Collections.Generic;

namespace PeriodParser
{
    [TestFixture]
    public class QuarterParserTest
    {
        private QuarterParser parser;
        Dictionary<string, object> parserResult;
        const int CurrentYear = 2021;
        const int CurrentQuarter = 3;

        [SetUp]
        public void SetUp()
        {
            parser = new QuarterParser();
        }

        [TestCase("This quarter for last 2 years")]
        [TestCase("Q3 for last 2 years")]
        public void QuartersEachYearType_LastDefitinion_Parser(string text)
        {
            parser.Parse(text);
            parserResult = parser.Result;

            AssertDictionaryValue("Period", "Quarters");
            AssertDictionaryValue("Type", "EachYear");
            //AssertDictionaryValue("YearlyPeriod", "Calendar");
            AssertDictionaryValue("BeginQuarter", CurrentQuarter);
            AssertDictionaryValue("BeginYear", CurrentYear - 2);
            AssertDictionaryValue("EndingYear", CurrentYear);
        }

        [TestCase("Q2 2018 - 2020")]
        [TestCase("q2 18-20")]
        [TestCase("Q2,2018 -2020")]
        [TestCase("Q2.18- 20")]
        [TestCase("Q2/2018 - 2020")]
        public void QuartersEachYearType_WithDateRange_Parser(string text)
        {
            parser.Parse(text);
            parserResult = parser.Result;

            AssertDictionaryValue("Period", "Quarters");
            AssertDictionaryValue("Type", "EachYear");
            //AssertDictionaryValue("YearlyPeriod", "Calendar");
            AssertDictionaryValue("BeginQuarter", 2);
            AssertDictionaryValue("BeginYear", 2018);
            AssertDictionaryValue("EndingYear", 2020);
        }

        [TestCase("Last 2 quarters", CurrentYear, 1)]
        [TestCase("Last 4 quarters", CurrentYear - 1, 3)]
        [TestCase("Last 7 quarters", CurrentYear - 2, 4)]
        [TestCase("Last 10 quarters", CurrentYear - 2, 1)]
        [TestCase("Last 13 quarters", CurrentYear - 3, 2)]
        [TestCase("Last 19 quarters", CurrentYear - 5, 4)]
        public void QuartersConsecutiveType_LastDefitinion_Parser(string text, int beginYear, int beginQuarter)
        {
            parser.Parse(text);
            parserResult = parser.Result;

            AssertDictionaryValue("Period", "Quarters");
            AssertDictionaryValue("Type", "Consecutive");
            //AssertDictionaryValue("YearlyPeriod", "beginQuarter");
            AssertDictionaryValue("BeginQuarter", beginQuarter);
            AssertDictionaryValue("BeginYear", beginYear);
            AssertDictionaryValue("EndingQuarter", CurrentQuarter);
            AssertDictionaryValue("EndingYear", CurrentYear);
        }


        [TestCase("Q2 2018 - Q1 2020")]
        [TestCase("q2 18- q1 20")]
        [TestCase("Q2,2018-Q1,2020")]
        [TestCase("Q2.18 -Q1.20")]
        [TestCase("Q2/2018 - Q1/2020")]
        public void QuartersConsecutive_WithDateRange_Parser(string text)
        {
            parser.Parse(text);
            parserResult = parser.Result;

            AssertDictionaryValue("Period", "Quarters");
            AssertDictionaryValue("Type", "Consecutive");
            //AssertDictionaryValue("YearlyPeriod", "Calendar");
            AssertDictionaryValue("BeginQuarter", 2);
            AssertDictionaryValue("EndingQuarter", 1);
            AssertDictionaryValue("BeginYear", 2018);
            AssertDictionaryValue("EndingYear", 2020);
        }

        void AssertDictionaryValue(string key, object value)
        {
            Assert.IsTrue(parserResult.ContainsKey(key) && parserResult[key].ToString() == value.ToString());
        }
    }
}