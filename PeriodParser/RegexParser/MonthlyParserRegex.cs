using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PeriodParser.RegexParser
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
                    if (Result.ContainsKey(Month2))
                        Result.Add(Type, "Consecutive");
                    else
                        Result.Add(Type, "EachYear");
                }
            }

            return isValid;
        }

        private bool TryParseDateRanges()
        {
            bool isValid = false;
            var dateRanges = SplitByDash(PeriodText);
            for (int i = 0; i < Math.Min(2, dateRanges.Length); i++)
            {
                isValid = TryParse(dateRanges[i]);
            }
            return isValid;
        }

        bool TryParse(string text)
        {
            return TryParseMonthAndYear(text) || TryParseYear(text) || TryParseMonth(text);
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
