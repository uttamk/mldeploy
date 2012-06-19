using System;

namespace Lib.MLDeploy
{
    internal static class StringExtensions
    {
        internal static bool IsLong(this char character)
        {
            long temp;
            return Int64.TryParse(new string(new[] { character }), out temp);
        }
        
        internal static long ToLong(this char character)
        {
            return Int64.Parse(new string(new[] { character }));
        }
        
        internal static long ToLong(this string text)
        {
            return Int64.Parse(text);
        }
    }
}