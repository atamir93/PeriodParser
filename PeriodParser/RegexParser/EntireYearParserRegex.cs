using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PeriodParser.RegexParser
{
    public class EntireYearParserRegex : PeriodParserRegex
    {
        private EntireYearParserRegex() : base() { }
        private static EntireYearParserRegex instance = null;
        public static EntireYearParserRegex GetInstance()
        {
            if (instance == null)
            {
                instance = new EntireYearParserRegex();
            }
            return instance;
        }
        public override bool Parse()
        {
            Result = new Dictionary<string, object>();
            Result.Add(Period, ProfitAndLossPeriod.Yearly);
            Result.Add(Type, "EntireYear");
            bool isValid = false;
            if (TryParseToYearWithLastDefinition(PeriodText))
                isValid = true;
            else
            {
                var dateRanges = SplitByDash(PeriodText);
                if (dateRanges.Length == 1)
                {
                    if (TryParse(PeriodText))
                        isValid = true;
                }
                else if (dateRanges.Length == 2)
                {
                    if (TryParse(dateRanges[0]))
                    {
                        if (TryParse(dateRanges[1]))
                            isValid = true;
                    }
                }
            }

            return isValid;
        }

        bool TryParse(string text)
        {
            if (TryParseMonthAndYear(text))
            {
                return true;
            }
            else if (TryParseYear(text))
            {
                return true;
            }
            return false;
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
