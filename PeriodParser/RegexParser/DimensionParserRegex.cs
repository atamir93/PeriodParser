using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PeriodParser.RegexParser
{
    public class DimensionParserRegex : PeriodParserRegex
    {
        public const string YearToDateDefinition = "ytd";
        private DimensionParserRegex() : base() { }

        private static DimensionParserRegex instance = null;
        public static DimensionParserRegex GetInstance()
        {
            if (instance == null)
            {
                instance = new DimensionParserRegex();
            }
            return instance;
        }

        public override bool Parse()
        {
            Result = new Dictionary<string, object>();
            bool isValid = false;
            Result.Add(Period, ProfitAndLossPeriod.Dimension);

            if (TryParseDimension(PeriodText))
                isValid = true;

            return isValid;
        }

        protected bool TryParseDimension(string text)
        {
            bool isValid = false;
            Regex rgx = new Regex(@"\s*(.*)\s+by\s*(.*)\s*");
            Match match = rgx.Match(text);
            if (match.Success)
            {
                var dateText = match.Groups[1].Value;
                var dimensionName = match.Groups[2].Value;
                Result.Add(DimensionName, dimensionName);

                if (dateText.Contains("-") && TryParseRange(dateText))
                {
                    if (!Result.ContainsKey(Month1))
                    {
                        Result.Add(Month1, FirstMonth);
                    }
                    if (!Result.ContainsKey(Month2))
                    {
                        Result.Add(Month2, LastMonth);
                    }
                    Result.Add(DimensionPeriod, DimensionCompareType.Range);
                    isValid = true;
                }
                else if (TryParseQuarterAndYear(dateText))
                {
                    Result.Add(DimensionPeriod, DimensionCompareType.Quarter);
                    isValid = true;
                }
                else if (TryParseMonthAndYear(dateText))
                {
                    if (text.Contains(YearToDateDefinition))
                        Result.Add(DimensionPeriod, DimensionCompareType.YearToDate);
                    else
                        Result.Add(DimensionPeriod, DimensionCompareType.Month);

                    isValid = true;
                }
                else if (TryParseYear(dateText))
                {
                    Result.Add(DimensionPeriod, DimensionCompareType.EntireYear);
                    isValid = true;
                }
            }
            return isValid;
        }

        bool TryParseRange(string text)
        {
            bool isValid = false;
            var dateRanges = SplitByDash(text);
            bool isEndingRange = false;
            for (int i = 0; i < Math.Min(2, dateRanges.Length); i++)
            {
                if (i > 0)
                    isEndingRange = true;
                isValid = TryParse(dateRanges[i], isEndingRange);
            }
            return isValid;
        }

        bool TryParse(string text, bool isEndRange = false)
        {
            return TryParseMonthAndYear(text, isEndRange) || TryParseYear(text) || TryParseMonth(text, isEndRange);
        }
    }
}
