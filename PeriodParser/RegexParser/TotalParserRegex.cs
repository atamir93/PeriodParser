using System.Collections.Generic;

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
            Result = new Dictionary<string, object>
            {
                { Period, ProfitAndLossPeriod.Single }
            };

            bool isValid = TryParseDateRanges();
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

        private bool TryParseDateRanges()
        {
            bool isValid = false;
            var dateRanges = SplitByDash(PeriodText);
            if (TryParse(dateRanges[0]))
            {
                if (dateRanges.Length > 1)
                {
                    if (TryParse(dateRanges[1], true))
                        isValid = true;
                }
                else
                    isValid = true;
            }

            return isValid;
        }

        bool TryParse(string text, bool isEndRange = false)
        {
            bool isParsed = false;
            if (TryParseMonthAndYear(text, isEndRange))
            {
                isParsed = true;
            }
            else if (TryParseYear(text))
            {
                isParsed = true;
            }
            else if (TryParseMonth(text, isEndRange))
            {
                isParsed = true;
            }
            return isParsed;
        }
    }
}
