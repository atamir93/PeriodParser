using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PeriodParser
{
    public class QuarterParser
    {
        const string LastDefinition = "last";
        const string ThisDefinition = "this";
        const string FiscalDefinition = "fiscal";
        readonly string[] YearDefinitions = { "yearly", "years", "year" };
        readonly string[] QuarterDefinitions = { "quarterly", "quarters", "quarter" };
        readonly string[] QuarterNumbers = { "q1", "q2", "q3", "q4" };

        public Dictionary<string, object> Result { get; set; }
        const int CurrentYear = 2021;
        const int CurrentQuarter = 3;

        public bool Parse(string periodText)
        {
            periodText = periodText.ToLower();
            Result = new Dictionary<string, object>();
            if (QuarterDefinitions.Any(q => periodText.Contains(q)))
            {
                foreach (var quarterText in QuarterDefinitions)
                {
                    periodText = periodText.ToLower().Replace(quarterText, "");
                }
            }
            Result.Add("Period", "Quarters");
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

        bool TryParseWitDateRange(string periodText)
        {
            var dateRanges = periodText.Split("-");
            var rangeFirst = dateRanges[0];
            var rangeSecond = dateRanges[1];

            bool hasFirstRangeQuarter = QuarterNumbers.Any(q => rangeFirst.Contains(q));
            bool hasSecondRangeQuarter = QuarterNumbers.Any(q => rangeSecond.Contains(q));

            if (hasFirstRangeQuarter && hasSecondRangeQuarter)
            {
                Result.Add("Type", "Consecutive");
                if (!TryParseRangeWithQuarterAndYear(rangeFirst))
                    return false;
                if (!TryParseRangeWithQuarterAndYear(rangeSecond))
                    return false;

            }
            else if (hasFirstRangeQuarter)
            {
                Result.Add("Type", "EachYear");
                if (!TryParseRangeWithQuarterAndYear(rangeFirst))
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
        bool TryParseRangeWithQuarterAndYear(string monthAndYearText)
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
                var quarterText = items[0];
                int quarterNumber = GetQuarterNumber(quarterText);
                if (quarterNumber == 0)
                {
                    Result.Add("Error", "");
                    return false;
                }
                else
                {
                    if (Result.ContainsKey("BeginQuarter"))
                    {
                        Result.Add("EndingQuarter", quarterNumber);
                    }
                    else
                    {
                        Result.Add("BeginQuarter", quarterNumber);
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

        int GetQuarterNumber(string text)
        {
            text = text.Replace("q", "");
            int quarterNumber = 0;
            if (int.TryParse(text, out quarterNumber))
            {
                if (quarterNumber < 1 || quarterNumber > 4)
                {
                    Result.Add("Error", "");
                    return 0;
                }
            }
            return quarterNumber;
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
                var quarter = QuarterNumbers.Where(q => periodText.Contains(q)).FirstOrDefault();
                if (quarter != null)
                {
                    Result.Add("BeginQuarter", quarter.Substring(1));
                    periodText = periodText.Replace(quarter, "");
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
            Result.Add("Type", "Consecutive");
            string[] numbers = Regex.Split(periodText, @"\D+").Where(n => !string.IsNullOrEmpty(n)).ToArray();  // get numbers from text
            int quarterDifference;

            if (numbers.Length == 0 || !int.TryParse(numbers[0], out quarterDifference))
            {
                Result.Add("Error", "");
                return false;
            }
            else
            {
                var beginQuarterAndYear = GetBeginQuarterAndYearFromDifference(CurrentQuarter, CurrentYear, quarterDifference);
                Result.Add("BeginQuarter", beginQuarterAndYear.quarter);
                Result.Add("BeginYear", beginQuarterAndYear.year);
                Result.Add("EndingQuarter", CurrentQuarter);
                Result.Add("EndingYear", CurrentYear);
            }

            return true;
        }

        (int year, int quarter) GetBeginQuarterAndYearFromDifference(int endingQuarter, int endingYear, int quarterlyPeriodDifference)
        {
            var yearDiff = quarterlyPeriodDifference / 4;
            var difference = quarterlyPeriodDifference % 4;
            var beginQuarter = endingQuarter - difference;
            if (beginQuarter <= 0)
            {
                yearDiff++;
                beginQuarter += 4;
            }

            return (endingYear - yearDiff, beginQuarter);
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
