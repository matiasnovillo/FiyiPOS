using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FiyiPOS.Library
{
    public static class Validator
    {
        public static bool IsRegexOk(string StringToAnalyze, string Pattern)
        {
            try
            {
                Regex rgx = new(Pattern);

                if (!rgx.IsMatch(StringToAnalyze)) { return false; }
                else { return true; }
            }
            catch (Exception) { throw; }
        }

        public static bool IsHexColour(string HexColourInStringFormat)
        {
            try
            {
                if (HexColourInStringFormat.Length > 6) { return false; }
                return IsRegexOk(HexColourInStringFormat, @"^[a-fA-F0-9]+$");
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Returns 'A' if stringA is greater than stringB and viceversa <br/>
        /// Returns 'E' if stringA is equal to stringB <br/>
        /// Returns '\0' if the strings have different length while <c>equalLength = true</c> <br/>
        /// </summary>
        public static char CompareStrings(string stringA, string stringB, bool equalLength = false)
        {
            try
            {
                if (equalLength && (stringA.Length != stringB.Length))
                { return '\0'; }

                char Result = char.MinValue;
                int Indicator = stringA.CompareTo(stringB);
                if (Indicator == 0)
                {
                    return 'E';
                }
                else if (Indicator == 1)
                {
                    return 'A';
                }
                else if (Indicator == -1)
                {
                    return 'B';
                }
                return Result;
            }
            catch (Exception) { throw; }
        }
    }
}
