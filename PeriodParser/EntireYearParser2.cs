using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PeriodParser
{
    public class EntireYearParser2 : PeriodParser
    {
        private EntireYearParser2() : base() { }
        private static EntireYearParser2 instance = null;
        public static EntireYearParser2 GetInstance()
        {
            if (instance == null)
            {
                instance = new EntireYearParser2();
            }
            return instance;
        }
        public override bool Parse()
        {
            Result = new Dictionary<string, object>();
            Result.Add(Period, ProfitAndLossPeriod.Yearly);
            Result.Add(Type, "EntireYear");
            if (PeriodText.Contains(LastDefinition))
            {
                if (!TryParseToYearWithLastDefinition(PeriodText))
                    return false;
            }
            else
            {
                var dateRanges = SplitByDash(PeriodText);
                if (dateRanges.Length == 1)
                {
                    return false;
                }
                else if (dateRanges.Length == 2)
                {
                    if (!TryParseToYearWithDateRange(PeriodText))
                        return false;
                }
                else
                {
                    Result.Add(Error, "");
                    return false;
                }
            }
            return true;
        }

        bool TryParseToYearWithLastDefinition(string periodText)
        {
            Regex rgx = new Regex(@"last\s+(\d+)\s+years");
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

        bool TryParseToYearWithDateRange(string periodText)
        {
            var dateRanges = SplitByDash(periodText);
            var rangeFirst = dateRanges[0];
            var rangeSecond = dateRanges[1];

            if (HasOnlyYear(rangeFirst))
            {
                if (Result.ContainsKey(Month1))
                    Result.Add(Month2, CurrentMonth);
                else
                    Result.Add(Month1, CurrentMonth);
                if (!TryParseRangeWithYear(rangeFirst))
                    return false;
            }
            else
            {
                if (!TryParseRangeWithMonthAndYear(rangeFirst))
                    return false;
            }

            if (!TryParseRangeWithYear(rangeSecond))
                return false;

            return true;
        }

        private bool TryParseRangeWithYear(string yearText)
        {
            //what if year is 2100 or >
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

            return true;
        }

        bool TryParseRangeWithMonthAndYear(string monthAndYearText)
        {
            var withoutCharactersExceptPipe = ReplaceCharactersExceptPipeToEmptySpace(monthAndYearText);
            string[] items = SplitByEmptySpace(withoutCharactersExceptPipe);
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
                        Result.Add(Month2, monthNumber);
                    else
                        Result.Add(Month1, monthNumber);
                    var yearText = items[1];
                    string year = GetYear(yearText.Trim());
                    if (string.IsNullOrEmpty(year))
                    {
                        Result.Add(Error, "");
                        return false;
                    }
                    else
                    {
                        Result.Add(Year1, year);
                    }
                }
            }
            return true;
        }
    }
}
