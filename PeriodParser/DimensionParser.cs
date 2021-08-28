namespace PeriodParser
{
    public class DimensionParser : PeriodParser
    {
        public DimensionParser(string text = "") : base(text) { }

        private static DimensionParser instance = null;
        public static DimensionParser GetInstance()
        {
            if (instance == null)
            {
                instance = new DimensionParser();
            }
            return instance;
        }

        public override bool Parse()
        {
            PeriodText = PeriodText.Trim();
            PeriodText = ReplaceCharactersExceptPipeAndDashToEmptySpace(PeriodText);
            var dimensionName = PeriodText.Substring(PeriodText.IndexOf(DimensionDefinition) + DimensionDefinition.Length).Trim().ToLower();

            Result.Add(Period, ProfitAndLossPeriod.Dimension);
            Result.Add(DimensionName, dimensionName.ToLower());

            PeriodText = PeriodText.Replace(DimensionDefinition, "");
            PeriodText = PeriodText.Replace(dimensionName, "");
            PeriodText = PeriodText.ToLower().Trim();
            var dateRanges = SplitByDash(PeriodText);
            if (PeriodText.Contains(YearToDateDefinition))
            {
                Result.Add(DimensionPeriod, DimensionCompareType.YearToDate);
                PeriodText = PeriodText.Replace(YearToDateDefinition, "");
                return TryParseRangeWithMonthAndYear(PeriodText);
            }
            else if (HasOnlyYear(PeriodText))
            {
                Result.Add(DimensionPeriod, DimensionCompareType.EntireYear);
                return TryParseRangeWithYear(PeriodText);
            }
            else if (dateRanges.Length == 2)
            {
                Result.Add(DimensionPeriod, DimensionCompareType.Range);
                if (!TryParseRangeWithMonthAndYear(dateRanges[0]))
                {
                    return false;
                }
                return TryParseRangeWithMonthAndYear(dateRanges[1]);
            }
            else if (ContainsAny(QuarterNumbers, PeriodText))
            {
                Result.Add(DimensionPeriod, DimensionCompareType.Quarter);
                return TryParseRangeWithQuarterAndYear(PeriodText);
            }
            else
            {
                Result.Add(DimensionPeriod, DimensionCompareType.Month);
                return TryParseRangeWithMonthAndYear(PeriodText);
            }
        }

        bool TryParseRangeWithMonthAndYear(string monthAndYearText)
        {
            var withoutCharactersExceptPipe = ReplaceCharactersExceptPipeToEmptySpace(monthAndYearText).Trim();
            string[] items = SplitByEmptySpace(withoutCharactersExceptPipe);
            if (items.Length < 2)
            {
                Result.Add(Error, "");
                return false;
            }
            else
            {
                var month = items[0];
                int monthNumber = GetMonthNumber(month);
                if (monthNumber == 0)
                {
                    Result.Add(Error, "");
                    return false;
                }
                else
                {
                    if (Result.ContainsKey(Month1))
                        Result.Add(Month2, monthNumber);
                    else
                        Result.Add(Month1, monthNumber);

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


        bool TryParseRangeWithYear(string yearText)
        {
            //what if year is 2100 or >
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
    }
}
