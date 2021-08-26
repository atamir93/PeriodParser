using System.Collections.Generic;

namespace PeriodParser
{
    public class DimensionParser : ProfitAndLossParser
    {
        public DimensionParser(string text = "") : base(text) { }
        public override bool Parse()
        {
            PeriodText = PeriodText.Trim();
            PeriodText = ReplaceCharactersExceptPipeAndDashToEmptySpace(PeriodText);
            var words = SplitByEmptySpace(PeriodText);
            var dimensionName = words[words.Length - 1];

            Result.Add(Period, "Dimension");
            Result.Add("DimensionName", dimensionName.ToLower());

            PeriodText = PeriodText.Replace(DimensionDefinition, "");
            PeriodText = PeriodText.Replace(dimensionName, "");
            PeriodText = PeriodText.ToLower().Trim();
            var dateRanges = SplitByDash(PeriodText);
            if (PeriodText.Contains(YearToDateDefinition))
            {
                Result.Add("DimensionPeriod", "YearToDate");
                PeriodText = PeriodText.Replace(YearToDateDefinition, "");
                return TryParseRangeWithMonthAndYear(PeriodText);
            }
            else if (HasOnlyYear(PeriodText))
            {
                Result.Add("DimensionPeriod", "EntireYear");
                return TryParseRangeWithYear(PeriodText);
            }
            else if (dateRanges.Length == 2)
            {
                Result.Add("DimensionPeriod", "Range");
                if (!TryParseRangeWithMonthAndYear(dateRanges[0]))
                {
                    return false;
                }
                return TryParseRangeWithMonthAndYear(dateRanges[1]);
            }
            else if (ContainsAny(QuarterNumbers, PeriodText))
            {
                Result.Add("DimensionPeriod", "Quarter");
                return TryParseRangeWithQuarterAndYear(PeriodText);
            }
            else
            {
                Result.Add("DimensionPeriod", "Month");
                return TryParseRangeWithMonthAndYear(PeriodText);
            }

            return true;
        }

        bool TryParseRangeWithMonthAndYear(string monthAndYearText)
        {
            var withoutCharactersExceptPipe = ReplaceCharactersExceptPipeToEmptySpace(monthAndYearText).Trim();
            string[] items = SplitByEmptySpace(withoutCharactersExceptPipe);
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
                        Result.Add("EndingMonth", monthNumber);
                    else
                        Result.Add("BeginMonth", monthNumber);

                    var yearText = items[1];
                    string year = GetYear(yearText.Trim());
                    if (string.IsNullOrEmpty(year))
                    {
                        Result.Add("Error", "");
                        return false;
                    }
                    else
                    {
                        if (Result.ContainsKey("BeginYear"))
                            Result.Add("EndingYear", year);
                        else
                            Result.Add("BeginYear", year);
                    }
                }
            }
            return true;
        }


        bool TryParseRangeWithYear(string yearText)
        {
            //TODO when year is 2100 or >
            string year = GetYear(yearText.Trim());
            if (string.IsNullOrEmpty(year))
            {
                Result.Add("Error", "");
                return false;
            }
            else
            {
                if (Result.ContainsKey("BeginYear"))
                    Result.Add("EndingYear", year);
                else
                    Result.Add("BeginYear", year);
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
                    string year = GetYear(yearText.Trim());
                    if (string.IsNullOrEmpty(year))
                    {
                        Result.Add("Error", "");
                        return false;
                    }
                    else
                    {
                        if (Result.ContainsKey("BeginYear"))
                            Result.Add("EndingYear", year);
                        else
                            Result.Add("BeginYear", year);
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
    }
}
