using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PeriodParser
{
    public class Parser2
    {
        const string DimensionDefinition = " by ";
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
        const int CurrentMonth = 8;
        const int CurrentQuarter = 3;

        
        public void Distribute(string periodText)
        {
            Result = new Dictionary<string, object>();
            periodText = periodText.ToLower();

            if (periodText.Contains(LastDefinition))
            {

            }
            else if (ContainsAny(periodText, QuarterNumbers))
            {

            }
            else
            {
                var dateRanges = periodText.Split("-");
                if (dateRanges.Any())
                {

                }
                else
                {

                }
            }
        }

        private bool TryParseUsingLastDefinition(string periodText)
        {
            var beforAndAfterLast = periodText.Split("last");

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
                else
                {

                }
            }
        }

        private bool TryParseToYTDWithoutDateRange(string periodText)
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
