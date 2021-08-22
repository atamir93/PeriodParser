namespace PeriodParser
{
    public class ParserConstants
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
        public readonly string[] MonthsFullNames = { "january", "february", "march", "april", "may", "june", "july", "august", "september", "october", "november", "december" };
        public readonly string[] MonthsShortNames = { "jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec" };
        public readonly string[] MonthsNumbers = { "1","01", "2", "02", "3", "03", "4", "04", "5", "05", "6", "06",
            "7", "07", "8", "08", "9", "09", "10", "11", "12" };
    }
}
