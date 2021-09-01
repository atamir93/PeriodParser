using PeriodParser.RegexParser;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace PeriodParser
{

    public class ParserFromPeriodTextBL
    {
        static string DimensionDefinition = "by";
        static string YearToDateDefinition = "ytd";
        static readonly string[] YearDefinitions = { "yearly", "years", "year" };
        static readonly string[] QuarterDefinitions = { "quarterly", "quarters", "quarter" };
        static readonly string[] QuarterNumbers = { "q1", "q2", "q3", "q4" };
        static readonly string[] MonthDefinitions = { "monthly", "months", "month" };
        static readonly string[] TotalDefinitions = { "totals", "total" };
        static readonly string[] SeasonsDefinitions = { "seasons", "season" };

        public static Dictionary<string, object> Parse(string text, ProfitAndLossView view)
        {
            text = text.ToLower().Trim();
            view.FiltersFromParser.Clear();
            RegexParser.PeriodParser parser = null;
            if (text.Contains(DimensionDefinition))
            {
                parser = DimensionParser.GetInstance();
            }
            else if (text.Contains(YearToDateDefinition))
            {
                parser = YearToDateParser.GetInstance();
            }
            else if (ContainsAny(QuarterDefinitions, text) || ContainsAny(QuarterNumbers, text))
            {
                parser = QuarterParser.GetInstance();
            }
            else if (EndsWithAny(SeasonsDefinitions, text))
            {
                parser = SeasonsParser.GetInstance();
            }
            else if (EndsWithAny(TotalDefinitions, text))
            {
                parser = TotalParser.GetInstance();
            }
            else if (ContainsAny(MonthDefinitions, text) || text.EndsWith(" m") || text.Contains("for last"))
            {
                parser = MonthlyParser.GetInstance();
            }
            else if (ContainsAny(YearDefinitions, text))
            {
                parser = EntireYearParser.GetInstance();
            }
            else if (text.EndsWith(" t"))
            {
                parser = TotalParser.GetInstance();
            }
            else if (text.EndsWith(" s"))
            {
                parser = SeasonsParser.GetInstance();
            }
            else
            {
                var period = GetMatchedPeriod(text, view.Period);
                switch (period)
                {
                    case ProfitAndLossPeriod.Single:
                        parser = TotalParser.GetInstance();
                        break;
                    case ProfitAndLossPeriod.Yearly:
                        if (view.Period == ProfitAndLossPeriod.Yearly && view.YearlyType == YearlySwitch.YearToDate)
                            parser = YearToDateParser.GetInstance();
                        else
                            parser = EntireYearParser.GetInstance();
                        break;
                    case ProfitAndLossPeriod.MonthRange:
                        parser = SeasonsParser.GetInstance();
                        break;
                    case ProfitAndLossPeriod.Quarterly:
                        parser = QuarterParser.GetInstance();
                        break;
                    case ProfitAndLossPeriod.Dimension:
                        parser = DimensionParser.GetInstance();
                        break;
                    case ProfitAndLossPeriod.Monthly:
                        parser = MonthlyParser.GetInstance();
                        break;
                    default:
                        break;
                }
            }
            parser.SetDates(new DateTime(2020, 5, 1), view.EndingMonth);
            parser.SetPeriodText(text);
            var parseSucceeded = parser.TryParse();
            parser.SetAllEndingFields();
            if (parseSucceeded && parser?.Result != null && !parser.Result.ContainsKey("Error") && parser.Result.ContainsKey("Period"))
                return parser.Result;

            return null;
        }

        static Regex GetRegexForYearRanges() => new Regex(@"^(\d+)\s*-\s*(\d+)\s*$");
        static Regex GetRegexForSingleMonthName() => new Regex(@"^(jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec)\w*\s*$");
        static Regex GetRegexForMonthRanges() => new Regex(@"^(jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec)\w*\s*-\s*(jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec)\w*\s*$");

        static ProfitAndLossPeriod GetMatchedPeriod(string text, ProfitAndLossPeriod currentPeriod)
        {
            ProfitAndLossPeriod period = currentPeriod;
            if (ContainsOnlyMonthName(text))
            {
                period = ProfitAndLossPeriod.Monthly;
            }
            else if (currentPeriod != ProfitAndLossPeriod.Single)
            {
                if (ContainsOnlyMonthRanges(text))
                {
                    period = ProfitAndLossPeriod.Monthly;
                }
                else if (ContainsOnlyYearRanges(text))
                {
                    period = ProfitAndLossPeriod.Yearly;
                }
            }
            return period;
        }

        static bool ContainsOnlyMonthName(string text)
        {
            Regex rgx = GetRegexForSingleMonthName();
            return rgx.IsMatch(text);
        }
        static bool ContainsOnlyMonthRanges(string text)
        {
            Regex rgx = GetRegexForMonthRanges();
            return rgx.IsMatch(text);
        }
        static bool ContainsOnlyYearRanges(string text)
        {
            Regex rgx = GetRegexForYearRanges();
            return rgx.IsMatch(text);
        }

        public static string AutocorrectParseResult(Dictionary<string, object> parserResult)
        {
            object period;
            if (parserResult.TryGetValue("Period", out period) && period is ProfitAndLossPeriod plPeriod)
            {
                var month1 = GetIntValue(parserResult, "Month1");
                var month2 = GetIntValue(parserResult, "Month2");
                string monthName1 = (month1 > 0 && month1 <= 12) ? GetMonthName(month1) : string.Empty;
                string monthName2 = (month2 > 0 && month2 <= 12) ? GetMonthName(month2) : string.Empty;
                var year1 = GetStringValue(parserResult, "Year1");
                var year2 = GetStringValue(parserResult, "Year2");
                var quarter1 = GetStringValue(parserResult, "Quarter1");
                var quarter2 = GetStringValue(parserResult, "Quarter2");
                var type = GetStringValue(parserResult, "Type");

                switch (plPeriod)
                {
                    case ProfitAndLossPeriod.Single:
                        return ParserToPeriodTextBL.GetSinglePeriodText(monthName1, monthName2, year1, year2);
                    case ProfitAndLossPeriod.Yearly:
                        if (type == "YTD" || type == "EntireYear")
                        {
                            if (type == "EntireYear")
                            {
                                return ParserToPeriodTextBL.GetEntireYearYearlyPeriodText(year1, year2);
                            }
                            else
                            {
                                return ParserToPeriodTextBL.GetYearToDateYearlyPeriodText(monthName1, year1, year2);
                            }
                        }
                        break;
                    case ProfitAndLossPeriod.MonthRange:
                        return ParserToPeriodTextBL.GetMonthRangePeriodText(monthName1, monthName2, year1, year2);
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
                            return ParserToPeriodTextBL.GetEachYearMonthlyPeriodText(monthName1, year1, year2);
                        }
                        else if (type == "Consecutive")
                        {
                            return ParserToPeriodTextBL.GetConsecutiveMonthlyPeriodText(monthName1, monthName2, year1, year2);
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
        }

        public static void DistributeToProfitAndLossPeriod(ProfitAndLossView view)
        {
            if (view.FiltersFromParser != null && view.FiltersFromParser.Any())
            {
                var parserResult = new Dictionary<string, object>();
                foreach (var item in view.FiltersFromParser)
                {
                    var keyAndValue = item.Split("=");
                    var key = keyAndValue[0];
                    var value = keyAndValue[1];
                    if (key == "Period")
                    {
                        object periodValue;
                        if (Enum.TryParse(typeof(ProfitAndLossPeriod), value, out periodValue))
                            parserResult.Add(key, periodValue);
                    }
                    else if (key == "DimensionCompareType")
                    {
                        object dimensionPeriodValue;
                        if (Enum.TryParse(typeof(DimensionCompareType), value, out dimensionPeriodValue))
                            parserResult.Add(key, dimensionPeriodValue);
                    }
                    else
                    {
                        parserResult.Add(key, value);
                    }
                }

                object period;
                if (parserResult.TryGetValue("Period", out period) && period is ProfitAndLossPeriod plPeriod)
                {
                    var month1 = GetIntValue(parserResult, "Month1");
                    if (month1 == 0)
                        month1 = view.EndingMonth;
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
                                view.BeginningYearYtd = year1;
                            }
                            else
                            {
                                view.YearlyType = YearlySwitch.EntireYear;
                                view.BeginningYearYearly = year1;
                            }
                            view.EndingYear = year2;
                            view.EndingMonth = month1;
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
                                view.Quarter = (Quarter)quarter1;
                            }
                            else if (type == "Consecutive")
                            {
                                view.YearlyOrConsecutive = EachYearOrConsecutive.Consecutive;
                                view.EndingYear = year2;
                                view.Quarter = (Quarter)quarter2;
                                //view.BeginningYearQuarterly = year1;
                                view.QuarterlyPeriodDifference = GetQuarterlyPeriodDifference(quarter1, quarter2, year1, year2);
                            }
                            break;
                        case ProfitAndLossPeriod.Monthly:
                            if (type == "EachYear")
                            {
                                view.YearlyOrConsecutive = EachYearOrConsecutive.EachYear;
                                view.BeginningYearMonthly = year1;
                                view.EndingYear = year2;
                                view.EndingMonth = month1;
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

                view.FiltersFromParser.Clear();
            }
        }

        static void SetDimensionPeriod(Dictionary<string, object> parserResult, ProfitAndLossView view)
        {
            var dimensionName = GetStringValue(parserResult, "DimensionName");
            //var dimension = (view.Context as IServerContext).DataProvider.GetEntities<Dimension>().SingleOrDefault(d => d.Name.ToLower().Trim() == dimensionName);
            if (dimensionName != null && parserResult.ContainsKey("DimensionCompareType"))
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
                            view.EndingMonth = month2;
                            view.EndingYear = year2;
                            view.BeginningMonthSingle = month1;
                            view.BeginningYearSingle = year1;
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
            //var dimension = context.DataProvider.GetEntities<Dimension>().SingleOrDefault(d => d.Name.ToLower().Trim() == dimensionName);
            if (dimensionName != null && parserResult.ContainsKey("DimensionCompareType"))
            {
                var dimensionCompareType = parserResult["DimensionCompareType"];
                if (dimensionCompareType is DimensionCompareType type)
                {
                    var month1 = GetIntValue(parserResult, "Month1");
                    var month2 = GetIntValue(parserResult, "Month2");
                    string monthName1 = (month1 > 0 && month1 <= 12) ? GetMonthName(month1) : string.Empty;
                    string monthName2 = (month2 > 0 && month2 <= 12) ? GetMonthName(month2) : string.Empty;
                    var year1 = GetStringValue(parserResult, "Year1");
                    var year2 = GetStringValue(parserResult, "Year2");
                    var quarter1 = GetStringValue(parserResult, "Quarter1");
                    switch (type)
                    {
                        case DimensionCompareType.Month:
                            return ParserToPeriodTextBL.GetMonthDimensionPeriodText(monthName1, year1, dimensionName);
                        case DimensionCompareType.Quarter:
                            return ParserToPeriodTextBL.GetQuarterDimensionPeriodText(quarter1, year1, dimensionName);
                        case DimensionCompareType.Range:
                            return ParserToPeriodTextBL.GetRangeDimensionPeriodText(monthName1, monthName2, year1, year2, dimensionName);
                        case DimensionCompareType.EntireYear:
                            return ParserToPeriodTextBL.GetEntireYearDimensionPeriodText(year1, dimensionName);
                        case DimensionCompareType.YearToDate:
                            return ParserToPeriodTextBL.GetYearToDateDimensionPeriodText(monthName1, year1, dimensionName);
                        default:
                            break;
                    }
                }
            }
            return string.Empty;
        }
        static string GetMonthName(int monthNumber) => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthNumber);

        static string GetStringValue(Dictionary<string, object> parserResult, string key)
        {
            return parserResult.ContainsKey(key) ? parserResult[key].ToString() : string.Empty;
        }
        static int GetIntValue(Dictionary<string, object> parserResult, string key)
        {
            if (parserResult.ContainsKey(key) && int.TryParse(parserResult[key].ToString(), out int intValue))
            {
                return intValue;
            }
            return 0;
        }

        public static bool EndsWithAny(string[] items, string text) => items.Any(i => text.EndsWith(i));
        public static bool ContainsAny(string[] items, string text) => items.Any(text.Contains);
    }
}