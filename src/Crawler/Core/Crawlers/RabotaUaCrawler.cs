using System.Collections.Generic;
using System.Linq;

namespace Crawler.Core.Crawlers
{
    public class RabotaUaCrawler : CrawlerBase, ICrawler
    {
        private string _baseUrl = @"http://rabota.ua";
        private string _searchBaseUrl = @"http://rabota.ua/jobsearch/vacancy_list?rubricIds=8,9&keyWords=&parentId=1";

        public RabotaUaCrawler(ILogger logger)
        {
            Logger = logger;
        }

        public void Crawl(IHtmlDocumentLoader loader, ICrawlerRepository context)
        {
            Loader = loader;
            Repository = context;

            StartCrawling();
        }

        protected override string BaseUrl
        {
            get { return _baseUrl; }
        }

        protected override string SearchBaseUrl
        {
            get { return _searchBaseUrl; }
        }

        protected override IEnumerable<HtmlAgilityPack.HtmlNode> GetJobRows(HtmlAgilityPack.HtmlDocument document)
        {
            var vacancyDivs = document.DocumentNode.Descendants("tr")
                .Where(d =>
                    d.Attributes.Contains("class") &&
                    d.Attributes["class"].Value.Contains("vacancy-list-item"));
            return vacancyDivs;
        }

        protected override string GetVacancyUrl(HtmlAgilityPack.HtmlNode div)
        {
            var vacancyHref = div.Descendants("a").Where(
                d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("vacancy-title"))
                .Select(d => d.Attributes["href"].Value).SingleOrDefault();
            return BaseUrl + vacancyHref;
        }

        protected override string CreateNextUrl(int nextPage)
        {
            return SearchBaseUrl + "&pg=" + nextPage;
        }

        protected override string GetVacancyBody(HtmlAgilityPack.HtmlDocument vacancyPage)
        {
            var nodes = vacancyPage.DocumentNode.Descendants("div").Where(
                d => d.Attributes.Contains("class") && (d.Attributes["class"].Value.Equals("d_des") || d.Attributes["class"].Value.Equals("descr")));
            var body = nodes.Aggregate("", (current, node) => current + node.InnerText);

            return body;
        }


        protected override string GetPosition(HtmlAgilityPack.HtmlNode div)
        {
            return div.Descendants("a").Where(
               d => d.Attributes.Contains("class") &&
               d.Attributes["class"].Value.Contains("rua-b-vacancy-title") || d.Attributes["class"].Value.Contains("jqKeywordHighlight")
               ).Select(d => d.InnerText).First();
        }
    }
}
