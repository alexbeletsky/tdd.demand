using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crawler.Core.Crawlers
{
    public class CareersStackoverfowComCrawler : CrawlerImpl, ICrawler
    {
        private string _baseUrl = @"http://careers.stackoverflow.com";
        private string _searchBaseUrl = @"http://careers.stackoverflow.com/Jobs?searchTerm=.net,java,c%2B%2B&searchType=Any&location=&range=20";

        public CareersStackoverfowComCrawler(ILogger logger)
        {
            Logger = logger;
        }

        public void Crawle(IHtmlDocumentLoader loader, ICrawlerRepository context)
        {
            Loader = loader;
            Repository = context;
        }

        protected override string BaseUrl
        {
            get { throw new NotImplementedException(); }
        }

        protected override string SearchBaseUrl
        {
            get { throw new NotImplementedException(); }
        }

        protected override IEnumerable<HtmlAgilityPack.HtmlNode> GetJobRows(HtmlAgilityPack.HtmlDocument document)
        {
            throw new NotImplementedException();
        }

        protected override string CreateNextUrl(int nextPage)
        {
            throw new NotImplementedException();
        }

        protected override string GetVacancyUrl(HtmlAgilityPack.HtmlNode row)
        {
            throw new NotImplementedException();
        }

        protected override string GetVacancyBody(HtmlAgilityPack.HtmlDocument htmlDocument)
        {
            throw new NotImplementedException();
        }

        protected override string GetPosition(HtmlAgilityPack.HtmlNode row)
        {
            throw new NotImplementedException();
        }

        protected override string GetCompany(HtmlAgilityPack.HtmlNode row)
        {
            throw new NotImplementedException();
        }
    }
}
