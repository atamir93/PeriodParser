using System.Collections.Generic;

namespace PeriodParser
{
    public class ProfitAndLossView
    {
        public string PeriodText { get; set; }
        public int BeginMonth { get; set; }
        public int BeginningMonthSingle { get; set; }
        public int BeginningMonthRange { get; set; }
        public int EndingMonth { get; set; }
        public int BeginYear { get; set; }
        public int BeginningYearSingle { get; set; }
        public int BeginningYearYtd { get; set; }
        public int BeginningYearMonthRange { get; set; }
        public int BeginningYearYearly { get; set; }
        public int BeginningYearQuarterly { get; set; }
        public int BeginningYearMonthly { get; set; }
        public int EndingYear { get; set; }
        public Quarter Quarter { get; set; }
        public int MonthlyPeriodDifference { get; set; }
        public int QuarterlyPeriodDifference { get; set; }
        public List<string> FiltersFromParser { get; set; } = new List<string>();
        public string DimensionToCompare { get; set; }
        public DimensionCompareType DimensionCompareType { get; set; }
        public ProfitAndLossPeriod Period { get; set; }
        public EachYearOrConsecutive YearlyOrConsecutive { get; set; }
        public YearlySwitch YearlyType { get; set; }
    }
    public enum Quarter
    {
        Q2 = 2,
        Q3 = 3,
        Q4 = 4,
        Q1 = 1,
    }

    public enum DimensionCompareType
    {
        Month = 3,
        Quarter = 2,
        Range = 4,
        EntireYear = 0,
        YearToDate = 1,
    }

    public enum ProfitAndLossPeriod
    {
        Single = 0,
        Yearly = 1,
        MonthRange = 4,
        Quarterly = 2,
        Dimension = 5,
        Monthly = 3,
    }

    public enum EachYearOrConsecutive
    {
        Consecutive = 1,
        EachYear = 0,
    }

    public enum YearlySwitch
    {
        YearToDate = 1,
        EntireYear = 0,
    }
}
