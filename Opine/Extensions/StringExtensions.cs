using System;

namespace Opine
{
    public static class StringExtensions
    {
        public static bool EqualsTo(this string @this, string value)
        {
            return string.Equals(@this, value, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}