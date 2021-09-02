using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PeriodParser.RegexParser
{
    public class QuarterParser : PeriodParser
    {
        private QuarterParser() : base() { }
        private static QuarterParser instance = null;
        public static QuarterParser GetInstance()
        {
            if (instance == null)
                instance = new QuarterParser();
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
                {
                    AddMissedDates();
                    AddQuarterlyType();
                }
            }

            return isValid;
        }

        void AddMissedDates()
        {
            if (!Result.ContainsKey(Quarter1) && !Result.ContainsKey(Quarter2))
            {
                Result.Add(Quarter1, FirstQuarter);
                Result.Add(Quarter2, LastQuarter);
            }
            if (!Result.ContainsKey(Year1))
            {
                Result.Add(Year1, CurrentYear);
            }
            if (!Result.ContainsKey(Year2))
            {
                int beginYear = int.Parse(Result[Year1].ToString());
                if (ContainsBothQuarters())
                {
                    if ((int)Result[Quarter1] > (int)Result[Quarter2])
                    {
                        Result[Year1] = beginYear - 1;
                    }
                }
                Result.Add(Year2, beginYear);
            }
        }

        bool ContainsBothQuarters()
        {
            return Result.ContainsKey(Quarter1) && Result.ContainsKey(Quarter2);
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
            return TryParseQuarterAndYear(text) || TryParseQuarter(text) || TryParseYear(text);
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
