using System;
using System.Collections.Generic;
using System.Text;

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

        public const string Period = "period";
        public const string Type = "type";

        public const int CurrentYear = 2021;
        public const int CurrentMonth = 8;
        public const int CurrentQuarter = 3;

        public string CurrentPeriod { get; set; }

        public Dictionary<string, object> Result { get; set; }

        public string PeriodText { get; set; }

        protected ProfitAndLossParser(string text)
        {
            PeriodText = text.ToLower().Trim();
            Result = new Dictionary<string, object>();
        }

        protected abstract bool Parse(string text);
    }
}
