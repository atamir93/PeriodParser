using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PeriodParser.RegexParser
{
    public class DimensionParser : PeriodParser
    {
        private DimensionParser() : base() { }

        private static DimensionParser instance = null;
        public static DimensionParser GetInstance()
        {
            if (instance == null)
                instance = new DimensionParser();
            return instance;
        }

        public override bool TryParse()
        {
            Result = new Dictionary<string, object>
            {
                { Period, ProfitAndLossPeriod.Dimension }
            };

            return TryParseDimension(PeriodText);
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
                PeriodText = dateText;
                isValid = TryParseDimensionPeriod(dateText);
            }
            return isValid;
        }

        bool TryParseDimensionPeriod(string dateText)
        {
            bool isValid = false;
            if (dateText.Contains("-") && TryParseDateRangesConsideringEndingRange())
            {
                AddMissedMonthes();
                Result.Add(DimensionPeriod, DimensionCompareType.Range);
                isValid = true;
            }
            else if (TryParseQuarterAndYear(dateText) || TryParseYearAndQuarter(dateText))
            {
                Result.Add(DimensionPeriod, DimensionCompareType.Quarter);
                isValid = true;
            }
            else if (TryParseMonthAndYear(dateText) || TryParseYearAndMonthName(dateText))
            {
                if (dateText.Contains(YearToDateDefinition))
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

            return isValid;
        }

        void AddMissedMonthes()
        {
            if (!Result.ContainsKey(Month1))
            {
                Result.Add(Month1, FirstMonthOfYear);
            }
            if (!Result.ContainsKey(Month2))
            {
                Result.Add(Month2, LastMonthOfYear);
            }
        }

        internal override bool TryParseDateText(string text, bool isEndRange = false)
        {
            return TryParseMonthAndYear(text, isEndRange) || TryParseYearAndMonthName(text, isEndRange) || TryParseYear(text) || TryParseMonth(text, isEndRange);
        }
    }
}
