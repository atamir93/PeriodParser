using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace PeriodParser
{
    public abstract class ProfitAndLossParser
    {
        public const string DimensionDefinition = " by ";
        public const string YearToDateDefinition = "ytd";
        public const string LastDefinition = "last";
        public const string FiscalDefinition = "fiscal";
        public readonly string[] YearDefinitions = { "yearly", "years", "year" };
        public readonly string[] QuarterDefinitions = { "quarterly", "quarters", "quarter" };
        public readonly string[] QuarterNumbers = { "q1", "q2", "q3", "q4" };
        public readonly string[] MonthDefinitions = { "monthly", "months", "month" };
        public readonly string[] TotalDefinitions = { "total", "totals" };
        readonly string[] SeasonsDefinitions = { "seasons", "season" };
        public readonly string[] MonthsFullNames = { "january", "february", "march", "april", "may", "june", "july", "august", "september", "october", "november", "december" };
        public readonly string[] MonthsShortNames = { "jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec" };
        public readonly string[] MonthsNumbers = { "1","01", "2", "02", "3", "03", "4", "04", "5", "05", "6", "06",
            "7", "07", "8", "08", "9", "09", "10", "11", "12" };

        public const string Period = "Period";
        public const string Type = "YearlyType";
        public const string Error = "Error";
        public const string YearlyPeriod = "YearlyPeriod";
        public const string EndingMonth = "EndingMonth";
        public const string BeginYear = "BeginYear";
        public const string EndingYear = "EndingYear";

        public const int CurrentYear = 2021;
        public const int CurrentMonth = 8;
        public const int CurrentQuarter = 3;

        public string CurrentPeriod { get; set; }

        public Dictionary<string, object> Result { get; set; }

        public string PeriodText { get; set; }

        public ProfitAndLossParser(string text)
        {
            PeriodText = text.ToLower().Trim();
            Result = new Dictionary<string, object>();
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

        protected string GetYear(string text)
        {
            string year = "";
            if (IsNumeric(text) && text.Length <= 4)
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

        protected int GetMonthNumber(string text)
        {
            int monthNumber = 0;
            if (int.TryParse(text, out monthNumber))
            {
                if (monthNumber < 1 || monthNumber > 12)
                {
                    Result.Add("Error", "");
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

        protected bool HasOnlyYear(string text)
        {
            var withoutCharactersExceptPipe = ReplaceCharactersExceptPipeToEmptySpace(text);
            string[] dateRangeItems = withoutCharactersExceptPipe.Trim().Split(" ");
            return dateRangeItems.Length == 1;
        }

        protected string ReplaceCharactersExceptPipeToEmptySpace(string text)
        {
            return Regex.Replace(text, @"[^0-9a-zA-Z|]+", " ");
        }
    }
}
