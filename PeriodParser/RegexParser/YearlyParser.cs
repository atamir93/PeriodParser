using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PeriodParser.RegexParser
{
    public class YearlyParser : PeriodParser
    {
        private YearlyParser() : base() { }
        public string YearlyType;
        private static YearlyParser instance = null;
        public static YearlyParser GetInstance(string yearlyType)
        {
            if (instance == null)
            {
                instance = new YearlyParser();
            }
            instance.YearlyType = yearlyType;
            return instance;
        }

        public override bool TryParse()
        {
            Result = new Dictionary<string, object>
            {
                { Period, ProfitAndLossPeriod.Yearly },
                { Type, YearlyType }
            };

            return TryParseToYearWithLastDefinition(PeriodText) || TryParseDateRanges();
        }

        internal override bool TryParseDateText(string text, bool isEndRange = false)
        {
            return TryParseMonthAndYear(text) || TryParseYear(text);
        }

        bool TryParseToYearWithLastDefinition(string periodText)
        {
            bool isParsed = false;
            Regex rgx = new Regex(@"last\s*(\d+)\s*year");
            Match match = rgx.Match(periodText);
            if (match.Success && int.TryParse(match.Groups[1].Value, out int yearDifference))
            {
                Result.Add(Year1, CurrentYear - yearDifference);
                Result.Add(Year2, CurrentYear);
                isParsed = true;
            }
            return isParsed;
        }
    }
}
