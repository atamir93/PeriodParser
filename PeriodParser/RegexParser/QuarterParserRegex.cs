using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PeriodParser.RegexParser
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
        public override bool TryParse()
        {
            Result = new Dictionary<string, object>
            {
                { Period, ProfitAndLossPeriod.Quarterly }
            };

            bool isValid;
            if (TryParseLastDefinition(PeriodText))
                isValid = true;
            else
            {
                isValid = TryParseDateRanges();
                if (isValid)
                    AddQuarterlyType();
            }

            return isValid;
        }

        private void AddQuarterlyType()
        {
            if (Result.ContainsKey(Quarter2))
                Result.Add(Type, "Consecutive");
            else
                Result.Add(Type, "EachYear");
        }

        internal override bool TryParseDateText(string text, bool isEndRange = false)
        {
            return TryParseQuarterAndYear(text) || TryParseYear(text) || TryParseQuarter(text);
        }

        bool TryParseLastDefinition(string text)
        {
            return TryParseEachYearLastDefinition(text) || TryParseConsecutiveLastDefinition(text);
        }

        bool TryParseEachYearLastDefinition(string periodText)
        {
            Regex rgx = new Regex(@"^(q[1-4]|this\s*quarter)\w*\s*for\s*last\s*(\d+)\s*year");
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
