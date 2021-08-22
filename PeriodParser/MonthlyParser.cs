using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PeriodParser
{
    public class MonthlyParser
    {
        const string LastDefinition = "last";
        const string ThisDefinition = "this";
        const string FiscalDefinition = "fiscal";
        readonly string[] YearDefinitions = { "yearly", "years", "year" };
        readonly string[] MonthDefinitions = { "monthly", "months", "month" };
        readonly string[] MonthsFullNames = { "january", "february", "march", "april", "may", "june", "july", "august", "september", "october", "november", "december" };
        readonly string[] MonthsShortNames = { "jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec" };
        readonly string[] MonthsNumbers = { "1","01", "2", "02", "3", "03", "4", "04", "5", "05", "6", "06",
            "7", "07", "8", "08", "9", "09", "10", "11", "12" };

        public Dictionary<string, object> Result { get; set; }
        const int CurrentYear = 2021;
        const int CurrentMonth = 8;

        public bool Parse(string periodText)
        {
            periodText = periodText.ToLower();
            Result = new Dictionary<string, object>();
            if (MonthDefinitions.Any(q => periodText.Contains(q)))
            {
                foreach (var quarterText in MonthDefinitions)
                {
                    periodText = periodText.ToLower().Replace(quarterText, "");
                }
            }
            Result.Add("Period", "Months");
            if (periodText.Contains(LastDefinition))
            {
                if (!TryParseWithLastDefinition(periodText))
                    return false;
            }
            else
            {
                var dateRanges = periodText.Split("-");
                if (dateRanges.Length == 2)
                {
                    if (!TryParseWitDateRange(periodText))
                        return false;
                }
                else
                {
                    Result.Add("Error", "");
                    return false;
                }
            }
            return true;
        }

        bool StartsWithMonth(string text)
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

        bool TryParseWitDateRange(string periodText)
        {
            var dateRanges = periodText.Split("-");
            var rangeFirst = dateRanges[0];
            var rangeSecond = dateRanges[1];

            bool hasFirstRangeMonth = StartsWithMonth(rangeFirst);
            bool hasSecondRangeMonth = StartsWithMonth(rangeSecond);

            if (hasFirstRangeMonth && hasSecondRangeMonth)
            {
                Result.Add("Type", "Consecutive");
                if (!TryParseRangeWithMonthAndYear(rangeFirst))
                    return false;
                if (!TryParseRangeWithMonthAndYear(rangeSecond))
                    return false;

            }
            else if (hasFirstRangeMonth)
            {
                Result.Add("Type", "EachYear");
                if (!TryParseRangeWithMonthAndYear(rangeFirst))
                    return false;
                if (!TryParseRangeWithYear(rangeSecond))
                    return false;

            }
            else
            {
                Result.Add("Error", "");
                return false;
            }

            return true;
        }
        bool TryParseRangeWithMonthAndYear(string monthAndYearText)
        {
            var withoutCharactersExceptPipe = ReplaceCharactersExceptPipeToEmptySpace(monthAndYearText.Trim());
            string[] items = withoutCharactersExceptPipe.Split(" ");
            if (items.Length < 2)
            {
                Result.Add("Error", "");
                return false;
            }
            else
            {
                var month = items[0];
                int monthNumber = GetMonthNumber(month);
                if (monthNumber == 0)
                {
                    Result.Add("Error", "");
                    return false;
                }
                else
                {
                    if (Result.ContainsKey("BeginMonth"))
                    {
                        Result.Add("EndingMonth", monthNumber);
                    }
                    else
                    {
                        Result.Add("BeginMonth", monthNumber);
                    }
                    var yearText = items[1];
                    if (IsNumeric(yearText) && yearText.Length <= 4)
                    {
                        string year;
                        switch (yearText.Length)
                        {
                            case 1:
                                year = $"200{yearText}";
                                break;
                            case 2:
                                year = $"20{yearText}";
                                break;
                            case 3:
                                year = $"2{yearText}";
                                break;
                            default:
                                year = yearText;
                                break;
                        }
                        if (Result.ContainsKey("BeginYear"))
                        {
                            Result.Add("EndingYear", year);
                        }
                        else
                        {
                            Result.Add("BeginYear", year);
                        }
                    }
                    else
                    {
                        Result.Add("Error", "");
                        return false;
                    }
                }
            }
            return true;
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

        bool TryParseWithLastDefinition(string periodText)
        {
            if (YearDefinitions.Any(y => periodText.Contains(y)))
            {
                if (!TryParseToEachYearMonthlyWithLastDefinitions(periodText))
                    return false;
            }
            else
            {
                if (!TryParseToConsecutiveMonthlyWithLastDefinitions(periodText))
                    return false;
            }

            return true;
        }

        bool TryParseToEachYearMonthlyWithLastDefinitions(string periodText)
        {
            Result.Add("Type", "EachYear");
            if (periodText.Contains(ThisDefinition))
            {
                Result.Add("BeginMonth", CurrentMonth);
            }
            else
            {
                var possibleMonth = periodText.Trim().Split(" ").FirstOrDefault();
                if (possibleMonth != null)
                {
                    int monthNumber = GetMonthNumber(possibleMonth);
                    if (monthNumber == 0)
                    {
                        Result.Add("Error", "");
                        return false;
                    }
                    else
                    {
                        Result.Add("BeginMonth", monthNumber);
                        periodText = periodText.Replace(possibleMonth, "");
                    }
                }
            }

            if (!Result.ContainsKey("BeginMonth"))
            {
                Result.Add("Error", "");
                return false;
            }

            string[] numbers = Regex.Split(periodText, @"\D+").Where(n => !string.IsNullOrEmpty(n)).ToArray();  // get numbers from text
            int yearDifference;

            if (numbers.Length == 0 || !int.TryParse(numbers[0], out yearDifference))
            {
                Result.Add("Error", "");
                return false;
            }
            else
            {
                Result.Add("BeginYear", CurrentYear - yearDifference);
                Result.Add("EndingYear", CurrentYear);
            }

            return true;
        }

        bool TryParseToConsecutiveMonthlyWithLastDefinitions(string periodText)
        {
            Result.Add("Type", "Consecutive");
            string[] numbers = Regex.Split(periodText, @"\D+").Where(n => !string.IsNullOrEmpty(n)).ToArray();  // get numbers from text
            int monthDifference;

            if (numbers.Length == 0 || !int.TryParse(numbers[0], out monthDifference))
            {
                Result.Add("Error", "");
                return false;
            }
            else
            {
                var beginMonthAndYear = GetBeginMonthAndYearFromDifference(CurrentMonth, CurrentYear, monthDifference);
                Result.Add("BeginMonth", beginMonthAndYear.month);
                Result.Add("BeginYear", beginMonthAndYear.year);
                Result.Add("EndingMonth", CurrentMonth);
                Result.Add("EndingYear", CurrentYear);
            }

            return true;
        }

        (int year, int month) GetBeginMonthAndYearFromDifference(int endingMonth, int endingYear, int monthlyPeriodDifference)
        {
            var yearDiff = monthlyPeriodDifference / 12;
            var difference = monthlyPeriodDifference % 12;
            var beginMonth = endingMonth - difference;
            if (beginMonth <= 0)
            {
                yearDiff++;
                beginMonth += 12;
            }

            return (endingYear - yearDiff, beginMonth);
        }


        bool HasOnlyYear(string text)
        {
            var withoutCharactersExceptPipe = ReplaceCharactersExceptPipeToEmptySpace(text);
            string[] dateRangeItems = withoutCharactersExceptPipe.Trim().Split(" ");
            return dateRangeItems.Length == 1;
        }

        string ReplaceCharactersExceptPipeToEmptySpace(string text)
        {
            return Regex.Replace(text, @"[^0-9a-zA-Z|]+", " ");
        }

        bool IsNumeric(string text)
        {
            return Regex.IsMatch(text, @"^\d+$");
        }

        private bool TryParseRangeWithYear(string yearText)
        {
            //TODO when year is 2100 or >
            yearText = yearText.Trim();
            if (IsNumeric(yearText) && yearText.Length <= 4)
            {
                string year;
                switch (yearText.Length)
                {
                    case 1:
                        year = $"200{yearText}";
                        break;
                    case 2:
                        year = $"20{yearText}";
                        break;
                    case 3:
                        year = $"2{yearText}";
                        break;
                    default:
                        year = yearText;
                        break;
                }
                Result.Add("EndingYear", year);

            }
            else
            {
                Result.Add("Error", "");
                return false;
            }
            return true;
        }
    }
}
