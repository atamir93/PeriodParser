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
            {
                instance = new SeasonsParser();
            }
            return instance;
        }

        public override bool TryParse()
        {
            Result = new Dictionary<string, object>
            {
                { Period, ProfitAndLossPeriod.MonthRange }
            };

            return TryParseDateRangesConsideringEndingRange(3);
        }

        internal override bool TryParseDateText(string text, bool isEndRange = false)
        {
            return TryParseMonthAndYear(text, isEndRange) || TryParseMonth(text, isEndRange) || TryParseYear(text);
        }
    }
}
