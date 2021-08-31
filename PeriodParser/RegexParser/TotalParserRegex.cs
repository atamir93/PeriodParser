using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PeriodParser.RegexParser
{
    public class TotalParserRegex : PeriodParserRegex
    {
        private TotalParserRegex() : base() { }
        private static TotalParserRegex instance = null;
        public static TotalParserRegex GetInstance()
        {
            if (instance == null)
            {
                instance = new TotalParserRegex();
            }
            return instance;
        }

        public override bool Parse()
        {
            bool isValid = false;
            Result = new Dictionary<string, object>();
            Result.Add(Period, ProfitAndLossPeriod.Single);

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
            if (isValid)
            {
                if (!Result.ContainsKey(Month1))
                {
                    Result.Add(Month1, FirstMonth);
                }
                if (!Result.ContainsKey(Month2))
                {
                    Result.Add(Month2, LastMonth);
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
            else if (TryParseYear(text))
            {
                return true;
            }
            else if (TryParseMonth(text, isEndRange))
            {
                return true;
            }
            return false;
        }
    }
}
