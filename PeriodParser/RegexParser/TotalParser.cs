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
                instance = new TotalParser();
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
                AddMissedMonthes();

            return isValid;
        }

        void AddMissedMonthes()
        {
            if (!Result.ContainsKey(Month1))
            {
                Result.Add(Month1, FirstMonth);
            }
            if (!Result.ContainsKey(Month2))
            {
                Result.Add(Month2, LastMonth);
            }
            if (!Result.ContainsKey(Year1) && !Result.ContainsKey(Year2))
            {
                var beginYear = CurrentYear;
                if ((int)Result[Month1] > (int)Result[Month2])
                    beginYear = CurrentYear - 1;

                Result.Add(Year1, beginYear);
                Result.Add(Year2, CurrentYear);
            }
            else if (Result.ContainsKey(Year1) && !Result.ContainsKey(Year2))
            {
                Result.Add(Year2, Result[Year1]);
            }
        }

        internal override bool TryParseDateText(string text, bool isEndRange = false)
        {
            return TryParseMonthAndYear(text, isEndRange) || TryParseYearAndMonthName(text, isEndRange) || TryParseYear(text) || TryParseMonth(text, isEndRange);
        }
    }
}
