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

        public override bool TryParse()
        {
            Result = new Dictionary<string, object>
            {
                { Period, ProfitAndLossPeriod.Single }
            };

            bool isValid = TryParseDateRangesConsideringEndingRange();
            if (isValid)
                AddFirstAndLastMonthes();

            return isValid;
        }

        void AddFirstAndLastMonthes()
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

        internal override bool TryParseDateText(string text, bool isEndRange = false)
        {
            return TryParseMonthAndYear(text, isEndRange) || TryParseYear(text) || TryParseMonth(text, isEndRange);
        }
    }
}
