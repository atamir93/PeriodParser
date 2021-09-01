using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PeriodParser.RegexParser
{
    public abstract class PeriodParserRegex
    {
        public const string Period = "Period";
        public const string Type = "Type";
        public const string Error = "Error";
        public const string DimensionName = "DimensionName";
        public const string DimensionPeriod = "DimensionCompareType";
        public const string Month1 = "Month1";
        public const string Month2 = "Month2";
        public const string Quarter1 = "Quarter1";
        public const string Quarter2 = "Quarter2";
        public const string Year1 = "Year1";
        public const string Year2 = "Year2";

        public int CurrentYear = 2020;
        public int CurrentMonth = 5;
        public int CurrentQuarter = 2;
        public readonly int FirstMonth = 1;
        public readonly int LastMonth = 12;

        public string CurrentPeriod { get; set; }
        public Dictionary<string, object> Result { get; set; }
        private string periodText;

        public string PeriodText
        {
            get { return periodText; }
            set { periodText = value.Trim().ToLower(); }
        }

        public void SetAllEndingFields()
        {
            if (Result.ContainsKey(Month1) && !Result.ContainsKey(Month2))
            {
                Result.Add(Month2, Result[Month1]);
            }

            if (Result.ContainsKey(Quarter1) && !Result.ContainsKey(Quarter2))
            {
                Result.Add(Quarter2, Result[Quarter1]);
            }

            if (Result.ContainsKey(Year1) && !Result.ContainsKey(Year2))
            {
                Result.Add(Year2, Result[Year1]);
            }
        }
        public void SetPeriodText(string text)
        {
            Result = new Dictionary<string, object>();
            PeriodText = text.ToLower().Trim();
        }
        public void SetCurrentDate(DateTime dateTime)
        {
            CurrentYear = dateTime.Year;
            CurrentMonth = dateTime.Month;
            CurrentQuarter = (int)Math.Ceiling(CurrentMonth / 3.0);
        }

        public abstract bool TryParse();
        internal abstract bool TryParseDateText(string text, bool isEndRange = false);
        internal bool TryParseDateRanges(int maxRanges = 2)
        {
            bool isValid = false;
            var dateRanges = GetDateRanges(PeriodText);
            for (int i = 0; i < Math.Min(maxRanges, dateRanges.Length); i++)
            {
                isValid = TryParseDateText(dateRanges[i]);
            }
            return isValid;
        }

        internal bool TryParseDateRangesConsideringEndingRange(int maxRanges = 2)
        {
            bool isValid = false;
            var dateRanges = GetDateRanges(PeriodText);
            bool isEndingRange = false;
            for (int i = 0; i < Math.Min(maxRanges, dateRanges.Length); i++)
            {
                if (i > 0)
                    isEndingRange = true;
                isValid = TryParseDateText(dateRanges[i], isEndingRange);
            }
            return isValid;
        }

        #region Regex patterns

        Regex GetRegexForYear() => new Regex(@"(\d+)");
        Regex GetRegexForMonthNumber() => new Regex(@"\s*(0?[1-9]|1[012])$");
        Regex GetRegexForMonthName() => new Regex(@"\s*(jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec)");
        Regex GetRegexForQuarterNumber() => new Regex(@"\s*q([1-4])$");
        Regex GetRegexForMonthNameAndYear() => new Regex(@"\s*(jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec)\D*(\d+)");
        Regex GetRegexForMonthNumberAndYear() => new Regex(@"(\d+)\D+(\d+)");
        Regex GetRegexForQuarterNumberAndYear() => new Regex(@"\s*q([1-4])\D*(\d+)");

        #endregion

        #region Parsers to year, month and quarter

        internal bool TryParseMonthAndYear(string text, bool isEndRange = false)
        {
            Regex rgx = GetRegexForMonthNameAndYear();
            Match match = rgx.Match(text);
            string monthText = "";
            string yearText = "";
            if (match.Success)
            {
                monthText = match.Groups[1].Value;
                yearText = match.Groups[2].Value;
            }
            else
            {
                rgx = GetRegexForMonthNumberAndYear();
                match = rgx.Match(text);
                if (match.Success)
                {
                    monthText = match.Groups[1].Value;
                    yearText = match.Groups[2].Value;
                }
            }
            return TryAddMonthToResult(monthText, isEndRange) && TryAddYearToResult(yearText);
        }

        internal bool TryParseQuarterAndYear(string text, bool isEndRange = false)
        {
            Regex rgx = GetRegexForQuarterNumberAndYear();
            Match match = rgx.Match(text);
            if (match.Success)
            {
                var quarterNumberText = match.Groups[1].Value;
                var yearText = match.Groups[2].Value;
                return TryAddQuarterToResult(quarterNumberText, isEndRange) && TryAddYearToResult(yearText);
            }
            return false;
        }

        internal bool TryParseYear(string text)
        {
            Regex rgx = GetRegexForYear();
            Match match = rgx.Match(text);
            if (match.Success)
            {
                var yearText = match.Groups[1].Value;
                return TryAddYearToResult(yearText);
            }
            return false;
        }

        internal bool TryParseMonth(string text, bool isEndRange = false)
        {
            Regex rgx = GetRegexForMonthName();
            Match match = rgx.Match(text);
            string monthText = "";
            if (match.Success)
            {
                monthText = match.Groups[1].Value;
            }
            else
            {
                rgx = GetRegexForMonthNumber();
                match = rgx.Match(text);
                if (match.Success)
                    monthText = match.Groups[1].Value;
            }

            return TryAddMonthToResult(monthText, isEndRange);
        }

        internal bool TryParseQuarter(string text, bool isEndRange = false)
        {
            Regex rgx = GetRegexForQuarterNumber();
            Match match = rgx.Match(text);
            if (match.Success)
            {
                var quarterNumberText = match.Groups[1].Value;
                return TryAddQuarterToResult(quarterNumberText, isEndRange);
            }
            return false;
        }

        #endregion

        #region Helper functions

        bool TryAddQuarterToResult(string quarterText, bool isEndRange = false)
        {
            bool success = false;
            var quarterNumber = GetQuarterNumber(quarterText);
            if (quarterNumber != 0)
            {
                if (isEndRange || Result.ContainsKey(Quarter1))
                    Result.Add(Quarter2, quarterNumber);
                else
                    Result.Add(Quarter1, quarterNumber);

                success = true;
            }
            return success;
        }

        bool TryAddMonthToResult(string monthText, bool isEndRange = false)
        {
            bool success = false;
            var monthNumber = GetMonthNumber(monthText);
            if (monthNumber != 0)
            {
                if (isEndRange || Result.ContainsKey(Month1))
                    Result.Add(Month2, monthNumber);
                else
                    Result.Add(Month1, monthNumber);

                success = true;
            }
            return success;
        }

        bool TryAddYearToResult(string yearText)
        {
            bool success = false;
            var year = GetYearNumber(yearText);
            if (!string.IsNullOrEmpty(year))
            {
                if (Result.ContainsKey(Year1))
                    Result.Add(Year2, year);
                else
                    Result.Add(Year1, year);

                success = true;
            }
            return success;
        }

        internal int GetQuarterNumber(string text)
        {
            text = text.Replace("q", "");
            int quarterNumber = 0;
            if (int.TryParse(text, out quarterNumber))
            {
                if (quarterNumber < 1 || quarterNumber > 4)
                {
                    Result.Add(Error, "");
                    return 0;
                }
            }
            return quarterNumber;
        }

        internal int GetMonthNumber(string text)
        {
            if (string.IsNullOrEmpty(text)) return 0;

            int monthNumber = 0;
            if (int.TryParse(text, out monthNumber))
            {
                if (monthNumber < 1 || monthNumber > 12)
                {
                    Result.Add(Error, "");
                    return 0;
                }
            }
            else
            {
                var shortName = text.Substring(0, 3);
                DateTime result;
                if (DateTime.TryParseExact(shortName, "MMM", CultureInfo.CurrentCulture, DateTimeStyles.None, out result))
                    monthNumber = result.Month;
            }
            return monthNumber;
        }

        internal string[] GetDateRanges(string text) => text.Split("-");

        string GetYearNumber(string text)
        {
            string year = "";
            if (text.Length <= 4)
            {
                var year2000 = "2000";
                year = year2000.Remove(year2000.Length - text.Length) + text;
            }
            return year;
        }

        internal (int year, int month) GetBeginMonthAndYearFromDifference(int endingMonth, int endingYear, int monthlyPeriodDifference)
        {
            var yearDiff = monthlyPeriodDifference / 12;
            var difference = monthlyPeriodDifference % 12;
            var beginMonth = endingMonth - difference + 1;
            if (beginMonth <= 0)
            {
                yearDiff++;
                beginMonth += 12;
            }

            return (endingYear - yearDiff, beginMonth);
        }

        internal (int year, int quarter) GetBeginQuarterAndYearFromDifference(int endingQuarter, int endingYear, int quarterlyPeriodDifference)
        {
            var yearDiff = quarterlyPeriodDifference / 4;
            var difference = quarterlyPeriodDifference % 4;
            var beginQuarter = endingQuarter - difference + 1;
            if (beginQuarter <= 0)
            {
                yearDiff++;
                beginQuarter += 4;
            }

            return (endingYear - yearDiff, beginQuarter);
        }

        #endregion
    }
}
