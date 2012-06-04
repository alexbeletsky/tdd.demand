using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crawler.Core.Matchers
{
    public class TddMatcher : IMatcher
    {
        private static IList<string> _patterns = new List<string>()
            {
                "tdd",
                "bdd",
                "junit",
                "nunit",
                "xunit",
                "unit",
                "test driven",
                "test driven development",
                "unit test",
                "unit tests",
                "unit testing",
                "cppunit",
                "moq",
                "mocks",
                "mocks",
                "development by tests"
            };

        
        public bool Match(string input)
        {
            return MatchUtil.Match(input, _patterns);
        }
    }
}
