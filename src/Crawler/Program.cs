using System;
using Crawler.Core;
using Crawler.Core.Model;
using Crawler.Core.Crawlers;

namespace Crawler
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new ConsoleLogger();
            try
            {
                var loader = new HtmlDocumentLoader();
                var repository = new CrawlerRepository();
                //var crawlers = new ICrawler[] { new RabotaUaCrawler(logger), new CareersStackoverfowComCrawler(logger) };
                var crawlers = new ICrawler[] { new CareersStackoverfowComCrawler(logger) };
                foreach (var crawler in crawlers)
                {
                    crawler.Crawl(loader, repository);                           
                }
            }
            catch (Exception e)
            {
                logger.Log("FAILED exception caught in Main() method. Exception message: " + e.Message);
                logger.Log(e.StackTrace);
            }
        }
    }
}
