using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PeriodParser.RegexParser
{
    public class EntireYearParser : PeriodParser
    {
        private EntireYearParser() : base() { }
        private static EntireYearParser instance = null;
        public static EntireYearParser GetInstance()
        {
            if (instance == null)
                instance = new EntireYearParser();
            return instance;
        }
        public override bool TryParse()
        {
            Result = new Dictionary<string, object>
            {
                { Period, ProfitAndLossPeriod.Yearly },
                { Type, "EntireYear" }
            };
            bool isValid = TryParseToYearWithLastDefinition(PeriodText) || TryParseDateRanges();
            if (isValid)
                AddMissedDates();
            return isValid;
        }

        internal override bool TryParseDateText(string text, bool isEndRange = false)
        {
            return TryParseMonthAndYear(text) || TryParseYearAndMonthName(text) || TryParseYear(text);
        }

        bool TryParseToYearWithLastDefinition(string periodText)
        {
            Regex rgx = new Regex(@"last\s*(\d+)\s*year");
            Match match = rgx.Match(periodText);
            if (match.Success)
            {
                int yearDifference;
                if (int.TryParse(match.Groups[1].Value, out yearDifference))
                {
                    var lastYear = GetLastMonthQuarterYear().year;
                    Result.Add(Year1, lastYear - yearDifference + 1);
                    Result.Add(Year2, lastYear);
                    return true;
                }
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
