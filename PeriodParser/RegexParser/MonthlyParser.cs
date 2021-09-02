using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PeriodParser.RegexParser
{
    public class MonthlyParser : PeriodParser
    {
        private MonthlyParser() : base() { }
        private static MonthlyParser instance = null;
        public static MonthlyParser GetInstance()
        {
            if (instance == null)
                instance = new MonthlyParser();
            return instance;
        }

        public override bool TryParse()
        {
            Result = new Dictionary<string, object>
            {
                { Period, ProfitAndLossPeriod.Monthly }
            };

            bool isValid;
            if (TryParseLastDefinition(PeriodText))
                isValid = true;
            else
            {
                isValid = TryParseDateRanges();
                if (isValid)
                {
                    AddMissedMonthes();
                    AddMonthlyType();
                }
            }

            return isValid;
        }

        void AddMissedMonthes()
        {
            if (!Result.ContainsKey(Month1) && !Result.ContainsKey(Month2))
            {
                Result.Add(Month1, FirstMonthOfYear);
                Result.Add(Month2, LastMonthOfYear);
            }

            if (!Result.ContainsKey(Year1) && !Result.ContainsKey(Year2))
            {
                var beginYear = CurrentYear;
                if (ContainsBothMonthes() && (int)Result[Month1] > (int)Result[Month2])
                    beginYear = CurrentYear - 1;

                Result.Add(Year1, beginYear);
                Result.Add(Year2, CurrentYear);
            }
            else if (Result.ContainsKey(Year1) && !Result.ContainsKey(Year2))
            {
                Result.Add(Year2, Result[Year1]);
            }
        }

        bool ContainsBothMonthes()
        {
            return Result.ContainsKey(Month1) && Result.ContainsKey(Month2);
        }

        void AddMonthlyType()
        {
            if (Result.ContainsKey(Month2))
                Result.Add(Type, "Consecutive");
            else
                Result.Add(Type, "EachYear");
        }

        internal override bool TryParseDateText(string text, bool isEndRange = false)
        {
            return TryParseMonthAndYear(text) || TryParseYearAndMonthName(text) || TryParseYear(text) || TryParseMonth(text);
        }

        bool TryParseLastDefinition(string text)
        {
            return TryParseEachYearLastDefinition(text) || TryParseConsecutiveLastDefinition(text);
        }

        bool TryParseEachYearLastDefinition(string periodText)
        {
            Regex rgx = new Regex(@"^(jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec|this\s*month)\w*\s*for\s*last\s*(\d+)\s*year");
            Match match = rgx.Match(periodText);
            if (match.Success)
            {
                int yearlyDifference;
                if (int.TryParse(match.Groups[2].Value, out yearlyDifference))
                {
                    var month = match.Groups[1].Value;
                    var monthNumber = GetMonthNumber(month);
                    if (monthNumber == 0)
                        monthNumber = CurrentMonth;
                    Result.Add(Month1, monthNumber);
                    var lastYear = GetLastMonthQuarterYear().year;
                    Result.Add(Year1, lastYear - yearlyDifference + 1);
                    Result.Add(Year2, lastYear);
                    Result.Add(Type, "EachYear");
                    return true;
                }
            }
            return false;
        }

        bool TryParseConsecutiveLastDefinition(string periodText)
        {
            Regex rgx = new Regex(@"last\s*(\d+)\s*month");
            Match match = rgx.Match(periodText);
            if (match.Success)
            {
                int monthlyDifference;
                if (int.TryParse(match.Groups[1].Value, out monthlyDifference))
                {
                    var lastMonthAndYear = GetLastMonthQuarterYear();
                    var lastMonth = lastMonthAndYear.month;
                    var lastYear = lastMonthAndYear.year;
                    var beginMonthAndYear = GetBeginMonthAndYearFromDifference(lastMonth, lastYear, monthlyDifference);
                    Result.Add(Month1, beginMonthAndYear.month);
                    Result.Add(Year1, beginMonthAndYear.year);
                    Result.Add(Month2, lastMonth);
                    Result.Add(Year2, lastYear);
                    Result.Add(Type, "Consecutive");
                    return true;
                }
            }
            return false;
        }
    }
}
