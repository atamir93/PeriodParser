using System.Text.RegularExpressions;

namespace PeriodParser
{
    public class ParserToPeriodTextBL
    {
        static void ChangeToMonthShortName(ref string monthName)
        {
            if (monthName.Length > 3)
                monthName = monthName.Substring(0, 3);
        }
        public static string GetEntireYearYearlyPeriodText(string beginYear, string endingYear)
        {
            string years = beginYear == endingYear ? beginYear : $"{beginYear} - {endingYear}";
            string periodText = $"{years} Yearly";
            return periodText;
        }

        public static string GetYearToDateYearlyPeriodText(string month, string beginYear, string endingYear)
        {
            ChangeToMonthShortName(ref month);
            string type = "YTD";
            string years = beginYear == endingYear ? beginYear : $"{beginYear} - {endingYear}";
            string periodText;
            if (string.IsNullOrEmpty(month))
                periodText = $"{years} {type}";
            else
                periodText = $"{years} {month} {type}";
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
            ChangeToMonthShortName(ref beginMonth);
            ChangeToMonthShortName(ref endingMonth);
            string periodText;
            if (beginYear == endingYear)
                periodText = $"{beginMonth} - {endingMonth} {beginYear}";
            else
                periodText = $"{beginMonth}, {beginYear} - {endingMonth}, {endingYear}";
            return $"{periodText} Monthly";
        }

        public static string GetEachYearMonthlyPeriodText(string beginMonth, string beginYear, string endingYear)
        {
            ChangeToMonthShortName(ref beginMonth);
            string periodText;
            if (beginYear == endingYear)
                periodText = $"{beginMonth} {beginYear}";
            else
                periodText = $"{beginMonth} {beginYear} - {endingYear}";
            return $"{periodText} Monthly";
        }

        public static string GetMonthRangePeriodText(string beginMonth, string endingMonth, string beginYear, string endingYear)
        {
            ChangeToMonthShortName(ref beginMonth);
            ChangeToMonthShortName(ref endingMonth);
            string months = beginMonth == endingMonth ? endingMonth : $"{beginMonth} - {endingMonth}";
            string years = beginYear == endingYear ? beginYear : $"{beginYear} - {endingYear}";

            return $"{months} {years} Seasons";
        }

        public static string GetSinglePeriodText(string beginMonth, string endingMonth, string beginYear, string endingYear)
        {
            ChangeToMonthShortName(ref beginMonth);
            ChangeToMonthShortName(ref endingMonth);
            string periodText;
            if (beginYear == endingYear)
                periodText = $"{beginMonth} - {endingMonth} {beginYear} Total";
            else
                periodText = $"{beginMonth}, {beginYear} - {endingMonth}, {endingYear} Total";
            return periodText;
        }

        public static string GetEntireYearDimensionPeriodText(string year, string dimensionName) => $"{year} by {dimensionName}";
        public static string GetYearToDateDimensionPeriodText(string month, string year, string dimensionName)
        {
            ChangeToMonthShortName(ref month);
            return $"{month} {year} YTD by {dimensionName}";
        }
        public static string GetQuarterDimensionPeriodText(string quarter, string year, string dimensionName)
        {
            if (IsNumeric(quarter))
                quarter = $"Q{quarter}";
            return $"{quarter} {year} by {dimensionName}";
        }
        public static string GetMonthDimensionPeriodText(string month, string year, string dimensionName)
        {
            ChangeToMonthShortName(ref month);
            return $"{month} {year} by {dimensionName}";
        }
        public static string GetRangeDimensionPeriodText(string beginMonth, string endingMonth, string beginYear, string endingYear, string dimensionName)
        {
            ChangeToMonthShortName(ref beginMonth);
            ChangeToMonthShortName(ref endingMonth);
            return $"{beginMonth} {beginYear} - {endingMonth} {endingYear} by {dimensionName}";
        }
    }
}
