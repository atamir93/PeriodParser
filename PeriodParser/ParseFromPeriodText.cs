using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PeriodParser
{
    public class ParseFromPeriodText
    {
        const string DimensionDefinition = "by";
        const string YearToDateDefinition = "ytd";
        const string LastDefinition = "last";
        const string FiscalDefinition = "fiscal";
        readonly string[] YearDefinitions = { "yearly", "years", "year" };
        readonly string[] QuarterDefinitions = { "quarterly", "quarters", "quarter" };
        readonly string[] QuarterNumbers = { "q1", "q2", "q2", "q4" };
        readonly string[] MonthDefinitions = { "monthly", "months", "month" };
        readonly string[] TotalDefinitions = { "total", "totals" };
        readonly string[] MonthsFullNames = { "january", "february", "march", "april", "may", "june", "july", "august", "september", "october", "november", "december" };
        readonly string[] MonthsShortNames = { "jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec" };
        readonly string[] MonthsNumbers = { "1","01", "2", "02", "3", "03", "4", "04", "5", "05", "6", "06",
            "7", "07", "8", "08", "9", "09", "10", "11", "12" };

        public Dictionary<string, object> Result { get; set; }
        const int CurrentYear = 2021;

        public void DistributeFromPeriodTextToView(string periodText)
        {
            Result = new Dictionary<string, object>();
            var periodParser = periodText.ToLower();
            //var lastPeriod = view.Period;
            //view.FiltersFromParser.Clear();
            if (periodParser.Contains(DimensionDefinition.ToLower()))
            {

            }
            else if (periodParser.EndsWith(YearToDateDefinition))
            {
                ParseToYTD(periodParser);
            }
            else if (EndsWithAny(periodParser, QuarterDefinitions))
            {

            }
            else if (EndsWithAny(periodParser, YearDefinitions))
            {

            }
            else if (EndsWithAny(periodParser, MonthDefinitions))
            {

            }
            else if (EndsWithAny(periodParser, TotalDefinitions))
            {

            }
            else
            {

            }
        }

        void ParseToYTD(string periodText)
        {
            Result.Add("Period", "Yearly");
            Result.Add("YearlyType", "YTD");
            if (periodText.Contains(LastDefinition))
            {
                if (!TryParseToYTDWithLastDefinition(periodText))
                    return;
            }
            else
            {
                var dateRanges = periodText.Split("-");
                if (dateRanges.Length == 0)
                {
                    if (!TryParseToYTDWithoutDateRange(periodText))
                        return;
                }
                else if (dateRanges.Length == 1)
                {

                }
                else
                {

                }
            }
        }

        string ReplaceCharactersExceptComaToEmptyEntry(string text)
        {
            return Regex.Replace(text, @"[^0-9a-zA-Z,]+", " ");
        }

        int GetMonthNumber(string text)
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

        private bool TryParseToYTDWithoutDateRange(string periodText)
        {
            var withoutCharactersExceptComa = ReplaceCharactersExceptComaToEmptyEntry(periodText);
            string[] items = withoutCharactersExceptComa.Split(" ");
            if (items.Length < 2)
            {
                Result.Add("Error", "");
                return false;
            }
            else
            {
                var month = items[0];
                int monthNumber = GetMonthNumber(month);
                if(monthNumber == 0)
                {
                    Result.Add("Error", "");
                    return false;
                }
                else
                {
                    Result.Add("Month", monthNumber);
                    var yearText = items[1];

                }
                Result.Add("YearlyPeriod", "Calendar");
                Result.Add("EndingYear", CurrentYear);
            }

            return true;
        }

        private bool TryParseToYTDWithLastDefinition(string periodText)
        {
            string[] numbers = Regex.Split(periodText, @"\D+").Where(n => !string.IsNullOrEmpty(n)).ToArray();
            int yearDifference;

            if (numbers.Length == 0 || !int.TryParse(numbers[0], out yearDifference))
            {
                Result.Add("Error", "");
                return false;
            }
            if (periodText.Contains(FiscalDefinition))
            {
                // to-do for fiscal years
            }
            else
            {
                Result.Add("YearlyPeriod", "Calendar");
                Result.Add("EndingYear", CurrentYear);
                Result.Add("BeginYear", CurrentYear - yearDifference);
            }
            return true;
        }

        bool EndsWithAny(string text, string[] items) => items.Any(i => text.EndsWith(i));
        bool ContainsAny(string text, string[] items) => items.Any(text.Contains);
    }
}
