using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace PeriodParser
{
    public abstract class PeriodParserRegex
    {
        public const string ThisDefinition = "this";
        public const string DimensionDefinition = "by";
        public const string YearToDateDefinition = "ytd";
        public const string LastDefinition = "last";
        public const string FiscalDefinition = "fiscal";
        public readonly string[] YearDefinitions = { "yearly", "years", "year" };
        public readonly string[] QuarterDefinitions = { "quarterly", "quarters", "quarter" };
        public readonly string[] QuarterNumbers = { "q1", "q2", "q3", "q4" };
        public readonly string[] MonthDefinitions = { "monthly", "months", "month" };
        public readonly string[] TotalDefinitions = { "totals", "total" };
        public readonly string[] SeasonsDefinitions = { "seasons", "season" };
        public readonly string[] MonthsFullNames = { "january", "february", "march", "april", "may", "june", "july", "august", "september", "october", "november", "december" };
        public readonly string[] MonthsShortNames = { "jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec" };
        public readonly string[] MonthsNumbers = { "1","01", "2", "02", "3", "03", "4", "04", "5", "05", "6", "06",
            "7", "07", "8", "08", "9", "09", "10", "11", "12" };

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

        protected Regex GetRegexForMonthNameAndYear()
        {
            return new Regex(@"\s*(jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec)\D*(\d+)");
        }

        protected Regex GetRegexForMonthNumberAndYear()
        {
            return new Regex(@"(\d+)\D+(\d+)");
        }
        protected Regex GetRegexForQuarterAndYear()
        {
            return new Regex(@"\s*q([1-4])\D*(\d+)");
        }

        protected Regex GetRegexForYear()
        {
            return new Regex(@"(\d+)");
        }

        protected Regex GetRegexForMonthNumber()
        {
            return new Regex(@"\s*(0?[1-9]|1[012])$");
        }
        protected Regex GetRegexForQuarterNumber()
        {
            return new Regex(@"\s*q([1-4])$");
        }

        protected Regex GetRegexForMonthName()
        {
            return new Regex(@"\s*(jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec)");
        }

        protected bool TryParseMonthNumberAndYear(string text, bool isEndRange = false)
        {
            Regex rgx = GetRegexForMonthNumberAndYear();
            Match match = rgx.Match(text);
            if (match.Success)
            {
                var monthNumberText = match.Groups[1].Value;
                var yearText = match.Groups[2].Value;
                var monthNumber = GetMonthNumber(monthNumberText);
                if (monthNumber != 0)
                {
                    if (isEndRange || Result.ContainsKey(Month1))
                        Result.Add(Month2, monthNumber);
                    else
                        Result.Add(Month1, monthNumber);
                    var year = GetYearNumber(yearText);
                    if (!string.IsNullOrEmpty(year))
                    {
                        if (Result.ContainsKey(Year1))
                            Result.Add(Year2, year);
                        else
                            Result.Add(Year1, year);

                        return true;
                    }
                }
            }
            return false;
        }

        protected bool TryParseMonthAndYear(string text, bool isEndRange = false)
        {
            if (TryParseMonthNameAndYear(text, isEndRange))
                return true;

            if (TryParseMonthNumberAndYear(text, isEndRange))
                return true;

            return false;
        }

        protected bool TryParseMonthNameAndYear(string text, bool isEndRange = false)
        {
            Regex rgx = GetRegexForMonthNameAndYear();
            Match match = rgx.Match(text);
            if (match.Success)
            {
                var monthNumberText = match.Groups[1].Value;
                var yearText = match.Groups[2].Value;
                var monthNumber = GetMonthNumber(monthNumberText);
                if (monthNumber != 0)
                {
                    if (isEndRange || Result.ContainsKey(Month1))
                        Result.Add(Month2, monthNumber);
                    else
                        Result.Add(Month1, monthNumber);
                    var year = GetYearNumber(yearText);
                    if (!string.IsNullOrEmpty(year))
                    {
                        if (Result.ContainsKey(Year1))
                            Result.Add(Year2, year);
                        else
                            Result.Add(Year1, year);

                        return true;
                    }
                }
            }
            return false;
        }

        protected bool TryParseMonth(string text, bool isEndRange = false)
        {
            Regex rgx = GetRegexForMonthName();
            Match match = rgx.Match(text);
            if (match.Success)
            {
                var monthNumberText = match.Groups[1].Value;
                var monthNumber = GetMonthNumber(monthNumberText);
                if (monthNumber != 0)
                {
                    if (isEndRange || Result.ContainsKey(Month1))
                        Result.Add(Month2, monthNumber);
                    else
                        Result.Add(Month1, monthNumber);

                    return true;
                }
            }

            rgx = GetRegexForMonthNumber();
            match = rgx.Match(text);
            if (match.Success)
            {
                var monthNumberText = match.Groups[1].Value;
                var monthNumber = GetMonthNumber(monthNumberText);
                if (monthNumber != 0)
                {
                    if (isEndRange || Result.ContainsKey(Month1))
                        Result.Add(Month2, monthNumber);
                    else
                        Result.Add(Month1, monthNumber);

                    return true;
                }
            }
            return false;
        }

        protected bool TryParseQuarterAndYear(string text, bool isEndRange = false)
        {
            Regex rgx = GetRegexForQuarterAndYear();
            Match match = rgx.Match(text);
            if (match.Success)
            {
                var quarterNumberText = match.Groups[1].Value;
                var yearText = match.Groups[2].Value;
                var quarterNumber = GetQuarterNumber(quarterNumberText);
                if (quarterNumber != 0)
                {
                    if (isEndRange || Result.ContainsKey(Quarter1))
                        Result.Add(Quarter2, quarterNumber);
                    else
                        Result.Add(Quarter1, quarterNumber);
                    var year = GetYearNumber(yearText);
                    if (!string.IsNullOrEmpty(year))
                    {
                        if (Result.ContainsKey(Year1))
                            Result.Add(Year2, year);
                        else
                            Result.Add(Year1, year);

                        return true;
                    }
                }
            }
            return false;
        }

        protected bool TryParseQuarter(string text, bool isEndRange = false)
        {
            Regex rgx = GetRegexForQuarterNumber();
            Match match = rgx.Match(text);
            if (match.Success)
            {
                var quarterNumberText = match.Groups[1].Value;
                var quarterNumber = GetQuarterNumber(quarterNumberText);
                if (quarterNumber != 0)
                {
                    if (isEndRange || Result.ContainsKey(Quarter1))
                        Result.Add(Quarter1, quarterNumber);
                    else
                        Result.Add(Quarter2, quarterNumber);

                    return true;
                }
            }
            return false;
        }

        protected bool TryParseYear(string text)
        {
            Regex rgx = GetRegexForYear();
            Match match = rgx.Match(text);
            if (match.Success)
            {
                var yearText = match.Groups[1].Value;
                var year = GetYearNumber(yearText);
                if (!string.IsNullOrEmpty(year))
                {
                    if (Result.ContainsKey(Year1))
                        Result.Add(Year2, year);
                    else
                        Result.Add(Year1, year);

                    return true;
                }
            }
            return false;
        }


        protected int GetQuarterNumber(string text)
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

        protected int GetMonthNumber(string text)
        {
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

        public abstract bool Parse();

        protected string[] SplitByDash(string text) => text.Split("-");
        protected string[] SplitByEmptySpace(string text) => text.Split(" ");

        protected int GetFirstNumber(string text)
        {
            string[] numbers = GetNumbers(text);
            int number;
            if (numbers.Length == 0 || !int.TryParse(numbers[0], out number))
                number = 0;

            return number;
        }

        protected string[] GetNumbers(string text) => Regex.Split(text, @"\D+").Where(n => !string.IsNullOrEmpty(n)).ToArray();

        protected string GetYearNumber(string text)
        {
            string year = "";
            if (text.Length <= 4)
            {
                var year2000 = "2000";
                year = year2000.Remove(year2000.Length - text.Length) + text;
            }
            return year;
        }

        protected string GetYear(string text)
        {
            string year = "";
            text = text.Trim();
            if (text.Length <= 4)
            {
                switch (text.Length)
                {
                    case 1:
                        year = $"200{text}";
                        break;
                    case 2:
                        year = $"20{text}";
                        break;
                    case 3:
                        year = $"2{text}";
                        break;
                    default:
                        year = text;
                        break;
                }
            }
            return year;
        }

        protected bool IsNumeric(string text)
        {
            return Regex.IsMatch(text, @"^\d+$");
        }

        protected bool HasOnlyYear(string text)
        {
            var withoutCharactersExceptPipe = ReplaceCharactersExceptPipeToEmptySpace(text);
            string[] dateRangeItems = withoutCharactersExceptPipe.Trim().Split(" ");
            return dateRangeItems.Length == 1 && IsNumeric(dateRangeItems[0]);
        }

        protected string ReplaceCharactersExceptPipeToEmptySpace(string text)
        {
            return Regex.Replace(text, @"[^0-9a-zA-Z|]+", " ");
        }
        protected string ReplaceCharactersExceptPipeAndDashToEmptySpace(string text)
        {
            return Regex.Replace(text, @"[^0-9a-zA-Z|-]+", " ");
        }

        protected bool StartsWithMonth(string text)
        {
            var withoutCharactersExceptPipe = ReplaceCharactersExceptPipeToEmptySpace(text.Trim());
            string[] items = withoutCharactersExceptPipe.Split(" ");
            if (items.Length > 1)
            {
                var possibleMonth = items[0];
                return MonthsFullNames.Any(m => m.Contains(possibleMonth)) || MonthsNumbers.Contains(possibleMonth);
            }
            else
            {
                return false;
            }
        }

        protected (int year, int month) GetBeginMonthAndYearFromDifference(int endingMonth, int endingYear, int monthlyPeriodDifference)
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

        protected (int year, int quarter) GetBeginQuarterAndYearFromDifference(int endingQuarter, int endingYear, int quarterlyPeriodDifference)
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

        public static bool EndsWithAny(string[] items, string text) => items.Any(i => text.EndsWith(i));
        public static bool ContainsAny(string[] items, string text) => items.Any(text.Contains);
    }
}
