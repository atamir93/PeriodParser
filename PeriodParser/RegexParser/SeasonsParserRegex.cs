using System.Collections.Generic;
using System.Text.RegularExpressions;

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
            bool isValid = false;
            Result = new Dictionary<string, object>();
            Result.Add(Period, ProfitAndLossPeriod.MonthRange);

            var dateRanges = SplitByDash(PeriodText);
            if (dateRanges.Length == 1)
            {
                if (TryParse(PeriodText))
                    isValid = true;
            }
            else if (dateRanges.Length == 2)
            {
                if (TryParse(dateRanges[0]))
                {
                    if (TryParse(dateRanges[1], true))
                        isValid = true;
                }
            }
            else if (dateRanges.Length == 3)
            {
                if (TryParse(dateRanges[0]))
                {
                    if (TryParse(dateRanges[1]))
                    {
                        if (TryParse(dateRanges[2], true))
                            isValid = true;
                    }
                }
            }

            return isValid;
        }

        bool TryParse(string text, bool isEndRange = false)
        {
            if (TryParseMonthAndYear(text, isEndRange))
            {
                return true;
            }
            else if (TryParseMonth(text, isEndRange))
            {
                return true;
            }
            else if (TryParseYear(text))
            {
                return true;
            }
            return false;
        }
    }
}
