using System;

namespace QuartzTest.Extensions
{
    public static class StringExtensions
    {
        public static int ParseToNumber(this string str)
        {
            if (!int.TryParse(str, out var result))
            {
                throw new Exception($"Cannot Parse value to number, value is:{str}");
            }

            return result;
        }
    }
}