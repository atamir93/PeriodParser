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

        bool EndsWithAny(string text, string[] items) => items.Any(i => text.EndsWith(i));
        bool ContainsAny(string text, string[] items) => items.Any(text.Contains);
    }
}
