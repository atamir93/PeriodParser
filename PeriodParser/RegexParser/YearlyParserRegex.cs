using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PeriodParser.RegexParser
{
    public class YearlyParserRegex : PeriodParserRegex
    {
        private YearlyParserRegex() : base() { }
        public string YearlyType;
        private static YearlyParserRegex instance = null;
        public static YearlyParserRegex GetInstance(string yearlyType)
        {
            if (instance == null)
            {
                instance = new YearlyParserRegex();
            }
            instance.YearlyType = yearlyType;
            return instance;
        }

        public override bool Parse()
        {
            bool isValid = false;
            Result = new Dictionary<string, object>
            {
                { Period, ProfitAndLossPeriod.Yearly },
                { Type, YearlyType }
            };

            if (TryParseToYearWithLastDefinition(PeriodText))
                isValid = true;
            else
            {
                isValid = TryParseDateRanges(isValid);
            }

            return isValid;
        }

        bool TryParseDateRanges(bool isValid)
        {
            var dateRanges = SplitByDash(PeriodText);

            if (TryParse(dateRanges[0]))
            {
                if (dateRanges.Length > 1)
                {
                    if (TryParse(dateRanges[1]))
                        isValid = true;
                }
                else
                    isValid = true;
            }

            return isValid;
        }

        bool TryParse(string text)
        {
            bool isParsed = false;
            if (TryParseMonthAndYear(text))
                isParsed = true;
            else if (TryParseYear(text))
                isParsed = true;

            return isParsed;
        }

        bool TryParseToYearWithLastDefinition(string periodText)
        {
            bool isParsed = false;
            Regex rgx = new Regex(@"last\s*(\d+)\s*year");
            Match match = rgx.Match(periodText);
            if (match.Success)
            {
                if (int.TryParse(match.Groups[1].Value, out int yearDifference))
                {
                    Result.Add(Year1, CurrentYear - yearDifference);
                    Result.Add(Year2, CurrentYear);
                    isParsed = true;
                }
            }
            return isParsed;
        }
    }
}
