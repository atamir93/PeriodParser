using System.Collections.Generic;
using System.Linq;

namespace PeriodParser
{
    public static class ProfitAndLossViewParser
    {
        static string DimensionDefinition = "by";
        static string YearToDateDefinition = "ytd";
        static readonly string[] YearDefinitions = { "yearly", "years", "year" };
        static readonly string[] QuarterDefinitions = { "quarterly", "quarters", "quarter" };
        static readonly string[] QuarterNumbers = { "q1", "q2", "q3", "q4" };
        static readonly string[] MonthDefinitions = { "monthly", "months", "month" };
        static readonly string[] TotalDefinitions = { "total", "totals" };
        static readonly string[] SeasonsDefinitions = { "seasons", "season" };

        public static Dictionary<string, object> Parse(string text, ProfitAndLossView view)
        {
            var lastPeriod = view.Period;
            view.FiltersFromParser.Clear();
            PeriodParser parser = null;
            if (text.Contains(DimensionDefinition))
            {
                parser = DimensionParser.GetInstance(text);
            }
            else if (text.Contains(YearToDateDefinition))
            {
                parser = YearToDateParser.GetInstance(text);
            }
            else if (ContainsAny(QuarterDefinitions, text) || ContainsAny(QuarterNumbers, text))
            {
                parser = QuarterParser.GetInstance(text);
            }
            else if (EndsWithAny(SeasonsDefinitions, text))
            {
                parser = SeasonsParser.GetInstance(text);
            }
            else if (EndsWithAny(TotalDefinitions, text))
            {
                parser = TotalParser.GetInstance(text);
            }
            else if (ContainsAny(MonthDefinitions, text))
            {
                parser = MonthlyParser.GetInstance(text);
            }
            else if (ContainsAny(YearDefinitions, text))
            {
                parser = EntireYearParser.GetInstance(text);
            }
            else
            {
                switch (lastPeriod)
                {
                    case ProfitAndLossPeriod.Single:
                        parser = TotalParser.GetInstance(text);
                        break;
                    case ProfitAndLossPeriod.Yearly:
                        parser = EntireYearParser.GetInstance(text);
                        break;
                    case ProfitAndLossPeriod.MonthRange:
                        parser = SeasonsParser.GetInstance(text);
                        break;
                    case ProfitAndLossPeriod.Quarterly:
                        parser = QuarterParser.GetInstance(text);
                        break;
                    case ProfitAndLossPeriod.Dimension:
                        parser = DimensionParser.GetInstance(text);
                        break;
                    case ProfitAndLossPeriod.Monthly:
                        parser = MonthlyParser.GetInstance(text);
                        break;
                    default:
                        break;
                }
            }

            if (parser?.Result != null && !parser.Result.ContainsKey("Error") && parser.Result.ContainsKey("Period"))
                return parser.Result;

            return null;
        }

        public static string AutocorrectParseResult(Dictionary<string, object> parserResult)
        {
            object period;
            if (parserResult.TryGetValue("Period", out period) && period is ProfitAndLossPeriod plPeriod)
            {
                var month1 = GetStringValue(parserResult, "Month1");
                var month2 = GetStringValue(parserResult, "Month2");
                var year1 = GetStringValue(parserResult, "Year1");
                var year2 = GetStringValue(parserResult, "Year2");
                var quarter1 = GetStringValue(parserResult, "Quarter1");
                var quarter2 = GetStringValue(parserResult, "Quarter2");
                var type = GetStringValue(parserResult, "Type");

                switch (plPeriod)
                {
                    case ProfitAndLossPeriod.Single:
                        return ParserToPeriodTextBL.GetSinglePeriodText(month1, month2, year1, year2);
                    case ProfitAndLossPeriod.Yearly:
                        if (type == "YTD" || type == "Yearly")
                        {
                            return ParserToPeriodTextBL.GetYearlyPeriodText(month1, type, year1, year2);
                        }
                        break;
                    case ProfitAndLossPeriod.MonthRange:
                        return ParserToPeriodTextBL.GetMonthRangePeriodText(month1, month2, year1, year2);
                    case ProfitAndLossPeriod.Quarterly:
                        if (type == "EachYear")
                        {
                            return ParserToPeriodTextBL.GetEachYearQuarterlyPeriodText(quarter1, year1, year2);
                        }
                        else if (type == "Consecutive")
                        {
                            return ParserToPeriodTextBL.GetConsecutiveQuarterlyPeriodText(quarter1, quarter2, year1, year2);
                        }
                        break;
                    case ProfitAndLossPeriod.Monthly:
                        if (type == "EachYear")
                        {
                            return ParserToPeriodTextBL.GetEachYearMonthlyPeriodText(month1, year1, year2);
                        }
                        else if (type == "Consecutive")
                        {
                            return ParserToPeriodTextBL.GetConsecutiveMonthlyPeriodText(month1, month2, year1, year2);
                        }
                        break;
                    case ProfitAndLossPeriod.Dimension:
                        return GetDimensionValue(parserResult);
                    default:
                        break;
                }
            }
            return string.Empty;
        }

