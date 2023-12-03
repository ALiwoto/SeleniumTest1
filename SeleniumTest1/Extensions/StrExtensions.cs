using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumTest1.Extensions
{
    public static class StrExtensions
    {
        private static readonly Dictionary<string, bool> _spaceIgnoredStrs = new()
        {
            {"(", true},
            {")", true},
            {"[]", true},
        };
        public static string GetNumericChars(this string input, string defaultValue = "") 
        {
            string result = string.Empty;
            foreach (var currentChar in input)
            {
                if (char.IsNumber(currentChar) || currentChar == '.')
                    result += currentChar;
            }

            if (string.IsNullOrEmpty(result))
                return defaultValue;

            return result;
        }
        public static string JoinProgrammicParts(this IEnumerable<string> parts)
        {
            var finalValue = new StringBuilder();
            foreach (var part in parts)
            {
                if (_spaceIgnoredStrs.GetValueOrDefault(part))
                {

                    finalValue.Append(part);
                }
                else
                {
                    finalValue.Append(' ');
                    finalValue.Append(part);
                }
            }

            return finalValue.ToString();
        }
    }
}
