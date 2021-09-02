using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeriodParser.Tests
{
    [TestFixture]
    public class ProfitAndLossViewParserTest
    {
        ProfitAndLossViewParser parser;
        ProfitAndLossView TestEntity;
        const int CurrentYear = 2020;
        const int EndingMonth = 10;

        [SetUp]
        public void SetUp()
        {
            parser = new ProfitAndLossViewParser();
            TestEntity = new ProfitAndLossView() { EndingMonth = EndingMonth, EndingYear = CurrentYear };
        }

        [Test]
        public void ProfitAndLossView_PeriodText_YearlyTest()
        {
            TestEntity.PeriodText = "18-20 yearly";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("2018 - 2020 Yearly"));
            AssertEntireYearPeriod(EndingMonth, 2018, 2020);

            TestEntity.PeriodText = "last 3 years";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("2018 - 2020 Yearly"));
            AssertEntireYearPeriod(EndingMonth, 2018, 2020);

            TestEntity.PeriodText = "June, 2016-2021";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("2016 - 2021 Yearly"));
            AssertEntireYearPeriod(6, 2016, 2021);
        }

        void AssertEntireYearPeriod(int endingMonth, int beginYear, int endingYear)
        {
            Assert.AreEqual(TestEntity.Period, ProfitAndLossPeriod.Yearly);
            Assert.AreEqual(TestEntity.YearlyType, YearlySwitch.EntireYear);
            Assert.AreEqual(TestEntity.EndingMonth, endingMonth);
            Assert.AreEqual(TestEntity.BeginningYearYearly, beginYear);
            Assert.AreEqual(TestEntity.EndingYear, endingYear);
        }

        [Test]
        public void ProfitAndLossView_PeriodText_YearlyToDateTest()
        {
            TestEntity.PeriodText = "18-20 ytd";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("2018 - 2020 YTD"));
            AssertYearToDatePeriod(EndingMonth, 2018, 2020);

            TestEntity.PeriodText = "last 3 years YTD";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("2018 - 2020 YTD"));
            AssertYearToDatePeriod(EndingMonth, 2018, 2020);

            TestEntity.PeriodText = "06, 2016-2021";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("2016 - 2021 Jun YTD"));
            AssertYearToDatePeriod(6, 2016, 2021);

            var endingMonth = TestEntity.EndingMonth;
            TestEntity.PeriodText = "ytd";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("2020 Jun YTD"));
            AssertYearToDatePeriod(endingMonth, 2020, 2020);
        }

        void AssertYearToDatePeriod(int endMonth, int beginYear, int endingYear)
        {
            Assert.AreEqual(TestEntity.Period, ProfitAndLossPeriod.Yearly);
            Assert.AreEqual(TestEntity.YearlyType, YearlySwitch.YearToDate);
            Assert.AreEqual(TestEntity.EndingMonth, endMonth);
            Assert.AreEqual(TestEntity.BeginningYearYtd, beginYear);
            Assert.AreEqual(TestEntity.EndingYear, endingYear);
        }

        [Test]
        public void ProfitAndLossView_PeriodText_QuarterlyEachYearTest()
        {
            TestEntity.PeriodText = "Q1 2018-2020";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Q1 2018 - 2020"));
            AssertQuarterlyEachYearPeriod(Quarter.Q1, 2018, 2020);

            TestEntity.PeriodText = "Q3 for last 3 years";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Q3 2018 - 2020"));
            AssertQuarterlyEachYearPeriod(Quarter.Q3, 2018, 2020);

            TestEntity.PeriodText = "This Quarter for last 4 years";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Q2 2017 - 2020"));
            AssertQuarterlyEachYearPeriod(Quarter.Q2, 2017, 2020);
        }

        void AssertQuarterlyEachYearPeriod(Quarter quarter, int beginYear, int endingYear)
        {
            Assert.AreEqual(TestEntity.Period, ProfitAndLossPeriod.Quarterly);
            Assert.AreEqual(TestEntity.YearlyOrConsecutive, EachYearOrConsecutive.EachYear);
            Assert.AreEqual(TestEntity.Quarter, quarter);
            Assert.AreEqual(TestEntity.BeginningYearQuarterly, beginYear);
            Assert.AreEqual(TestEntity.EndingYear, endingYear);
        }

        [Test]
        public void ProfitAndLossView_PeriodText_QuarterlyConsecutiveTest()
        {
            TestEntity.PeriodText = "Q1,18 - q3,2020";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Q1, 2018 - Q3, 2020"));
            AssertQuarterlyConsecutivePeriod(Quarter.Q1, Quarter.Q3, 2018, 2020);

            TestEntity.PeriodText = "Q4 - Q3";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Q4, 2019 - Q3, 2020"));
            AssertQuarterlyConsecutivePeriod(Quarter.Q4, Quarter.Q3, 2019, 2020);

            TestEntity.PeriodText = "Last 10 quarters";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Q1, 2018 - Q2, 2020"));
            AssertQuarterlyConsecutivePeriod(Quarter.Q1, Quarter.Q2, 2018, 2020);
        }

        void AssertQuarterlyConsecutivePeriod(Quarter beginQuarter, Quarter endingQuarter, int beginYear, int endingYear)
        {
            Assert.AreEqual(TestEntity.Period, ProfitAndLossPeriod.Quarterly);
            Assert.AreEqual(TestEntity.YearlyOrConsecutive, EachYearOrConsecutive.Consecutive);
            Assert.AreEqual(TestEntity.Quarter, endingQuarter);
            Assert.AreEqual(TestEntity.QuarterlyPeriodDifference, ParserFromPeriodTextBL.GetQuarterlyPeriodDifference((int)beginQuarter, (int)endingQuarter, beginYear, endingYear));
            Assert.AreEqual(TestEntity.EndingYear, endingYear);
        }

        [Test]
        public void ProfitAndLossView_PeriodText_MonthlyEachYearTest()
        {
            TestEntity.PeriodText = "April 2018-2020 m";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Apr 2018 - 2020 Monthly"));
            AssertMonthlyEachYearPeriod(4, 2018, 2020);

            TestEntity.PeriodText = "10 2017-21";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Oct 2017 - 2021 Monthly"));
            AssertMonthlyEachYearPeriod(10, 2017, 2021);

            TestEntity.PeriodText = "This month for last 3 years";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("May 2018 - 2020 Monthly"));
            AssertMonthlyEachYearPeriod(5, 2018, 2020);

            TestEntity.PeriodText = "September for last 2 years";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Sep 2019 - 2020 Monthly"));
            AssertMonthlyEachYearPeriod(9, 2019, 2020);
        }

        void AssertMonthlyEachYearPeriod(int month, int beginYear, int endingYear)
        {
            Assert.AreEqual(TestEntity.Period, ProfitAndLossPeriod.Monthly);
            Assert.AreEqual(TestEntity.YearlyOrConsecutive, EachYearOrConsecutive.EachYear);
            Assert.AreEqual(TestEntity.EndingMonth, month);
            Assert.AreEqual(TestEntity.BeginningYearMonthly, beginYear);
            Assert.AreEqual(TestEntity.EndingYear, endingYear);
        }

        [Test]
        public void ProfitAndLossView_PeriodText_MonthlyConsecutiveTest()
        {
            TestEntity.PeriodText = "June, 2018 - August, 2020 Monthly";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Jun, 2018 - Aug, 2020 Monthly"));
            AssertMonthlyConsecutivePeriod(6, 8, 2018, 2020);

            TestEntity.PeriodText = "Jan - Mar 2020";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Jan - Mar 2020 Monthly"));
            AssertMonthlyConsecutivePeriod(1, 3, 2020, 2020);

            TestEntity.PeriodText = "Apr 17 - Nov 19";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Apr, 2017 - Nov, 2019 Monthly"));
            AssertMonthlyConsecutivePeriod(4, 11, 2017, 2019);

            TestEntity.PeriodText = "Last 14 months";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Mar, 2019 - Apr, 2020 Monthly"));
            AssertMonthlyConsecutivePeriod(3, 4, 2019, 2020);

            TestEntity.PeriodText = "Last 2 months";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Mar - Apr 2020 Monthly"));
            AssertMonthlyConsecutivePeriod(3, 4, 2020, 2020);
        }

        void AssertMonthlyConsecutivePeriod(int month1, int month2, int beginYear, int endingYear)
        {
            Assert.AreEqual(TestEntity.Period, ProfitAndLossPeriod.Monthly);
            Assert.AreEqual(TestEntity.YearlyOrConsecutive, EachYearOrConsecutive.Consecutive);
            Assert.AreEqual(TestEntity.MonthlyPeriodDifference, ParserFromPeriodTextBL.GetMonthlyPeriodDifference(month1, month2, beginYear, endingYear));
            Assert.AreEqual(TestEntity.EndingMonth, month2);
            Assert.AreEqual(TestEntity.EndingYear, endingYear);
        }

        [Test]
        public void ProfitAndLossView_PeriodText_SeasonsTest()
        {
            TestEntity.PeriodText = "04-05 2018-2020 seasons";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Apr - May 2018 - 2020 Seasons"));
            AssertSeasonsPeriod(4, 5, 2018, 2020);

            TestEntity.PeriodText = "Sep-Nov 2019";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Sep - Nov 2019 Seasons"));
            AssertSeasonsPeriod(9, 11, 2019, 2019);

            TestEntity.PeriodText = "04-05 2018-2020 s";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Apr - May 2018 - 2020 Seasons"));
            AssertSeasonsPeriod(4, 5, 2018, 2020);
        }

        void AssertSeasonsPeriod(int month1, int month2, int beginYear, int endingYear)
        {
            Assert.AreEqual(TestEntity.Period, ProfitAndLossPeriod.MonthRange);
            Assert.AreEqual(TestEntity.BeginningMonthRange, month1);
            Assert.AreEqual(TestEntity.BeginningYearMonthRange, beginYear);
            Assert.AreEqual(TestEntity.EndingMonth, month2);
            Assert.AreEqual(TestEntity.EndingYear, endingYear);
        }

        [Test]
        public void ProfitAndLossView_PeriodText_TotalTest()
        {
            TestEntity.PeriodText = "04,2018-05,2020 total";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Apr, 2018 - May, 2020 Total"));
            AssertTotalPeriod(4, 5, 2018, 2020);

            TestEntity.PeriodText = "2017-2020";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Jan, 2017 - Dec, 2020 Total"));
            AssertTotalPeriod(1, 12, 2017, 2020);

            TestEntity.PeriodText = "04,2018-05,2020 t";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Apr, 2018 - May, 2020 Total"));
            AssertTotalPeriod(4, 5, 2018, 2020);
        }

        void AssertTotalPeriod(int month1, int month2, int beginYear, int endingYear)
        {
            Assert.AreEqual(TestEntity.Period, ProfitAndLossPeriod.Single);
            Assert.AreEqual(TestEntity.BeginningMonthSingle, month1);
            Assert.AreEqual(TestEntity.BeginningYearSingle, beginYear);
            Assert.AreEqual(TestEntity.EndingMonth, month2);
            Assert.AreEqual(TestEntity.EndingYear, endingYear);
        }

        [Test]
        public void ProfitAndLossView_PeriodText_DimensionTest()
        {
            //Two words dimension name
            var dimensionName = "sale type";
            TestEntity.PeriodText = $"2018 by {dimensionName}";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo($"2018 by {dimensionName}"));
            AssertYearlyEntireYearPeriod(2018, dimensionName);

            TestEntity.PeriodText = $"2020 by {dimensionName.ToLower()}";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo($"2020 by {dimensionName}"));
            AssertYearlyEntireYearPeriod(2020, dimensionName);

            TestEntity.PeriodText = $"June 2019 ytd by {dimensionName.ToLower()}";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo($"Jun 2019 YTD by {dimensionName}"));
            AssertDimensionPeriod(6, 2019, dimensionName, DimensionCompareType.YearToDate);

            TestEntity.PeriodText = $"Q3 2018 by {dimensionName.ToUpper()}";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo($"Q3 2018 by {dimensionName}"));
            AssertDimensionQuarterlyPeriod(Quarter.Q3, 2018, dimensionName, DimensionCompareType.Quarter);

            //One word dimension name
            dimensionName = "location";

            TestEntity.PeriodText = $"09/19 by {dimensionName.ToLower()}";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo($"Sep 2019 by {dimensionName}"));
            AssertDimensionPeriod(9, 2019, dimensionName, DimensionCompareType.Month);

            TestEntity.PeriodText = $"11/17-03/20 by {dimensionName.ToLower()}";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo($"Nov 2017 - Mar 2020 by {dimensionName}"));
            AssertDimensionRangePeriod(11, 3, 2017, 2020, dimensionName);

            void AssertYearlyEntireYearPeriod(int endingYear, string name)
            {
                Assert.AreEqual(TestEntity.Period, ProfitAndLossPeriod.Dimension);
                Assert.AreEqual(TestEntity.DimensionToCompare, name);
                Assert.AreEqual(TestEntity.DimensionCompareType, DimensionCompareType.EntireYear);
                Assert.AreEqual(TestEntity.EndingYear, endingYear);
            }

            void AssertDimensionPeriod(int month, int endingYear, string name, DimensionCompareType type)
            {
                Assert.AreEqual(TestEntity.Period, ProfitAndLossPeriod.Dimension);
                Assert.AreEqual(TestEntity.DimensionToCompare, name);
                Assert.AreEqual(TestEntity.DimensionCompareType, type);
                Assert.AreEqual(TestEntity.EndingMonth, month);
                Assert.AreEqual(TestEntity.EndingYear, endingYear);
            }

            void AssertDimensionRangePeriod(int month1, int month2, int beginYear, int endingYear, string name)
            {
                Assert.AreEqual(TestEntity.Period, ProfitAndLossPeriod.Dimension);
                Assert.AreEqual(TestEntity.DimensionToCompare, name);
                Assert.AreEqual(TestEntity.DimensionCompareType, DimensionCompareType.Range);
                Assert.AreEqual(TestEntity.BeginningMonthSingle, month1);
                Assert.AreEqual(TestEntity.BeginningYearSingle, beginYear);
                Assert.AreEqual(TestEntity.EndingMonth, month2);
                Assert.AreEqual(TestEntity.EndingYear, endingYear);
            }
        }

        void AssertDimensionQuarterlyPeriod(Quarter quarter, int endingYear, string name, DimensionCompareType type)
        {
            Assert.AreEqual(TestEntity.Period, ProfitAndLossPeriod.Dimension);
            Assert.AreEqual(TestEntity.DimensionToCompare, name);
            Assert.AreEqual(TestEntity.DimensionCompareType, type);
            Assert.AreEqual(TestEntity.Quarter, quarter);
            Assert.AreEqual(TestEntity.EndingYear, endingYear);
        }

        [Test]
        public void ProfitAndLossView_PeriodText_DifferentPeriodsTest()
        {
            //Total
            TestEntity.PeriodText = "04/2018-05/2020 totals";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Apr, 2018 - May, 2020 Total"));
            AssertTotalPeriod(4, 5, 2018, 2020);

            //Dimension quarterly
            var dimensionName = "sale type";
            TestEntity.PeriodText = $"Q3 2018 by {dimensionName.ToLower()}";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo($"Q3 2018 by {dimensionName}"));
            AssertDimensionQuarterlyPeriod(Quarter.Q3, 2018, dimensionName, DimensionCompareType.Quarter);

            //Monthly - Consecutive
            TestEntity.PeriodText = "Last 2 months";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Mar - Apr 2020 Monthly"));
            AssertMonthlyConsecutivePeriod(3, 4, 2020, 2020);

            //Yearly - Year to date
            TestEntity.PeriodText = "Jun 16-21 ytd";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("2016 - 2021 Jun YTD"));
            AssertYearToDatePeriod(6, 2016, 2021);

            //Season
            TestEntity.PeriodText = "09-11 2019 seasons";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Sep - Nov 2019 Seasons"));
            AssertSeasonsPeriod(9, 11, 2019, 2019);

            //Quarterly - Consecutive
            TestEntity.PeriodText = "Q1 2018 - Q3 2020";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Q1, 2018 - Q3, 2020"));
            AssertQuarterlyConsecutivePeriod(Quarter.Q1, Quarter.Q3, 2018, 2020);

            //Monthly - Each year
            TestEntity.PeriodText = "April 2018-2020 m";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Apr 2018 - 2020 Monthly"));
            AssertMonthlyEachYearPeriod(4, 2018, 2020);

            //Yearly - Entire year
            var endingMonthBefore = TestEntity.EndingMonth;
            TestEntity.PeriodText = "last 3 years";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("2018 - 2020 Yearly"));
            AssertEntireYearPeriod(endingMonthBefore, 2018, 2020);

            //Quarterly - Each year
            TestEntity.PeriodText = "Q3 for last 3 years";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Q3 2018 - 2020"));
            AssertQuarterlyEachYearPeriod(Quarter.Q3, 2018, 2020);
        }

        [Test]
        public void ProfitAndLossView_PeriodIsNotDefinedAndPreviousIsNotTotal_Test()
        {
            //Make sure that current period is not Total
            TestEntity.Period = ProfitAndLossPeriod.Quarterly;

            //For month ranges when Total is not selected, assume Monthly for this year
            TestEntity.PeriodText = "Feb - September";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Feb - Sep 2020 Monthly"));
            AssertMonthlyConsecutivePeriod(2, 9, 2020, 2020);

            //For year ranges when Total is not selected, assume Yearly period
            var endingMonthBefore = TestEntity.EndingMonth;
            TestEntity.PeriodText = "2017 - 2019";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("2017 - 2019 Yearly"));
            AssertEntireYearPeriod(endingMonthBefore, 2017, 2019);

            //For single month assume Monthly for this year
            TestEntity.PeriodText = "November ";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Nov 2020 Monthly"));
            AssertMonthlyEachYearPeriod(11, 2020, 2020);

            //For single quarter assume Quarterly of this year
            TestEntity.PeriodText = "Q2";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Q2 2020"));
            AssertQuarterlyEachYearPeriod(Quarter.Q2, 2020, 2020);

            //For month ranges when Total is not selected, assume Monthly for November of last year to February of this year
            TestEntity.PeriodText = "November - February";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Nov, 2019 - Feb, 2020 Monthly"));
            AssertMonthlyConsecutivePeriod(11, 2, 2019, 2020);
        }

        [Test]
        public void ProfitAndLossView_PeriodIsNotDefinedAndPreviousIsTotal_Test()
        {
            //Select Total
            TestEntity.Period = ProfitAndLossPeriod.Single;

            //Assume Total for this month since Total is already selected
            TestEntity.PeriodText = "Feb - September";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Feb - Sep 2020 Total"));
            AssertTotalPeriod(2, 9, 2020, 2020);

            //Assume Total for Jan 2017 - Dec 2019 since Total is already selected
            TestEntity.PeriodText = "2017 - 2019";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Jan, 2017 - Dec, 2019 Total"));
            AssertTotalPeriod(1, 12, 2017, 2019);

            //Assume Total for November of last year to February of this year since Total is already selected
            TestEntity.PeriodText = "November - February";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Nov, 2019 - Feb, 2020 Total"));
            AssertTotalPeriod(11, 2, 2019, 2020);

            //For single quarter assume Quarterly of this year
            TestEntity.PeriodText = "Q2";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Q2 2020"));
            AssertQuarterlyEachYearPeriod(Quarter.Q2, 2020, 2020);

            //Select Total
            TestEntity.Period = ProfitAndLossPeriod.Single;
            //For single month assume Monthly for this year
            TestEntity.PeriodText = "November ";
            parser.Autocorrect(TestEntity);
            Assert.That(TestEntity.PeriodText, Is.EqualTo("Nov 2020 Monthly"));
            AssertMonthlyEachYearPeriod(11, 2020, 2020);
        }
    }
}
