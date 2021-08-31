using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PeriodParser
{
    public class QuarterParserRegex : PeriodParserRegex
    {
        private QuarterParserRegex() : base() { }
        private static QuarterParserRegex instance = null;
        public static QuarterParserRegex GetInstance()
        {
            if (instance == null)
            {
                instance = new QuarterParserRegex();
            }
            return instance;
        }
        public override bool Parse()
        {
            Result = new Dictionary<string, object>();
            Result.Add(Period, ProfitAndLossPeriod.Quarterly);

            bool isValid = false;
            if (TryParseLastDefinition(PeriodText))
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

                if (isValid)
                {
                    if (Result.ContainsKey(Quarter2))
                        Result.Add(Type, "Consecutive");
                    else
                        Result.Add(Type, "EachYear");
                }
            }

            return isValid;
        }

        bool TryParse(string text)
        {
            if (TryParseQuarterAndYear(text))
            {
                return true;
            }
            else if (TryParseYear(text))
            {
                return true;
            }
            else if (TryParseQuarter(text))
            {
                return true;
            }
            return false;
        }

        bool TryParseLastDefinition(string text)
        {
            if (TryParseEachYearLastDefinition(text))
            {
                return true;
            }
            else if (TryParseConsecutiveLastDefinition(text))
            {
                return true;
            }
            return false;
        }

        bool TryParseEachYearLastDefinition(string periodText)
        {
            Regex rgx = new Regex(@"^(q1|q2|q3|q4|this\s*quarter)\w*\s*for\s*last\s*(\d+)\s*year");
            Match match = rgx.Match(periodText);
            if (match.Success)
            {
                int yearlyDifference;
                if (int.TryParse(match.Groups[2].Value, out yearlyDifference))
                {
                    var quarterText = match.Groups[1].Value;
                    var quarterNumber = GetQuarterNumber(quarterText);
                    if (quarterNumber == 0)
                        quarterNumber = CurrentQuarter;
                    Result.Add(Quarter1, quarterNumber);
                    Result.Add(Year1, CurrentYear - yearlyDifference);
                    Result.Add(Year2, CurrentYear);
                    Result.Add(Type, "EachYear");
                    return true;
                }
            }
            return false;
        }

        bool TryParseConsecutiveLastDefinition(string periodText)
        {
            Regex rgx = new Regex(@"last\s*(\d+)\s*quarter");
            Match match = rgx.Match(periodText);
            if (match.Success)
            {
                int quarterlyDifference;
                if (int.TryParse(match.Groups[1].Value, out quarterlyDifference))
                {
                    var beginQuarterAndYear = GetBeginQuarterAndYearFromDifference(CurrentQuarter, CurrentYear, quarterlyDifference);
                    Result.Add(Quarter1, beginQuarterAndYear.quarter);
                    Result.Add(Year1, beginQuarterAndYear.year);
                    Result.Add(Quarter2, CurrentQuarter);
                    Result.Add(Year2, CurrentYear);
                    Result.Add(Type, "Consecutive");
                    return true;
                }
            }
            return false;
        }
    }
}
