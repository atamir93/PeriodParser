using System.Collections.Generic;
using System.Linq;

namespace PeriodParser
{
    public class ProfitAndLossViewParser
    {
        public const string DimensionDefinition = " by ";
        public const string YearToDateDefinition = "ytd";
        public const string LastDefinition = "last";
        public const string FiscalDefinition = "fiscal";
        public readonly string[] YearDefinitions = { "yearly", "years", "year" };
        public readonly string[] QuarterDefinitions = { "quarterly", "quarters", "quarter" };
        public readonly string[] QuarterNumbers = { "q1", "q2", "q3", "q4" };
        public readonly string[] MonthDefinitions = { "monthly", "months", "month" };
        public readonly string[] TotalDefinitions = { "total", "totals" };
        public readonly string[] SeasonsDefinitions = { "seasons", "season" };

        public Dictionary<string, object> Parse(string text, ProfitAndLossView view)
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

            if (parser?.Result == null || parser.Result.ContainsKey("Error"))
                return null;

            return parser.Result;
        }

        public static bool EndsWithAny(string[] items, string text) => items.Any(i => text.EndsWith(i));
        public static bool ContainsAny(string[] items, string text) => items.Any(text.Contains);
    }
}
