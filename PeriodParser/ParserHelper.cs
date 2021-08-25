using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PeriodParser
{
    public static class ParserHelper
    {

        public static int GetFirstNumber(string text)
        {
            string[] numbers = GetNumbers(text);
            int number;
            if (numbers.Length == 0 || !int.TryParse(numbers[0], out number))
                number = 0;

            return number;
        }

        public static string[] GetNumbers(string text) => Regex.Split(text, @"\D+").Where(n => !string.IsNullOrEmpty(n)).ToArray();

        public static bool EndsWithAny(string[] items, string text) => items.Any(i => text.EndsWith(i));
        public static bool ContainsAny(string[] items, string text) => items.Any(text.Contains);

        static string ReplaceCharactersExceptPipeToEmptyEntry(string text)
        {
            return Regex.Replace(text.Trim(), @"[^0-9a-zA-Z|]+", " ");
        }
        static string ReplaceCharactersExceptPipeAndDashToEmptyEntry(string text)
        {
            return Regex.Replace(text.Trim(), @"[^0-9a-zA-Z|-]+", " ");
        }

        static int GetMonthNumber(string text)
        {
            if (int.TryParse(text, out int monthNumber) && (monthNumber < 1 || monthNumber > 12))
            {
                monthNumber = 0;
            }
            else
            {
                var shortName = text.Substring(0, 3);
                if (DateTime.TryParseExact(shortName, "MMM", CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime result))
                    monthNumber = result.Month;
            }
            return monthNumber;
        }
    }
}
