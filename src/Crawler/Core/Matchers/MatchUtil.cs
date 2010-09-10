using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Crawler.Core.Matchers
{
    class MatchUtil
    {
        public static bool Match(string input, IList<string> patterns)
        {
            var lower = input.ToLower();
            foreach (var pattern in patterns)
            {
                var start = pattern.StartsWith("\\.") ? "(?!\\w)" : "\\b";
                if (Regex.IsMatch(lower, start + pattern + "(?!\\w)"))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
