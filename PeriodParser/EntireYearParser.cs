﻿using System.Collections.Generic;

namespace PeriodParser
{
    public class EntireYearParser : PeriodParser
    {
        private EntireYearParser() : base() { }
        private static EntireYearParser instance = null;
        public static EntireYearParser GetInstance()
        {
            if (instance == null)
            {
                instance = new EntireYearParser();
            }
            return instance;
        }
        public override bool Parse()
        {
            Result = new Dictionary<string, object>();
            foreach (var yearText in YearDefinitions)
            {
                PeriodText = PeriodText.ToLower().Replace(yearText, "");
            }
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
            int yearDifference = GetFirstNumber(periodText);
            if (yearDifference == 0)
            {
                Result.Add(Error, "");
                return false;
            }
            if (periodText.Contains(FiscalDefinition))
            {
                //Result.Add(YearlyPeriod, "Fiscal");
                // to-do for fiscal years
            }
            else
            {
                //Result.Add(YearlyPeriod, "Calendar");
                if (Result.ContainsKey(Month1))
                    Result.Add(Month2, CurrentMonth);
                else
                    Result.Add(Month1, CurrentMonth);
                Result.Add(Year1, CurrentYear - yearDifference);
                Result.Add(Year2, CurrentYear);
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
