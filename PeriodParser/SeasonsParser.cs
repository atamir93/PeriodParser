using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PeriodParser
{
    public class SeasonsParser
    {
        readonly string[] SeasonsDefinitions = { "seasons", "season" };
        const string FiscalDefinition = "fiscal";
        readonly string[] MonthsFullNames = { "january", "february", "march", "april", "may", "june", "july", "august", "september", "october", "november", "december" };
        readonly string[] MonthsShortNames = { "jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec" };
        readonly string[] MonthsNumbers = { "1","01", "2", "02", "3", "03", "4", "04", "5", "05", "6", "06",
            "7", "07", "8", "08", "9", "09", "10", "11", "12" };

        public Dictionary<string, object> Result { get; set; }
        const int CurrentYear = 2021;
        const int CurrentMonth = 8;

        public bool Parse(string periodText)
        {
            periodText = periodText.ToLower().Trim();
            Result = new Dictionary<string, object>();
            if (SeasonsDefinitions.Any(q => periodText.Contains(q)))
            {
                foreach (var seasonText in SeasonsDefinitions)
                {
                    periodText = periodText.ToLower().Replace(seasonText, "");
                }
            }

            Result.Add("Period", "Seasons");

            periodText = ReplaceCharactersExceptPipeAndDashToEmptySpace(periodText.Trim());
            var dateRanges = periodText.Split("-");
            if (dateRanges.Length == 3)
            {
                if (!TryParseFullRange(periodText))
                    return false;
            }
            else if (dateRanges.Length == 2)
            {
                if (!TryParseRangeWithSingleYear(periodText))
                    return false;
            }
            else
            {
                Result.Add("Error", "");
                return false;
            }

            return true;
        }

        bool TryParseRangeWithSingleYear(string monthMonthYearYearText)
        {
            var dateRanges = monthMonthYearYearText.Split("-");

            var monthItem = dateRanges[0].Trim();
            if (!TryParseRangeWithMonth(monthItem))
                return false;

            var monthAndYearItems = dateRanges[1].Trim();
            if (TryParseRangeWithMonthAndYear(monthAndYearItems))
            {
                if (Result.ContainsKey("BeginYear"))
                {
                    Result.Add("EndingYear", Result["BeginYear"]);
                }
                else
                    return false;
            }
            else
            {
                return false;
            }

            return true;
        }

        bool TryParseFullRange(string monthMonthYearYearText)
        {
            var dateRanges = monthMonthYearYearText.Split("-");

            var monthItem = dateRanges[0].Trim();
            if (!TryParseRangeWithMonth(monthItem))
                return false;

            var monthAndYearItems = dateRanges[1].Trim();
            if (!TryParseRangeWithMonthAndYear(monthAndYearItems))
                return false;

            var yearItem = dateRanges[2].Trim();
            if (!TryParseRangeWithYear(yearItem))
                return false;

            return true;
        }

        bool TryParseRangeWithMonth(string monthText)
        {
            int monthNumber = GetMonthNumber(monthText);
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
        string ReplaceCharactersExceptPipeToEmptySpace(string text)
        {
            return Regex.Replace(text, @"[^0-9a-zA-Z|]+", " ");
        }
        string ReplaceCharactersExceptPipeAndDashToEmptySpace(string text)
        {
            return Regex.Replace(text, @"[^0-9a-zA-Z|-]+", " ");
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
