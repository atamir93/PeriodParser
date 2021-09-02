using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PeriodParser.RegexParser
{
    public class YearToDateParser : PeriodParser
    {
        private YearToDateParser() : base() { }
        private static YearToDateParser instance = null;
        public static YearToDateParser GetInstance()
        {
            if (instance == null)
                instance = new YearToDateParser();
            return instance;
        }
        public override bool TryParse()
        {
            Result = new Dictionary<string, object>
            {
                { Period, ProfitAndLossPeriod.Yearly },
                { Type, "YTD" }
            };

            bool isValid = TryParseToYearWithLastDefinition(PeriodText) || TryParseDateRanges();
            if (isValid)
                AddMissedDates();
            return isValid;
        }

        internal override bool TryParseDateText(string text, bool isEndRange = false)
        {
            return TryParseOnlyYTDText(text) || TryParseMonthAndYear(text) || TryParseYearAndMonthName(text, isEndRange) || TryParseYear(text) || TryParseMonth(text);
        }

        bool TryParseOnlyYTDText(string text)
        {
            if (text == YearToDateDefinition)
            {
                Result.Add(Month1, EndingMonth);
                Result.Add(Year1, CurrentYear);
                return true;
            }
            return false;
        }

        bool TryParseToYearWithLastDefinition(string periodText)
        {
            Regex rgx = new Regex(@"last\s*(\d+)\s*year");
            Match match = rgx.Match(periodText);
            if (match.Success && int.TryParse(match.Groups[1].Value, out int yearDifference))
            {
                var lastYear = GetLastMonthQuarterYear().year;
                Result.Add(Year1, lastYear - yearDifference + 1);
                Result.Add(Year2, lastYear);
                return true;
            }
            return false;
        }

        void AddMissedDates()
        {
            if (!Result.ContainsKey(Year1))
            {
                Result.Add(Year1, CurrentYear);
            }
            if (!Result.ContainsKey(Year2))
            {
                Result.Add(Year2, Result[Year1]);
            }
        }
    }
}
