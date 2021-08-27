using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PeriodParser
{
    public class TotalParser : ProfitAndLossParser
    {
        public TotalParser(string text = "") : base(text) { }

        public override bool Parse()
        {
            PeriodText = PeriodText.ToLower().Trim();
            Result = new Dictionary<string, object>();
            if (TotalDefinitions.Any(q => PeriodText.Contains(q)))
            {
                foreach (var seasonText in TotalDefinitions)
                {
                    PeriodText = PeriodText.ToLower().Replace(seasonText, "");
                }
            }

            Result.Add(Period, "Total");

            var dateRanges = PeriodText.Split("-");
            if (dateRanges.Length == 2)
            {
                if (!TryParseWitDateRange(PeriodText))
                    return false;
            }
            else if (dateRanges.Length == 1)
            {
                if (!TryParseWithoutRange(PeriodText))
                    return false;
            }
            else
            {
                Result.Add(Error, "");
                return false;
            }

            return true;
        }

        bool TryParseWithoutRange(string yearText)
        {
            var year = GetYear(yearText);
            if (string.IsNullOrEmpty(year))
            {
                Result.Add(Error, "");
                return false;
            }
            else
            {
                Result.Add(Month1, FirstMonth);
                Result.Add(Month2, LastMonth);
                Result.Add(Year1, year);
                Result.Add(Year2, year);
            }

            return true;
        }

        bool TryParseWitDateRange(string periodText)
        {
            var dateRanges = periodText.Split("-");
            var rangeFirst = dateRanges[0].Trim();
            var rangeSecond = dateRanges[1].Trim();

            bool hasFirstRangeMonth = StartsWithMonth(rangeFirst);
            bool hasSecondRangeMonth = StartsWithMonth(rangeSecond);

            if (hasFirstRangeMonth && hasSecondRangeMonth)
            {
                if (!TryParseRangeWithMonthAndYear(rangeFirst))
                    return false;
                if (!TryParseRangeWithMonthAndYear(rangeSecond))
                    return false;

            }
            else if (hasFirstRangeMonth)
            {
                if (!TryParseRangeWithMonthAndYear(rangeFirst))
                    return false;
                if (!TryParseRangeWithYear(rangeSecond))
                    return false;
            }
            else if (hasSecondRangeMonth)
            {
                if (!TryParseRangeWithYear(rangeFirst))
                    return false;
                if (!TryParseRangeWithMonthAndYear(rangeSecond))
                    return false;
            }
            else
            {
                if (!TryParseRangeWithYear(rangeFirst))
                    return false;
                if (!TryParseRangeWithYear(rangeSecond))
                    return false;
            }

            return true;
        }

        bool TryParseRangeWithMonthAndYear(string monthAndYearText)
        {
            var withoutCharactersExceptPipe = ReplaceCharactersExceptPipeToEmptySpace(monthAndYearText.Trim());
            string[] items = withoutCharactersExceptPipe.Split(" ");
            if (items.Length < 2)
            {
                Result.Add(Error, "");
                return false;
            }
            else
            {
                var month = items[0];
                int monthNumber = GetMonthNumber(month);
                if (monthNumber == 0)
                {
                    Result.Add(Error, "");
                    return false;
                }
                else
                {
                    if (Result.ContainsKey(Month1))
                    {
                        Result.Add(Month2, monthNumber);
                    }
                    else
                    {
                        Result.Add(Month1, monthNumber);
                    }
                    var yearText = items[1];
                    string year = GetYear(yearText.Trim());
                    if (string.IsNullOrEmpty(year))
                    {
                        Result.Add(Error, "");
                        return false;
                    }
                    else
                    {
                        if (Result.ContainsKey(Year1))
                            Result.Add(Year2, year);
                        else
                            Result.Add(Year1, year);
                    }
                }
            }
            return true;
        }
        private bool TryParseRangeWithYear(string yearText)
        {
            //TODO when year is 2100 or >
            yearText = yearText.Trim();
            string year = GetYear(yearText.Trim());
            if (string.IsNullOrEmpty(year))
            {
                Result.Add(Error, "");
                return false;
            }
            else
            {
                if (Result.ContainsKey(Year1))
                {
                    Result.Add(Year2, year);
                }
                else
                {
                    Result.Add(Year1, year);
                }
                if (Result.ContainsKey(Month1))
                {
                    Result.Add(Month2, 12);
                }
                else
                {
                    Result.Add(Month1, 1);
                }
            }
            
            return true;
        }
    }
}