        public static void StoreParserResult(Dictionary<string, object> parserResult, ProfitAndLossView view)
        {
            view.FiltersFromParser.Clear();
            foreach (var item in parserResult)
            {
                view.FiltersFromParser.Add($"{item.Key}={item.Value}");
            }
            object period;
            if (parserResult.TryGetValue("Period", out period) && period is ProfitAndLossPeriod plPeriod)
            {
                var month1 = GetStringValue(parserResult, "Month1");
                var month2 = GetStringValue(parserResult, "Month2");
                var year1 = GetStringValue(parserResult, "Year1");
                var year2 = GetStringValue(parserResult, "Year2");
                var quarter1 = GetStringValue(parserResult, "Quarter1");
                var quarter2 = GetStringValue(parserResult, "Quarter2");
                var type = GetStringValue(parserResult, "Type");

                switch (plPeriod)
                {
                    case ProfitAndLossPeriod.Single:
                        ParserToPeriodTextBL.GetSinglePeriodText(month1, month2, year1, year2);
                        break;

                    case ProfitAndLossPeriod.Yearly:
                        if (type == "YTD" || type == "Yearly")
                        {
                            ParserToPeriodTextBL.GetYearlyPeriodText(month1, type, year1, year2);
                        }
                        break;
                    case ProfitAndLossPeriod.MonthRange:
                        ParserToPeriodTextBL.GetMonthRangePeriodText(month1, month2, year1, year2);
                        break;

                    case ProfitAndLossPeriod.Quarterly:
                        if (type == "EachYear")
                        {
                            ParserToPeriodTextBL.GetEachYearQuarterlyPeriodText(quarter1, year1, year2);
                        }
                        else if (type == "Consecutive")
                        {
                            ParserToPeriodTextBL.GetConsecutiveQuarterlyPeriodText(quarter1, quarter2, year1, year2);
                        }
                        break;
                    case ProfitAndLossPeriod.Monthly:
                        if (type == "EachYear")
                        {
                            ParserToPeriodTextBL.GetEachYearMonthlyPeriodText(month1, year1, year2);
                        }
                        else if (type == "Consecutive")
                        {
                            ParserToPeriodTextBL.GetConsecutiveMonthlyPeriodText(month1, month2, year1, year2);
                        }
                        break;
                    case ProfitAndLossPeriod.Dimension:
                        GetDimensionValue(parserResult);
                        break;

                    default:
                        break;
                }
            }
        }

        public static void DistributeToProfitAndLossPeriod(ProfitAndLossView view)
        {
            var parserResult = new Dictionary<string, object>();
            foreach (var item in view.FiltersFromParser)
            {
                var keyAndValue = item.Split("=");
                parserResult.Add(keyAndValue[0], keyAndValue[1]);
            }

            object period;
            if (parserResult.TryGetValue("Period", out period) && period is ProfitAndLossPeriod plPeriod)
            {
                var month1 = GetIntValue(parserResult, "Month1");
                var month2 = GetIntValue(parserResult, "Month2");
                var year1 = GetIntValue(parserResult, "Year1");
                var year2 = GetIntValue(parserResult, "Year2");
                var quarter1 = GetIntValue(parserResult, "Quarter1");
                var quarter2 = GetIntValue(parserResult, "Quarter2");
                var type = GetStringValue(parserResult, "Type");

                view.Period = plPeriod;
                switch (plPeriod)
                {
                    case ProfitAndLossPeriod.Single:
                        view.BeginningMonthSingle = month1;
                        view.EndingMonth = month2;
                        view.BeginningYearSingle = year1;
                        view.EndingYear = year2;
                        break;

                    case ProfitAndLossPeriod.Yearly:
                        if (type == "YTD")
                        {
                            view.YearlyType = YearlySwitch.YearToDate;
                            view.BeginningYearYearly = year1;
                        }
                        else
                        {
                            view.YearlyType = YearlySwitch.EntireYear;
                            view.BeginningYearYtd = year1;
                        }
                        view.EndingYear = year2;
                        view.EndingMonth = month2;
                        break;
                    case ProfitAndLossPeriod.MonthRange:
                        view.BeginningMonthRange = month1;
                        view.EndingMonth = month2;
                        view.BeginningYearMonthRange = year1;
                        view.EndingYear = year2;
                        break;

                    case ProfitAndLossPeriod.Quarterly:
                        if (type == "EachYear")
                        {
                            view.YearlyOrConsecutive = EachYearOrConsecutive.EachYear;
                            view.BeginningYearQuarterly = year1;
                            view.EndingYear = year2;
                            view.Quarter = (Quarter)quarter2;
                        }
                        else if (type == "Consecutive")
                        {
                            view.YearlyOrConsecutive = EachYearOrConsecutive.Consecutive;
                            view.EndingYear = year2;
                            view.Quarter = (Quarter)quarter2;
                            view.QuarterlyPeriodDifference = GetQuarterlyPeriodDifference(quarter1, quarter2, year1, year2);
                        }
                        break;
                    case ProfitAndLossPeriod.Monthly:
                        if (type == "EachYear")
                        {
                            view.YearlyOrConsecutive = EachYearOrConsecutive.EachYear;
                            view.BeginningYearMonthly = year1;
                            view.EndingYear = year2;
                            view.EndingMonth = month2;
                        }
                        else if (type == "Consecutive")
                        {
                            view.YearlyOrConsecutive = EachYearOrConsecutive.Consecutive;
                            view.EndingYear = year2;
                            view.EndingMonth = month2;
                            view.MonthlyPeriodDifference = GetMonthlyPeriodDifference(month1, month2, year1, year2);
                        }
                        break;
                    case ProfitAndLossPeriod.Dimension:
                        SetDimensionPeriod(parserResult, view);
                        break;
                    default:
                        break;
                }
            }
        }

