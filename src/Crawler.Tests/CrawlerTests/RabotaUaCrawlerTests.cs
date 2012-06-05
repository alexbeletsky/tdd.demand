using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Crawler.Core.Crawlers;
using Moq;
using Crawler.Core;
using HtmlAgilityPack;
using Crawler.Tests.Properties;
using System.IO;
using Crawler.Core.Domain;

namespace Crawler.Tests.CrawlerTests
{
    [TestFixture]
    public class RabotaUaCrawlerTests
    {
        private static ILogger _logger = new Mock<ILogger>().Object;

        [Test]
        public void Smoke()
        {
            //arrange
            var crawler = new RabotaUaCrawler(_logger);

            //act/assert
            Assert.That(crawler, Is.Not.Null);
        }

        [Test]
        public void CrawlOnePage()
        {
            //arrange
            var loader = new Mock<IHtmlDocumentLoader>();
            var context = new Mock<ICrawlerRepository>();
            var crawler = new RabotaUaCrawler(_logger);

            var document = new HtmlDocument();
            document.Load(new FileStream("./TestData/rabotaua/rabotaua.results.htm", FileMode.Open));
            loader.Setup(l => l.LoadDocument("http://rabota.ua/jobsearch/vacancy_list?rubricIds=8,9&keyWords=&parentId=1&pg=1")).Returns(document);
            loader.Setup(l => l.LoadDocument("http://rabota.ua/jobsearch/vacancy_list?rubricIds=8,9&keyWords=&parentId=1&pg=2")).Returns(new HtmlDocument());
            var vacancy = new HtmlDocument();
            vacancy.Load(new FileStream("./TestData/rabotaua/dnet.withtdd.htm", FileMode.Open));
            loader.Setup(l => l.LoadDocument(It.IsRegex(@"http://rabota.ua/company\d+/vacancy\d+"))).
                Returns(vacancy);

            var storage = new List<TddDemandRecord>();
            context.Setup(c => c.Add(It.IsAny<TddDemandRecord>())).Callback((TddDemandRecord r) => storage.Add(r));
            
            //act
            crawler.Crawl(loader.Object, context.Object);

            //assert
            context.Verify(c => c.SaveChanges());
            Assert.That(storage.Count, Is.EqualTo(20), "Expected that all 20 divs processed");
        }

        [Test]
        public void CouldGetVacancyHrefAndLoadPage()
        {
            //arrange
            var loader = new Mock<IHtmlDocumentLoader>();
            var context = new Mock<ICrawlerRepository>();
            var crawler = new RabotaUaCrawler(_logger);

            var resultsPage = new HtmlDocument();
            resultsPage.Load(new FileStream("TestData/rabotaua/rabotaua.java.htm", FileMode.Open));
            loader.Setup(l => l.LoadDocument("http://rabota.ua/jobsearch/vacancy_list?rubricIds=8,9&keyWords=&parentId=1&pg=1")).
                Returns(resultsPage);
            //empty page to stop crawler
            loader.Setup(l => l.LoadDocument("http://rabota.ua/jobsearch/vacancy_list?rubricIds=8,9&keyWords=&parentId=1&pg=2")).
                Returns(new HtmlDocument());
            var vacancy = new HtmlDocument();
            vacancy.Load(new FileStream("TestData/rabotaua/dnet.withtdd.htm", FileMode.Open));
            loader.Setup(l => l.LoadDocument(It.IsRegex(@"http://rabota.ua/company\d+/vacancy\d+"))).
                Returns(vacancy);

            var storage = new List<TddDemandRecord>();
            context.Setup(c => c.Add(It.IsAny<TddDemandRecord>())).Callback((TddDemandRecord r) => storage.Add(r));

            //act
            crawler.Crawl(loader.Object, context.Object);

            //assert
            context.Verify(c => c.SaveChanges());
            Assert.That(storage.Count, Is.EqualTo(1), "Expected that all 1 divs processed");
        }

        private TddDemandRecord ProcessPagesAndReturnFirstRecord(string results, string vacancy)
        {
            //arrange
            var loader = new Mock<IHtmlDocumentLoader>();
            var context = new Mock<ICrawlerRepository>();
            var crawler = new RabotaUaCrawler(_logger);

            var resultsPage = new HtmlDocument();
            resultsPage.Load(new FileStream(results, FileMode.Open));
            loader.Setup(l => l.LoadDocument("http://rabota.ua/jobsearch/vacancy_list?rubricIds=8,9&keyWords=&parentId=1&pg=1")).
                Returns(resultsPage);
            //empty page to stop crawler
            loader.Setup(l => l.LoadDocument("http://rabota.ua/jobsearch/vacancy_list?rubricIds=8,9&keyWords=&parentId=1&pg=2")).
                Returns(new HtmlDocument());
            var vacancyPage = new HtmlDocument();
            vacancyPage.Load(new FileStream(vacancy, FileMode.Open));
            loader.Setup(l => l.LoadDocument(It.IsRegex(@"http://rabota.ua/company\d+/vacancy\d+"))).
                            Returns(vacancyPage);

            var storage = new List<TddDemandRecord>();
            context.Setup(c => c.Add(It.IsAny<TddDemandRecord>())).Callback((TddDemandRecord r) => storage.Add(r));

            //act
            crawler.Crawl(loader.Object, context.Object);

            //return
            return storage.First();
        }

        [Test]
        public void JavaWithTdd()
        {
            //assert
            var record = ProcessPagesAndReturnFirstRecord("TestData/rabotaua/rabotaua.java.htm", "TestData/rabotaua/java.withtdd.htm");
            Assert.That(record.Site, Is.EqualTo("http://rabota.ua"));
            Assert.That(record.Position, Is.EqualTo("Senior Java Developer"));
            Assert.That(record.Technology, Is.EqualTo("Java"));
            Assert.That(record.Demand, Is.True);
        }
    }
}
