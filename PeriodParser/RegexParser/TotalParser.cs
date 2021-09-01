using System.Collections.Generic;

namespace PeriodParser.RegexParser
{
    public class TotalParser : PeriodParser
    {
        private TotalParser() : base() { }
        private static TotalParser instance = null;
        public static TotalParser GetInstance()
        {
            if (instance == null)
            {
                instance = new TotalParser();
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
