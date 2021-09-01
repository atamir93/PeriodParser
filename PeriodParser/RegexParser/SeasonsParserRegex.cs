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
