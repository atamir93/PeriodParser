using System;
using System.Collections.Generic;

namespace PeriodParser.RegexParser
{
    public class SeasonsParserRegex : PeriodParserRegex
    {
        private SeasonsParserRegex() : base() { }
        private static SeasonsParserRegex instance = null;
        public static SeasonsParserRegex GetInstance()
        {
            if (instance == null)
            {
                instance = new SeasonsParserRegex();
            }
            return instance;
        }

        public override bool Parse()
        {
            Result = new Dictionary<string, object>
            {
                { Period, ProfitAndLossPeriod.MonthRange }
            };

            return TryParseDataRanges();
        }

        private bool TryParseDataRanges()
        {
            bool isValid = false;
            var dateRanges = SplitByDash(PeriodText);
            bool isEndingRange = false;
            for (int i = 0; i < Math.Min(3, dateRanges.Length); i++)
            {
                if (i > 0)
                    isEndingRange = true;
                isValid = TryParse(dateRanges[i], isEndingRange);
            }

            return isValid;
        }

        bool TryParse(string text, bool isEndRange = false)
        {
            return TryParseMonthAndYear(text, isEndRange) || TryParseMonth(text, isEndRange) || TryParseYear(text);
        }
    }
}
