using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace PeriodParser
{
    class Program
    {
        //static void Main(string[] args)
        //{
        //    var text = "12 June, 2018- August, 2020 Monthly or 06.18-08/20 or Jun 18 - Aug 20";

        //    var notNumbers = Regex.Split(text, @"\d+");    // non digits
        //    var numbers = Regex.Split(text.Replace(" ", ""), @"\D+");   // digits only, but may start with blank value

        //    var characters = Regex.Split(text, @"[-/*/]");  // split by - or /

        //    var substrings = text.Split();  // split by empty space
        //    var substring2 = text.Split(' ', '.', StringSplitOptions.RemoveEmptyEntries);   // split by empty space, but remove dots

        //    var withouotSpecCharacters = Regex.Replace(text, @"[^0-9a-zA-Z]+", " ");    // get rid of characters
        //    var withouotSpecCharacters2 = Regex.Replace(text, @"[^0-9a-zA-Z-,]+", " "); // get rid of characters except - and ,

        //    var splitHyphens = text.Split('-'); // split by -
        //    var splitHyphens2 = text.Substring(0, text.IndexOf("-")).Trim(); // split by - and get first

        //    var monthNumber = GetMonthNumber("01");
        //    var monthNumber2 = GetMonthNumber("1");
        //    var monthNumber3 = GetMonthNumber("-1");
        //    var monthNumber4 = GetMonthNumber("13");
        //    var monthNumber5 = GetMonthNumber("10");
        //    var monthNumber6 = GetMonthNumber("010");
        //    var monthNumber7 = GetMonthNumber("may");
        //    var monthNumber8 = GetMonthNumber("May");
        //    var monthNumber9 = GetMonthNumber("Apr");
        //    var monthNumber10 = GetMonthNumber("April");
        //    var monthNumber11 = GetMonthNumber("apr");
        //    var monthNumber12 = GetMonthNumber("april");

        //    var test1 = "".Split("-");  //length = 1
        //    var test2 = "asddsa".Split("-");    //length = 1
        //    var test3 = "asd - asddsa".Split("-");  //length = 2



        //    Console.WriteLine("Hello World!");
        //}

        static int GetMonthNumber(string text)
        {
            int monthNumber = 0;
            if (int.TryParse(text, out monthNumber))
            {
                if (monthNumber < 1 || monthNumber > 12)
                {
                    //Result.Add("Error", "");
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

        string[] GetNumbers(string text)
        {
            return Regex.Split(text, @"\d+");
        }

        string[] GetOperands(string text)
        {
            return Regex.Split(text, @"\s+");
        }

        string[] GetCharacters(string text)
        {
            return Regex.Split(text, @"[-+*/]");
        }

        string[] GetSubstrings(string text)
        {
            return text.Split();
        }

        string[] GetSubstrings2(string text)
        {
            return text.Split(' ', '.');
        }

        string[] GetSubstrings3(string text)
        {
            return text.Split(' ', '.', StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
