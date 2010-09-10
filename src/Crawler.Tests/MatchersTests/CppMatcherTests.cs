using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Crawler.Core.Matchers;

namespace Crawler.Tests.MatchersTests
{
    [TestFixture]
    public class CppMatcherTests
    {
        [Test]
        public void Smoke()
        {
            //arrange
            var matcher = new CppMatcher();

            //act/assert
            Assert.That(matcher, Is.Not.Null);
        }

        [Test]
        public void MatchCpp()
        {
            //arrange
            var matcher = new CppMatcher();
            var input = "c++ required";
            
            //act
            var result = matcher.Match(input);

            //act/assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void MatchCpp2()
        {
            //arrange
            var matcher = new CppMatcher();
            var input = "required c++";

            //act
            var result = matcher.Match(input);

            //act/assert
            Assert.That(result, Is.True);
        }


        [Test]
        public void MatchCpp3()
        {
            //arrange
            var matcher = new CppMatcher();
            var input = "required C++";

            //act
            var result = matcher.Match(input);

            //act/assert
            Assert.That(result, Is.True);
        }

    }
}
