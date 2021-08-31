using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PeriodParser
{
    public class MonthlyParserRegex : PeriodParserRegex
    {
        private MonthlyParserRegex() : base() { }
        private static MonthlyParserRegex instance = null;
        public static MonthlyParserRegex GetInstance()
        {
            if (instance == null)
            {
                instance = new MonthlyParserRegex();
            }
            return instance;
        }
        public override bool Parse()
        {
            Result = new Dictionary<string, object>();
            Result.Add(Period, ProfitAndLossPeriod.Monthly);

            bool isValid = false;
            if (TryParseLastDefinition(PeriodText))
                isValid = true;
            else
            {
                var dateRanges = SplitByDash(PeriodText);
                if (dateRanges.Length == 1)
                {
                    if (TryParse(PeriodText))
                        isValid = true;
                }
                else if (dateRanges.Length == 2)
                {
                    if (TryParse(dateRanges[0]))
                    {
                        if (TryParse(dateRanges[1]))
                            isValid = true;
                    }
                }

                if (isValid)
                {
                    if (Result.ContainsKey(Month2))
                        Result.Add(Type, "Consecutive");
                    else
                        Result.Add(Type, "EachYear");
                }
            }

            return isValid;
        }

        bool TryParse(string text)
        {
            if (TryParseMonthNameAndYear(text))
            {
                return true;
            }
            else if (TryParseMonthNumberAndYear(text))
            {
                return true;
            }
            else if (TryParseYear(text))
            {
                return true;
            }
            else if (TryParseMonth(text))
            {
                return true;
            }
            return false;
        }

        bool TryParseLastDefinition(string text)
        {
            if (TryParseEachYearLastDefinition(text))
            {
                return true;
            }
            else if (TryParseConsecutiveLastDefinition(text))
            {
                return true;
            }
            return false;
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
                    Result.Add(Year1, CurrentYear - yearlyDifference);
                    Result.Add(Year2, CurrentYear);
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
                    var beginMonthAndYear = GetBeginMonthAndYearFromDifference(CurrentMonth, CurrentYear, monthlyDifference);
                    Result.Add(Month1, beginMonthAndYear.month);
                    Result.Add(Year1, beginMonthAndYear.year);
                    Result.Add(Month2, CurrentMonth);
                    Result.Add(Year2, CurrentYear);
                    Result.Add(Type, "Consecutive");
                    return true;
                }
            }
            return false;
        }
    }
}
