using System.Collections.Generic;

namespace PeriodParser
{
    public class EntireYearParser : ProfitAndLossParser
    {
        public EntireYearParser(string text = "") : base(text) { }

        public override bool Parse()
        {
            Result = new Dictionary<string, object>();
            foreach (var yearText in YearDefinitions)
            {
                PeriodText = PeriodText.ToLower().Replace(yearText, "");
            }
            Result.Add(Period, "Yearly");
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
            int yearDifference = GetFirstNumber(periodText);
            if (yearDifference == 0)
            {
                Result.Add(Error, "");
                return false;
            }
            if (periodText.Contains(FiscalDefinition))
            {
                Result.Add(YearlyPeriod, "Fiscal");
                // to-do for fiscal years
            }
            else
            {
                Result.Add(YearlyPeriod, "Calendar");
                Result.Add(EndingMonth, CurrentMonth);
                Result.Add(BeginYear, CurrentYear - yearDifference);
                Result.Add(EndingYear, CurrentYear);
            }
            return true;
        }

        bool TryParseToYearWithDateRange(string periodText)
        {
            var dateRanges = SplitByDash(periodText);
            var rangeFirst = dateRanges[0];
            var rangeSecond = dateRanges[1];

            if (HasOnlyYear(rangeFirst))
            {
                Result.Add(EndingMonth, CurrentMonth);
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
            //TODO when year is 2100 or >
            string year = GetYear(yearText.Trim());
            if (string.IsNullOrEmpty(year))
            {
                Result.Add("Error", "");
                return false;
            }
            else
            {
                if (Result.ContainsKey("BeginYear"))
                    Result.Add("EndingYear", year);
                else
                    Result.Add("BeginYear", year);
            }

            return true;
        }

        bool TryParseRangeWithMonthAndYear(string monthAndYearText)
        {
            var withoutCharactersExceptPipe = ReplaceCharactersExceptPipeToEmptySpace(monthAndYearText);
            string[] items = SplitByEmptySpace(withoutCharactersExceptPipe);
            if (items.Length < 2)
            {
                Result.Add("Error", "");
                return false;
            }
            else
            {
                var month = items[0];
                int monthNumber = GetMonthNumber(month);
                if (monthNumber == 0)
                {
                    Result.Add("Error", "");
                    return false;
                }
                else
                {
                    Result.Add("EndingMonth", monthNumber);
                    var yearText = items[1];
                    string year = GetYear(yearText.Trim());
                    if (string.IsNullOrEmpty(year))
                    {
                        Result.Add("Error", "");
                        return false;
                    }
                    else
                    {
                        Result.Add("BeginYear", year);
                    }
                }
            }
            return true;
        }
    }
}
