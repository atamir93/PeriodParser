using System.Collections.Generic;
using System.Linq;

namespace PeriodParser
{
    public class MonthlyParser : ProfitAndLossParser
    {
        public MonthlyParser(string text = "") : base(text) { }

        public override bool Parse()
        {
            PeriodText = PeriodText.ToLower();
            Result = new Dictionary<string, object>();
            if (MonthDefinitions.Any(q => PeriodText.Contains(q)))
            {
                foreach (var quarterText in MonthDefinitions)
                {
                    PeriodText = PeriodText.ToLower().Replace(quarterText, "");
                }
            }
            Result.Add("Period", "Months");
            if (PeriodText.Contains(LastDefinition))
            {
                if (!TryParseWithLastDefinition(PeriodText))
                    return false;
            }
            else
            {
                var dateRanges = SplitByDash(PeriodText);
                if (dateRanges.Length == 2)
                {
                    if (!TryParseWitDateRange(PeriodText))
                        return false;
                }
                else
                {
                    Result.Add("Error", "");
                    return false;
                }
            }
            return true;
        }

        bool TryParseWitDateRange(string periodText)
        {
            var dateRanges = SplitByDash(periodText);
            var rangeFirst = dateRanges[0];
            var rangeSecond = dateRanges[1];

            bool hasFirstRangeMonth = StartsWithMonth(rangeFirst);
            bool hasSecondRangeMonth = StartsWithMonth(rangeSecond);

            if (hasFirstRangeMonth && hasSecondRangeMonth)
            {
                Result.Add("Type", "Consecutive");
                if (!TryParseRangeWithMonthAndYear(rangeFirst))
                    return false;
                if (!TryParseRangeWithMonthAndYear(rangeSecond))
                    return false;

            }
            else if (hasFirstRangeMonth)
            {
                Result.Add("Type", "EachYear");
                if (!TryParseRangeWithMonthAndYear(rangeFirst))
                    return false;
                if (!TryParseRangeWithYear(rangeSecond))
                    return false;

            }
            else
            {
                Result.Add("Error", "");
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
                    if (Result.ContainsKey("BeginMonth"))
                    {
                        Result.Add("EndingMonth", monthNumber);
                    }
                    else
                    {
                        Result.Add("BeginMonth", monthNumber);
                    }
                    var yearText = items[1];
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
                }
            }
            return true;
        }

        bool TryParseWithLastDefinition(string periodText)
        {
            if (YearDefinitions.Any(y => periodText.Contains(y)))
            {
                if (!TryParseToEachYearMonthlyWithLastDefinitions(periodText))
                    return false;
            }
            else
            {
                if (!TryParseToConsecutiveMonthlyWithLastDefinitions(periodText))
                    return false;
            }

            return true;
        }

        bool TryParseToEachYearMonthlyWithLastDefinitions(string periodText)
        {
            Result.Add("Type", "EachYear");
            if (periodText.Contains(ThisDefinition))
            {
                Result.Add("BeginMonth", CurrentMonth);
            }
            else
            {
                var possibleMonth = periodText.Trim().Split(" ").FirstOrDefault();
                if (possibleMonth != null)
                {
                    int monthNumber = GetMonthNumber(possibleMonth);
                    if (monthNumber == 0)
                    {
                        Result.Add("Error", "");
                        return false;
                    }
                    else
                    {
                        Result.Add("BeginMonth", monthNumber);
                        periodText = periodText.Replace(possibleMonth, "");
                    }
                }
            }

            if (!Result.ContainsKey("BeginMonth"))
            {
                Result.Add("Error", "");
                return false;
            }

            int yearDifference = GetFirstNumber(periodText);
            if (yearDifference == 0)
            {
                Result.Add(Error, "");
                return false;
            }
            else
            {
                Result.Add("BeginYear", CurrentYear - yearDifference);
                Result.Add("EndingYear", CurrentYear);
            }

            return true;
        }

        bool TryParseToConsecutiveMonthlyWithLastDefinitions(string periodText)
        {
            Result.Add("Type", "Consecutive");
            int monthDifference = GetFirstNumber(periodText);
            if (monthDifference == 0)
            {
                Result.Add(Error, "");
                return false;
            }
            else
            {
                var beginMonthAndYear = GetBeginMonthAndYearFromDifference(CurrentMonth, CurrentYear, monthDifference);
                Result.Add("BeginMonth", beginMonthAndYear.month);
                Result.Add("BeginYear", beginMonthAndYear.year);
                Result.Add("EndingMonth", CurrentMonth);
                Result.Add("EndingYear", CurrentYear);
            }

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
                Result.Add("EndingYear", year);
            }
            return true;
        }
    }
}
