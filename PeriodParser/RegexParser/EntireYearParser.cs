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
            {
                instance = new EntireYearParser();
            }
            return instance;
        }
        public override bool TryParse()
        {
            Result = new Dictionary<string, object>
            {
                { Period, ProfitAndLossPeriod.Yearly },
                { Type, "EntireYear" }
            };
            return TryParseToYearWithLastDefinition(PeriodText) || TryParseDateRanges();
        }

        internal override bool TryParseDateText(string text, bool isEndRange = false)
        {
            return TryParseMonthAndYear(text) || TryParseYear(text);
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
                    Result.Add(Year1, CurrentYear - yearDifference);
                    Result.Add(Year2, CurrentYear);
                    return true;
                }
            }
            return false;
        }
    }
}
