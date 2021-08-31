using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PeriodParser
{
    public class DimensionParserRegex : PeriodParserRegex
    {
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

                if (dateText.Contains("-"))
                {
                    TryParseRange(dateText);
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
            var dateRanges = SplitByDash(PeriodText);
            if (dateRanges.Length == 1)
            {
                if (TryParse(PeriodText))
                    isValid = true;
            }
            else if (dateRanges.Length == 2)
            {
                if (TryParse(dateRanges[0]))
                {
                    if (TryParse(dateRanges[1], true))
                        isValid = true;
                }
            }
            if (isValid)
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
            }
            return isValid;
        }

        bool TryParse(string text, bool isEndRange = false)
        {
            if (TryParseMonthNameAndYear(text, isEndRange))
            {
                return true;
            }
            else if (TryParseMonthNumberAndYear(text, isEndRange))
            {
                return true;
            }
            else if (TryParseYear(text))
            {
                return true;
            }
            else if (TryParseMonth(text, isEndRange))
            {
                return true;
            }
            return false;
        }

        //protected bool TryParseQuarterlyDimension(string text, bool isEndRange = false)
        //{
        //    Regex rgx = new Regex(@"\s*q([1-4])\D*(\d+)\s*by\s*(.*)\s*");
        //    Match match = rgx.Match(text);
        //    if (match.Success)
        //    {
        //        var quarterNumberText = match.Groups[1].Value;
        //        var yearText = match.Groups[2].Value;
        //        var dimensionName = match.Groups[3].Value;
        //        var quarterNumber = GetQuarterNumber(quarterNumberText);
        //        if (quarterNumber != 0)
        //        {
        //            if (isEndRange || Result.ContainsKey(Quarter1))
        //                Result.Add(Quarter2, quarterNumber);
        //            else
        //                Result.Add(Quarter1, quarterNumber);
        //            var year = GetYearNumber(yearText);
        //            if (!string.IsNullOrEmpty(year))
        //            {
        //                if (Result.ContainsKey(Year1))
        //                    Result.Add(Year2, year);
        //                else
        //                    Result.Add(Year1, year);

        //                Result.Add(DimensionName, dimensionName);
        //                Result.Add(DimensionPeriod, DimensionCompareType.Quarter);

        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}

        //protected bool TryParseEntireYearDimension(string text)
        //{
        //    Regex rgx = new Regex(@"^(\d+)\s+by\s*(.*)\s*");
        //    Match match = rgx.Match(text);
        //    if (match.Success)
        //    {
        //        var yearText = match.Groups[1].Value;
        //        var dimensionName = match.Groups[2].Value;
        //        var year = GetYearNumber(yearText);
        //        if (!string.IsNullOrEmpty(year))
        //        {
        //            if (Result.ContainsKey(Year1))
        //                Result.Add(Year2, year);
        //            else
        //                Result.Add(Year1, year);

        //            Result.Add(DimensionName, dimensionName);
        //            Result.Add(DimensionPeriod, DimensionCompareType.EntireYear);

        //            return true;
        //        }
        //    }
        //    return false;
        //}
        //protected bool TryParseYearToDateDimension(string text)
        //{
        //    if (TryParseYearToDateWithMonthNameDimension(text))
        //        return true;
        //    if (TryParseYearToDateWithMonthNumberDimension(text))
        //        return true;
        //    return false;
        //}
        //protected bool TryParseMonthlyDimension(string text)
        //{
        //    if (TryParseMonthNameDimension(text))
        //        return true;
        //    if (TryParseMonthNumberDimension(text))
        //        return true;
        //    return false;
        //}

        //protected bool TryParseYearToDateWithMonthNumberDimension(string text)
        //{
        //    Regex rgx = new Regex(@"\s*(\d+)\D+(\d+)\s+by\s*ytd\s*(.*)\s*");
        //    Match match = rgx.Match(text);
        //    if (match.Success)
        //    {
        //        var monthNumberText = match.Groups[1].Value;
        //        var yearText = match.Groups[2].Value;
        //        var dimensionName = match.Groups[3].Value;
        //        var monthNumber = GetMonthNumber(monthNumberText);
        //        if (monthNumber != 0)
        //        {
        //            if (Result.ContainsKey(Month1))
        //                Result.Add(Month2, monthNumber);
        //            else
        //                Result.Add(Month1, monthNumber);
        //            var year = GetYearNumber(yearText);
        //            if (!string.IsNullOrEmpty(year))
        //            {
        //                if (Result.ContainsKey(Year1))
        //                    Result.Add(Year2, year);
        //                else
        //                    Result.Add(Year1, year);

        //                Result.Add(DimensionName, dimensionName);
        //                Result.Add(DimensionPeriod, DimensionCompareType.YearToDate);
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}
        //protected bool TryParseYearToDateWithMonthNameDimension(string text)
        //{
        //    Regex rgx = new Regex(@"\s*(jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec)\D*(\d+)\s+by\s*ytd\s*(.*)\s*");
        //    Match match = rgx.Match(text);
        //    if (match.Success)
        //    {
        //        var monthNumberText = match.Groups[1].Value;
        //        var yearText = match.Groups[2].Value;
        //        var dimensionName = match.Groups[3].Value;
        //        var monthNumber = GetMonthNumber(monthNumberText);
        //        if (monthNumber != 0)
        //        {
        //            if (Result.ContainsKey(Month1))
        //                Result.Add(Month2, monthNumber);
        //            else
        //                Result.Add(Month1, monthNumber);
        //            var year = GetYearNumber(yearText);
        //            if (!string.IsNullOrEmpty(year))
        //            {
        //                if (Result.ContainsKey(Year1))
        //                    Result.Add(Year2, year);
        //                else
        //                    Result.Add(Year1, year);

        //                Result.Add(DimensionName, dimensionName);
        //                Result.Add(DimensionPeriod, DimensionCompareType.YearToDate);
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}

        //protected bool TryParseMonthNumberDimension(string text)
        //{
        //    Regex rgx = new Regex(@"\s*(\d+)\D+(\d+)\s+by\s*(.*)\s*");
        //    Match match = rgx.Match(text);
        //    if (match.Success)
        //    {
        //        var monthNumberText = match.Groups[1].Value;
        //        var yearText = match.Groups[2].Value;
        //        var dimensionName = match.Groups[3].Value;
        //        var monthNumber = GetMonthNumber(monthNumberText);
        //        if (monthNumber != 0)
        //        {
        //            if (Result.ContainsKey(Month1))
        //                Result.Add(Month2, monthNumber);
        //            else
        //                Result.Add(Month1, monthNumber);
        //            var year = GetYearNumber(yearText);
        //            if (!string.IsNullOrEmpty(year))
        //            {
        //                if (Result.ContainsKey(Year1))
        //                    Result.Add(Year2, year);
        //                else
        //                    Result.Add(Year1, year);

        //                Result.Add(DimensionName, dimensionName);
        //                Result.Add(DimensionPeriod, DimensionCompareType.YearToDate);
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}
        //protected bool TryParseMonthNameDimension(string text)
        //{
        //    Regex rgx = new Regex(@"\s*(jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec)\D*(\d+)\s+by\s*(.*)\s*");
        //    Match match = rgx.Match(text);
        //    if (match.Success)
        //    {
        //        var monthNumberText = match.Groups[1].Value;
        //        var yearText = match.Groups[2].Value;
        //        var dimensionName = match.Groups[3].Value;
        //        var monthNumber = GetMonthNumber(monthNumberText);
        //        if (monthNumber != 0)
        //        {
        //            if (Result.ContainsKey(Month1))
        //                Result.Add(Month2, monthNumber);
        //            else
        //                Result.Add(Month1, monthNumber);
        //            var year = GetYearNumber(yearText);
        //            if (!string.IsNullOrEmpty(year))
        //            {
        //                if (Result.ContainsKey(Year1))
        //                    Result.Add(Year2, year);
        //                else
        //                    Result.Add(Year1, year);

        //                Result.Add(DimensionName, dimensionName);
        //                Result.Add(DimensionPeriod, DimensionCompareType.Month);
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}
    }
}
