using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PeriodParser
{
    public class ParserToPeriodTextBL
    {
        public static string GetYearlyPeriodText(string month, string type, string beginYear, string endingYear)
        {
            string years = beginYear == endingYear ? beginYear : $"{beginYear} - {endingYear}";
            string periodText = $"{month} {years} {type}";
            return periodText;
        }



        public static string GetConsecutiveQuarterlyPeriodText(string beginQuarter, string endingQuarter, string beginYear, string endingYear)
        {
            if (IsNumeric(beginQuarter))
                beginQuarter = $"Q{beginQuarter}";
            if (IsNumeric(endingQuarter))
                endingQuarter = $"Q{endingQuarter}";

            string periodText;
            if (beginYear == endingYear)
                periodText = $"{beginQuarter} - {endingQuarter} {beginYear}";
            else
                periodText = $"{beginQuarter}, {beginYear} - {endingQuarter}, {endingYear}";
            return periodText;
        }

        public static string GetEachYearQuarterlyPeriodText(string quarter, string beginYear, string endingYear)
        {
            if (IsNumeric(quarter))
                quarter = $"Q{quarter}";

            string periodText;
            if (beginYear == endingYear)
                periodText = $"{quarter} {beginYear}";
            else
                periodText = $"{quarter} {beginYear} - {endingYear}";
            return periodText;
        }

        static bool IsNumeric(string text)
        {
            return Regex.IsMatch(text, @"^\d+$");
        }

        public static string GetConsecutiveMonthlyPeriodText(string beginMonth, string endingMonth, string beginYear, string endingYear)
        {
            string periodText;
            if (beginYear == endingYear)
                periodText = $"{beginMonth} - {endingMonth} {beginYear}";
            else
                periodText = $"{beginMonth}, {beginYear} - {endingMonth}, {endingYear}";
            return $"{periodText} Monthly";
        }

        public static string GetEachYearMonthlyPeriodText(string beginMonth, string beginYear, string endingYear)
        {
            string periodText;
            if (beginYear == endingYear)
                periodText = $"{beginMonth} {beginYear}";
            else
                periodText = $"{beginMonth} {beginYear} - {endingYear}";
            return $"{periodText} Monthly";
        }

        public static string GetMonthRangePeriodText(string beginMonth, string endingMonth, string beginYear, string endingYear)
        {
            string months = beginMonth == endingMonth ? endingMonth : $"{beginMonth} - {endingMonth}";
            string years = beginYear == endingYear ? beginYear : $"{beginYear} - {endingYear}";

            return $"{months} {years} Seasons";
        }

        public static string GetSinglePeriodText(string beginMonth, string endingMonth, string beginYear, string endingYear)
        {
            string periodText = $"{beginMonth}, {beginYear} - {endingMonth}, {endingYear} Total";
            return periodText;
        }

        public static string GetEntireYearDimensionPeriodText(string year, string dimensionName) => $"{year} by {dimensionName}";
        public static string GetYearToDateDimensionPeriodText(string month, string year, string dimensionName) => $"{month} {year} YTD by {dimensionName}";
        public static string GetQuarterDimensionPeriodText(string quarter, string year, string dimensionName)
        {
            if (IsNumeric(quarter))
                quarter = $"Q{quarter}";
            return $"{quarter} {year} by {dimensionName}";
        }
        public static string GetMonthDimensionPeriodText(string month, string year, string dimensionName) => $"{month} {year} by {dimensionName}";
        public static string GetRangeDimensionPeriodText(string beginMonth, string endingMonth, string beginYear, string endingYear, string dimensionName) => $"{beginMonth} {beginYear} - {endingMonth} {endingYear} by {dimensionName}";
    }
}
