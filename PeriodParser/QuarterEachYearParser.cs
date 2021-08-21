using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PeriodParser
{
    public class QuarterEachYearParser
    {
        const string LastDefinition = "last";
        const string ThisDefinition = "this";
        const string FiscalDefinition = "fiscal";
        readonly string[] YearDefinitions = { "yearly", "years", "year" };
        readonly string[] QuarterDefinitions = { "quarterly", "quarters", "quarter" };
        readonly string[] QuarterNumbers = { "q1", "q2", "q2", "q4" };

        public Dictionary<string, object> Result { get; set; }
        const int CurrentYear = 2021;
        const int CurrentMonth = 8;
        const int CurrentQuarter = 3;

        public bool Parse(string periodText)
        {
            Result = new Dictionary<string, object>();
            foreach (var yearText in QuarterDefinitions)
            {
                periodText = periodText.ToLower().Replace(yearText, "");
            }
            Result.Add("Period", "Quarters");
            if (periodText.Contains(LastDefinition))
            {
                if (!TryParseWithLastDefinition(periodText))
                    return false;
            }
            else
            {
                //var dateRanges = periodText.Split("-");
                //if (dateRanges.Length == 1)
                //{
                //    //if (!TryParseToYTDWithoutDateRange(periodText))
                //    return false;
                //}
                //else if (dateRanges.Length == 2)
                //{
                //    if (!TryParseToYTDWithDateRange(periodText))
                //        return false;
                //}
                //else
                //{
                //    Result.Add("Error", "");
                //    return false;
                //}
            }
            return true;
        }

        bool TryParseWithLastDefinition(string periodText)
        {
            if (YearDefinitions.Any(y => periodText.Contains(y)))
            {
                if (!TryParseToEachYearQuartersWithLastDefinitions(periodText))
                    return false;
            }
            else
            {
                if (!TryParseToConsecutiveQuartersWithLastDefinitions(periodText))
                    return false;
            }

            return true;
        }

        bool TryParseToEachYearQuartersWithLastDefinitions(string periodText)
        {
            Result.Add("Type", "EachYear");
            if (periodText.Contains(ThisDefinition))
            {
                Result.Add("BeginQuarter", CurrentQuarter);
            }
            else
            {
                var quarter = QuarterDefinitions.Where(q => periodText.Contains(q)).FirstOrDefault();
                if (quarter != null)
                {
                    Result.Add("BeginQuarter", quarter);
                }
            }

            if (!Result.ContainsKey("BeginQuarter"))
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

        bool TryParseToConsecutiveQuartersWithLastDefinitions(string periodText)
        {
            Result.Add("Type", "EachYear");
            string[] numbers = Regex.Split(periodText, @"\D+").Where(n => !string.IsNullOrEmpty(n)).ToArray();  // get numbers from text
            int quarterDifference;

            if (numbers.Length == 0 || !int.TryParse(numbers[0], out quarterDifference))
            {
                Result.Add("Error", "");
                return false;
            }
            else
            {
                var yearDiff = quarterDifference / 4;
                var smallQuarterDifference = quarterDifference % 4;
                var beginYear = CurrentYear - yearDiff;
                if (CurrentQuarter <= smallQuarterDifference)
                {
                    beginYear--;
                }
                //Result.Add("BeginQuarter", CurrentQuarter);
                Result.Add("BeginYear", beginYear);
                Result.Add("EndingQuarter", CurrentQuarter);
                Result.Add("EndingYear", CurrentYear);
            }

            return true; ;
        }

        bool TryParseToYTDWithDateRange(string periodText)
        {
            var dateRanges = periodText.Split("-");
            var rangeFirst = dateRanges[0];
            var rangeSecond = dateRanges[1];

            if (HasOnlyYear(rangeFirst))
            {
                Result.Add("EndingMonth", CurrentMonth);
                if (!TryParseRangeWithYear(rangeFirst))
                    return false;
            }
            else
            {
                if (!TryParseRangeWithMonthAndYear(rangeFirst))
                    return false;
            }

            if (!TryParseRangeWithYear(rangeSecond))
                return false;

            return true;
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
                if (Result.ContainsKey("BeginYear"))
                    Result.Add("EndingYear", year);
                else
                    Result.Add("BeginYear", year);

            }
            else
            {
                Result.Add("Error", "");
                return false;
            }
            return true;
        }

        private bool TryParseRangeWithMonthAndYear(string monthAndYearText)
        {
            var withoutCharactersExceptPipe = ReplaceCharactersExceptPipeToEmptySpace(monthAndYearText);
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
                    Result.Add("EndingMonth", monthNumber);
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
                        Result.Add("BeginYear", year);
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

        bool IsNumeric(string text)
        {
            return Regex.IsMatch(text, @"^\d+$");
        }
    }
}
