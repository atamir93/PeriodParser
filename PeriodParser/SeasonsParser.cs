﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PeriodParser
{
    public class SeasonsParser : PeriodParser
    {
        public SeasonsParser(string text = "") : base(text) { }
        private static SeasonsParser instance = null;
        public static SeasonsParser GetInstance(string text)
        {
            if (instance == null)
            {
                instance = new SeasonsParser();
            }
            instance.SetPeriodText(text);
            return instance;
        }
        public override bool Parse()
        {
            PeriodText = PeriodText.ToLower().Trim();
            Result = new Dictionary<string, object>();
            if (SeasonsDefinitions.Any(q => PeriodText.Contains(q)))
            {
                foreach (var seasonText in SeasonsDefinitions)
                {
                    PeriodText = PeriodText.ToLower().Replace(seasonText, "");
                }
            }

            Result.Add(Period, "Seasons");
            PeriodText = ReplaceCharactersExceptPipeAndDashToEmptySpace(PeriodText.Trim());
            var dateRanges = SplitByDash(PeriodText);
            if (dateRanges.Length == 3)
            {
                if (!TryParseFullRange(PeriodText))
                    return false;
            }
            else if (dateRanges.Length == 2)
            {
                if (!TryParseRangeWithSingleYear(PeriodText))
                    return false;
            }
            else
            {
                Result.Add(Error, "");
                return false;
            }

            return true;
        }

        bool TryParseRangeWithSingleYear(string monthMonthYearYearText)
        {
            var dateRanges = SplitByDash(monthMonthYearYearText);

            var monthItem = dateRanges[0].Trim();
            if (!TryParseRangeWithMonth(monthItem))
                return false;

            var monthAndYearItems = dateRanges[1].Trim();
            if (TryParseRangeWithMonthAndYear(monthAndYearItems))
            {
                if (Result.ContainsKey(Year1))
                {
                    Result.Add(Year2, Result["Year1"]);
                }
                else
                    return false;
            }
            else
            {
                return false;
            }

            return true;
        }

        bool TryParseFullRange(string monthMonthYearYearText)
        {
            var dateRanges = monthMonthYearYearText.Split("-");

            var monthItem = dateRanges[0].Trim();
            if (!TryParseRangeWithMonth(monthItem))
                return false;

            var monthAndYearItems = dateRanges[1].Trim();
            if (!TryParseRangeWithMonthAndYear(monthAndYearItems))
                return false;

            var yearItem = dateRanges[2].Trim();
            if (!TryParseRangeWithYear(yearItem))
                return false;

            return true;
        }

        bool TryParseRangeWithMonth(string monthText)
        {
            int monthNumber = GetMonthNumber(monthText);
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
                        {
                            Result.Add(Year2, year);
                        }
                        else
                        {
                            Result.Add(Year1, year);
                        }
                    }

                }
            }
            return true;
        }

        private bool TryParseRangeWithYear(string yearText)
        {
            //TODO when year is 2100 or >
            string year = GetYear(yearText.Trim());
            if (string.IsNullOrEmpty(year))
            {
                Result.Add(Error, "");
                return false;
            }
            else
            {
                Result.Add(Year2, year);
            }
            return true;
        }
    }
}
