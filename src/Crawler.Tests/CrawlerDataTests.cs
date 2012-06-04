using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Crawler.Core;
using Crawler.Core.Domain;
using System.Transactions;
using Crawler.Core.Model;

namespace Crawler.Tests
{
    /// <summary>
    /// CrawlerDataTests
    /// 
    /// Simple test cases, to check EF4 Code-first features
    /// </summary>
    [TestFixture]
    public class CrawlerDataTests
    {
        private static readonly ICrawlerRepository _context = new CrawlerRepository();

        [TestFixtureSetUp]
        public static void Setup()
        {
            _context.Database.Delete();
            _context.Database.Create();
        }

        [TestFixtureTearDown]
        public static void TearDown()
        {
        }

        [Test]
        public void AddNewRecord()
        {
            //arrange
            var record = new TddDemandRecord
            {
                Demand = false,
                Site = "http://localhost",
                Technology = "C#"
            };

            //act
            _context.Add(record);
            _context.SaveChanges();

            //assert
            var found = _context.GetBySiteName("http://localhost").First();
            Assert.That(found, Is.Not.Null);
        }
    }
}
