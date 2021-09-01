using System.Linq;

namespace PeriodParser
{
    public class ProfitAndLossViewParser
    {
        public void Autocorrect(ProfitAndLossView entity)
        {
            string text = entity.PeriodText;
            if (!string.IsNullOrEmpty(text))
            {
                entity.FiltersFromParser.Clear();
                var parserResult = ParserFromPeriodTextBL.Parse(text, entity);
                if (parserResult == null || !parserResult.Any())
                    entity.PeriodText = "";
                else
                {
                    var autocorrectedValue = ParserFromPeriodTextBL.AutocorrectParseResult(parserResult);
                    if (string.IsNullOrEmpty(autocorrectedValue))
                        entity.PeriodText = "";
                    else
                    {
                        ParserFromPeriodTextBL.StoreParserResult(parserResult, entity);
                        entity.PeriodText = autocorrectedValue;
                        Distribute(entity);
                    }
                }
            }
            else
            {
                entity.PeriodText = "";
            }
        }

        public void Distribute(ProfitAndLossView entity)
        {
            ParserFromPeriodTextBL.DistributeToProfitAndLossPeriod(entity);
        }
    }
}