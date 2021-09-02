using System.Collections.Generic;

namespace PeriodParser.RegexParser
{
    public class SeasonsParser : PeriodParser
    {
        private SeasonsParser() : base() { }
        private static SeasonsParser instance = null;
        public static SeasonsParser GetInstance()
        {
            if (instance == null)
                instance = new SeasonsParser();
            return instance;
        }

        public override bool TryParse()
        {
            Result = new Dictionary<string, object>
            {
                { Period, ProfitAndLossPeriod.MonthRange }
            };

            bool isValid = TryParseDateRangesConsideringEndingRange(3);
            if (isValid)
                AddMissedDates();

            return isValid;
        }

        internal override bool TryParseDateText(string text, bool isEndRange = false)
        {
            return TryParseMonthAndYear(text, isEndRange) || TryParseYearAndMonthName(text, isEndRange) || TryParseMonth(text, isEndRange) || TryParseYear(text);
        }

        void AddMissedDates()
        {
            if (Result.ContainsKey(Month1) && !Result.ContainsKey(Month2))
            {
                Result.Add(Month2, Result[Month1]);
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
    }
}
