﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeriodParser
{
    public class TotalParserTest
    {
        private TotalParser parser;
        Dictionary<string, object> parserResult;
        const int CurrentYear = 2021;
        const int CurrentMonth = 8;
        const int FirstMonth = 1;
        const int LasttMonth = 12;

        [SetUp]
        public void SetUp()
        {
            parser = new TotalParser();
        }

        [TestCase("April 2018 - November 2020 totals")]
        [TestCase("apr 18- nov 20")]
        [TestCase("04,2018-11,2020")]
        [TestCase("4.18 -11.20 total")]
        [TestCase("04/2018 - 11/2020")]
        public void Total_WithFullDateRange_Parser(string text)
        {
            parser.Parse(text);
            parserResult = parser.Result;

            AssertDictionaryValue("Period", "Total");
            //AssertDictionaryValue("YearlyPeriod", "Calendar");
            AssertDictionaryValue("BeginMonth", 4);
            AssertDictionaryValue("EndingMonth", 11);
            AssertDictionaryValue("BeginYear", 2018);
            AssertDictionaryValue("EndingYear", 2020);
        }

        [TestCase("April 2018 - 2020 totals")]
        [TestCase("apr 18- 20")]
        [TestCase("04,2018-2020")]
        [TestCase("4.18 -20 total")]
        [TestCase("04/2018 - 2020")]
        public void Total_WithBeginMonthDateRange_Parser(string text)
        {
            parser.Parse(text);
            parserResult = parser.Result;

            AssertDictionaryValue("Period", "Total");
            //AssertDictionaryValue("YearlyPeriod", "Calendar");
            AssertDictionaryValue("BeginMonth", 4);
            AssertDictionaryValue("EndingMonth", 12);
            AssertDictionaryValue("BeginYear", 2018);
            AssertDictionaryValue("EndingYear", 2020);
        }

        [TestCase(" 2018 - November 2020 totals")]
        [TestCase("18- nov 20")]
        [TestCase("2018-11,2020")]
        [TestCase("18 -11.20 total")]
        [TestCase("2018 - 11/2020")]
        public void Total_WithEndingMonthFullDateRange_Parser(string text)
        {
            parser.Parse(text);
            parserResult = parser.Result;

            AssertDictionaryValue("Period", "Total");
            AssertDictionaryValue("BeginMonth", 1);
            AssertDictionaryValue("EndingMonth", 11);
            AssertDictionaryValue("BeginYear", 2018);
            AssertDictionaryValue("EndingYear", 2020);
        }

        [TestCase(" 2018 - 2020 totals")]
        [TestCase("18- 20")]
        [TestCase("2018-2020")]
        [TestCase("18 -20 total")]
        [TestCase("2018 - 2020")]
        public void Total_WithoutMonthsFullDateRange_Parser(string text)
        {
            parser.Parse(text);
            parserResult = parser.Result;

            AssertDictionaryValue("Period", "Total");
            AssertDictionaryValue("BeginMonth", 1);
            AssertDictionaryValue("EndingMonth", 12);
            AssertDictionaryValue("BeginYear", 2018);
            AssertDictionaryValue("EndingYear", 2020);
        }

        [TestCase(" 2018 totals")]
        [TestCase("2018")]
        public void Total_SingleYear_Parser(string text)
        {
            parser.Parse(text);
            parserResult = parser.Result;

            AssertDictionaryValue("Period", "Total");
            AssertDictionaryValue("BeginMonth", 1);
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