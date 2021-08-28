using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PeriodParser
{
    public class QuarterParser : PeriodParser
    {
        public QuarterParser(string text = "") : base(text) { }

        private static QuarterParser instance = null;
        public static QuarterParser GetInstance(string text)
        {
            if (instance == null)
            {
                instance = new QuarterParser();
            }
            instance.SetPeriodText(text);
            return instance;
        }
        public override bool Parse()
        {
            PeriodText = PeriodText.ToLower();
            Result = new Dictionary<string, object>();
            if (QuarterDefinitions.Any(q => PeriodText.Contains(q)))
            {
                foreach (var quarterText in QuarterDefinitions)
                {
                    PeriodText = PeriodText.ToLower().Replace(quarterText, "");
                }
            }
            Result.Add(Period, "Quarters");
            if (PeriodText.Contains(LastDefinition))
            {
                if (!TryParseWithLastDefinition(PeriodText))
                    return false;
            }
            else
            {
                var dateRanges = SplitByDash(PeriodText);
                if (dateRanges.Length == 2)
                {
                    if (!TryParseWitDateRange(PeriodText))
                        return false;
                }
                else
                {
                    Result.Add(Error, "");
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
                Result.Add(Type, "Consecutive");
                if (!TryParseRangeWithQuarterAndYear(rangeFirst))
                    return false;
                if (!TryParseRangeWithQuarterAndYear(rangeSecond))
                    return false;

            }
            else if (hasFirstRangeQuarter)
            {
                Result.Add(Type, "EachYear");
                if (!TryParseRangeWithQuarterAndYear(rangeFirst))
                    return false;
                if (!TryParseRangeWithYear(rangeSecond))
                    return false;

            }
            else
            {
                Result.Add(Error, "");
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
                Result.Add(Error, "");
                return false;
            }
            else
            {
                var quarterText = items[0];
                int quarterNumber = GetQuarterNumber(quarterText);
                if (quarterNumber == 0)
                {
                    Result.Add(Error, "");
                    return false;
                }
                else
                {
                    if (Result.ContainsKey(Quarter1))
                    {
                        Result.Add(Quarter2, quarterNumber);
                    }
                    else
                    {
                        Result.Add(Quarter1, quarterNumber);
                    }

                    var yearText = items[1];
                    string year = GetYear(yearText.Trim());
                    if (string.IsNullOrEmpty(year))
                    {
                        Result.Add(Error, "");
                        return false;
                    }
                    else
                    {
                        if (Result.ContainsKey(Year1))
                            Result.Add(Year2, year);
                        else
                            Result.Add(Year1, year);
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
                    Result.Add(Error, "");
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
            Result.Add(Type, "EachYear");
            if (periodText.Contains(ThisDefinition))
            {
                Result.Add(Quarter1, CurrentQuarter);
            }
            else
            {
                var quarter = QuarterNumbers.Where(q => periodText.Contains(q)).FirstOrDefault();
                if (quarter != null)
                {
                    Result.Add(Quarter1, quarter.Substring(1));
                    periodText = periodText.Replace(quarter, "");
                }
            }

            if (!Result.ContainsKey(Quarter1))
            {
                Result.Add(Error, "");
                return false;
            }

            int yearDifference = GetFirstNumber(periodText);
            if (yearDifference == 0)
            {
                Result.Add(Error, "");
                return false;
            }
            else
            {
                Result.Add(Year1, CurrentYear - yearDifference);
                Result.Add(Year2, CurrentYear);
            }

            return true;
        }

        bool TryParseToConsecutiveQuartersWithLastDefinitions(string periodText)
        {
            Result.Add(Type, "Consecutive");
            int quarterDifference = GetFirstNumber(periodText);
            if (quarterDifference == 0)
            {
                Result.Add(Error, "");
                return false;
            }
            else
            {
                var beginQuarterAndYear = GetBeginQuarterAndYearFromDifference(CurrentQuarter, CurrentYear, quarterDifference);
                Result.Add(Quarter1, beginQuarterAndYear.quarter);
                Result.Add(Year1, beginQuarterAndYear.year);
                Result.Add(Quarter2, CurrentQuarter);
                Result.Add(Year2, CurrentYear);
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

        private bool TryParseRangeWithYear(string yearText)
        {
            //TODO when year is 2100 or >
            string year = GetYear(yearText.Trim());
            if (string.IsNullOrEmpty(year))
            {
                Result.Add(Error, "");
                return false;
            }
            else
            {
                Result.Add(Year2, year);
            }
            return true;
        }
    }
}
