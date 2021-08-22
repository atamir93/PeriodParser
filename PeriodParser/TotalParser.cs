using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PeriodParser
{
    public class TotalParser
    {
        readonly string[] TotalDefinitions = { "totals", "total" };
        readonly string[] MonthsFullNames = { "january", "february", "march", "april", "may", "june", "july", "august", "september", "october", "november", "december" };
        readonly string[] MonthsShortNames = { "jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec" };
        readonly string[] MonthsNumbers = { "1","01", "2", "02", "3", "03", "4", "04", "5", "05", "6", "06",
            "7", "07", "8", "08", "9", "09", "10", "11", "12" };

        public Dictionary<string, object> Result { get; set; }
        const int CurrentYear = 2021;
        const int CurrentMonth = 8;
        const int FirstMonth = 1;
        const int LastMonth = 12;

        public bool Parse(string periodText)
        {
            periodText = periodText.ToLower().Trim();
            Result = new Dictionary<string, object>();
            if (TotalDefinitions.Any(q => periodText.Contains(q)))
            {
                foreach (var seasonText in TotalDefinitions)
                {
                    periodText = periodText.ToLower().Replace(seasonText, "");
                }
            }

            Result.Add("Period", "Total");

            var dateRanges = periodText.Split("-");
            if (dateRanges.Length == 2)
            {
                if (!TryParseWitDateRange(periodText))
                    return false;
            }
            else if (dateRanges.Length == 1)
            {
                if (!TryParseWithoutRange(periodText))
                    return false;
            }
            else
            {
                Result.Add("Error", "");
                return false;
            }

            return true;
        }

        bool TryParseWithoutRange(string yearText)
        {
            var year = GetYear(yearText);
            if (string.IsNullOrEmpty(year))
            {
                Result.Add("Error", "");
                return false;
            }
            else
            {
                Result.Add("BeginMonth", FirstMonth);
                Result.Add("EndingMonth", LastMonth);
                Result.Add("BeginYear", year);
                Result.Add("EndingYear", year);
            }

            return true;
        }

        string GetYear(string yearText)
        {
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
                return year;
            }
            return null;
        }

        bool TryParseWitDateRange(string periodText)
        {
            var dateRanges = periodText.Split("-");
            var rangeFirst = dateRanges[0].Trim();
            var rangeSecond = dateRanges[1].Trim();

            bool hasFirstRangeMonth = StartsWithMonth(rangeFirst);
            bool hasSecondRangeMonth = StartsWithMonth(rangeSecond);

            if (hasFirstRangeMonth && hasSecondRangeMonth)
            {
                if (!TryParseRangeWithMonthAndYear(rangeFirst))
                    return false;
                if (!TryParseRangeWithMonthAndYear(rangeSecond))
                    return false;

            }
            else if (hasFirstRangeMonth)
            {
                if (!TryParseRangeWithMonthAndYear(rangeFirst))
                    return false;
                if (!TryParseRangeWithYear(rangeSecond))
                    return false;
            }
            else if (hasSecondRangeMonth)
            {
                if (!TryParseRangeWithYear(rangeFirst))
                    return false;
                if (!TryParseRangeWithMonthAndYear(rangeSecond))
                    return false;
            }
            else
            {
                if (!TryParseRangeWithYear(rangeFirst))
                    return false;
                if (!TryParseRangeWithYear(rangeSecond))
                    return false;
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
                {
                    Result.Add("EndingYear", year);
                }
                else
                {
                    Result.Add("BeginYear", year);
                }
                if (Result.ContainsKey("BeginMonth"))
                {
                    Result.Add("EndingMonth", 12);
                }
                else
                {
                    Result.Add("BeginMonth", 1);
                }
            }
            else
            {
                Result.Add("Error", "");
                return false;
            }
            return true;
        }
        string ReplaceCharactersExceptPipeToEmptySpace(string text)
        {
            return Regex.Replace(text, @"[^0-9a-zA-Z|]+", " ");
        }

        bool IsNumeric(string text)
        {
            return Regex.IsMatch(text, @"^\d+$");
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
    }
}