        static void SetDimensionPeriod(Dictionary<string, object> parserResult, ProfitAndLossView view)
        {
            var dimensionName = GetStringValue(parserResult, "DimensionName");
            if (!string.IsNullOrEmpty(dimensionName) && parserResult.ContainsKey("DimensionCompareType"))
            {
                var dimensionCompareType = parserResult["DimensionCompareType"];
                if (dimensionCompareType is DimensionCompareType type)
                {
                    view.Period = ProfitAndLossPeriod.Dimension;
                    view.DimensionToCompare = dimensionName;
                    view.DimensionCompareType = type;

                    var month1 = GetIntValue(parserResult, "Month1");
                    var month2 = GetIntValue(parserResult, "Month2");
                    var year1 = GetIntValue(parserResult, "Year1");
                    var year2 = GetIntValue(parserResult, "Year2");
                    var quarter1 = GetIntValue(parserResult, "Quarter1");
                    switch (type)
                    {
                        case DimensionCompareType.Month:
                            view.EndingMonth = month1;
                            view.EndingYear = year1;
                            break;
                        case DimensionCompareType.Quarter:
                            view.Quarter = (Quarter)quarter1;
                            view.EndingYear = year1;
                            break;
                        case DimensionCompareType.Range:
                            view.BeginningMonthRange = month1;
                            view.EndingMonth = month2;
                            view.BeginningYearSingle = year1;
                            view.EndingYear = year2;
                            break;
                        case DimensionCompareType.EntireYear:
                            view.EndingMonth = month1;
                            view.EndingYear = year1;
                            break;
                        case DimensionCompareType.YearToDate:
                            view.EndingMonth = month1;
                            view.EndingYear = year1;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        static int GetQuarterlyPeriodDifference(int beginQ, int endingQ, int beginY, int endingY)
        {
            return ((endingY - beginY) * 4) + (endingQ - beginQ);
        }

        static int GetMonthlyPeriodDifference(int beginM, int endingM, int beginY, int endingY)
        {
            return ((endingY - beginY) * 12) + (endingM - beginM);
        }

        static string GetDimensionValue(Dictionary<string, object> parserResult)
        {
            var dimensionName = GetStringValue(parserResult, "DimensionName");
            if (!string.IsNullOrEmpty(dimensionName) && parserResult.ContainsKey("DimensionCompareType"))
            {
                var dimensionCompareType = parserResult["DimensionCompareType"];
                if (dimensionCompareType is DimensionCompareType type)
                {
                    var month1 = GetStringValue(parserResult, "Month1");
                    var month2 = GetStringValue(parserResult, "Month2");
                    var year1 = GetStringValue(parserResult, "Year1");
                    var year2 = GetStringValue(parserResult, "Year2");
                    var quarter1 = GetStringValue(parserResult, "Quarter1");
                    switch (type)
                    {
                        case DimensionCompareType.Month:
                            return ParserToPeriodTextBL.GetMonthDimensionPeriodText(month1, year1, dimensionName);
                        case DimensionCompareType.Quarter:
                            return ParserToPeriodTextBL.GetQuarterDimensionPeriodText(quarter1, year1, dimensionName);
                        case DimensionCompareType.Range:
                            return ParserToPeriodTextBL.GetRangeDimensionPeriodText(month1, month2, year1, year2, dimensionName);
                        case DimensionCompareType.EntireYear:
                            return ParserToPeriodTextBL.GetEntireYearDimensionPeriodText(year1, dimensionName);
                        case DimensionCompareType.YearToDate:
                            return ParserToPeriodTextBL.GetYearToDateDimensionPeriodText(month1, year1, dimensionName);
                        default:
                            break;
                    }
                }
            }
            return string.Empty;
        }

        static string GetStringValue(Dictionary<string, object> parserResult, string key)
        {
            return parserResult.ContainsKey(key) ? parserResult[key].ToString() : string.Empty;
        }
        static int GetIntValue(Dictionary<string, object> parserResult, string key)
        {
            return parserResult.ContainsKey(key) && parserResult[key] is int ? (int)parserResult[key] : 0;
        }

        public static bool EndsWithAny(string[] items, string text) => items.Any(i => text.EndsWith(i));
        public static bool ContainsAny(string[] items, string text) => items.Any(text.Contains);
    }
}
