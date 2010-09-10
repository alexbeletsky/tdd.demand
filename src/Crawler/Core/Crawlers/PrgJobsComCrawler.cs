using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crawler.Core.Domain;
using Crawler.Core.Matchers;

namespace Crawler.Core.Crawlers
{
    public class PrgJobsComCrawler : ICrawler
    {
        private static readonly string _baseUrl = @"http://www.prgjobs.com/";
        private static readonly string _searchBaseUrl = @"http://www.prgjobs.com/jobout.cfm?ApplicantSearchArea=&SearchText=";
        private IHtmlDocumentLoader _loader;
        private ICrawlerRepository _context;
        private ILogger _logger;

        public PrgJobsComCrawler(ILogger logger)
        {
            _logger = logger;
        }
        
        public void Crawle(IHtmlDocumentLoader loader, ICrawlerRepository context)
        {
            _loader = loader;
            _context = context;

            CleanUp();
            StartCrawling();
        }

        private void StartCrawling()
        {
            _logger.Log(_baseUrl + " crawler started...");

            for (var nextPage = 1; ; nextPage++)
            {
                //TEMP: just to avoid crawling of dumb data..
                if (nextPage > 20)
                {
                    _logger.Log("crawled more than 20 pages, that it.. break");
                    break;
                }


                var url = CreateNextUrl(nextPage);
                var document = _loader.LoadDocument(url);

                _logger.Log("processing page: [" + nextPage.ToString() + "] with url: " + url); 

                var rows = GetJobRows(document);
                var rowsCount = rows.Count();

                _logger.Log("extracted " + rowsCount + " vacations on page");
                if (rowsCount == 0)
                {
                    _logger.Log("no more vacancies to process, breaking main loop");
                    break;
                }

                _logger.Log("starting to process all vacancies");
                foreach (var row in rows)
                {
                    _logger.Log("starting processing div, extracting vacancy href...");
                    var vacancyUrl = GetVacancyUrl(row);
                    if (vacancyUrl == null)
                    {
                        _logger.Log("FAILED to extract vacancy href, not stopped, proceed with next one");
                        continue;
                    }

                    _logger.Log("started to process vacancy with url: " + vacancyUrl);
                    var vacancyBody = GetVacancyBody(_loader.LoadDocument(vacancyUrl));
                    if (vacancyBody == null)
                    {
                        _logger.Log("FAILED to extract vacancy body, not stopped, proceed with next one");
                        continue;
                    }

                    var position = GetPosition(row);
                    var company = GetCompany(row);
                    var technology = GetTechnology(position);
                    var demand = GetDemand(vacancyBody);

                    var record = new TddDemandRecord()
                    {
                        Site = _baseUrl,
                        Company = company,
                        Position = position,
                        Technology = technology,
                        Demand = demand,
                        Url = vacancyUrl
                    };

                    _logger.Log("new record has been created and initialized");
                    _context.Add(record);
                    _context.SaveChanges();
                    _logger.Log("record has been successfully stored to database.");
                    _logger.Log("finished to process vacancy");

                }
                _logger.Log("finished to process page");
            }
            _logger.Log(_baseUrl + " crawler has successfully finished");
        }

        private string GetVacancyBody(HtmlAgilityPack.HtmlDocument htmlDocument)
        {
            var node = htmlDocument.DocumentNode.SelectSingleNode("/html/body/div/center/table/tr[2]/td/div/table/tr[1]/td/p/font");
            return node.InnerText;
        }

        private string GetPosition(HtmlAgilityPack.HtmlNode row)
        {
            return row.Descendants("td").ElementAt(1).Descendants("a").Single().InnerText;
        }

        private string GetCompany(HtmlAgilityPack.HtmlNode row)
        {
            return row.Descendants("td").ElementAt(2).Descendants("em").Single().InnerText;
        }

        private string GetVacancyUrl(HtmlAgilityPack.HtmlNode row)
        {
            return row.Descendants("td").ElementAt(1).Descendants("a").Single().Attributes["href"].Value;
        }

        private static IEnumerable<HtmlAgilityPack.HtmlNode> GetJobRows(HtmlAgilityPack.HtmlDocument document)
        {
            var table = document.DocumentNode.Descendants("table").ElementAtOrDefault(2);
            if (table == null)
            {
                return new List<HtmlAgilityPack.HtmlNode>();
            }

            var rows = table.Descendants("tr")
                .Where(r => r.ChildNodes.Count > 3);
            return rows;
        }

        private string CreateNextUrl(int nextPage)
        {
            return _searchBaseUrl + "&Page=" + nextPage;
        }

        private void CleanUp()
        {
            _logger.Log("removing database records from last crawler run...");
            var records = _context.GetBySiteName(_baseUrl);
            foreach (var record in records)
            {
                _context.Delete(record);
            }
            _context.SaveChanges();
            _logger.Log("all records successfully removed");
        }

        private bool GetDemand(string vacancyBody)
        {
            return MatchToTdd(vacancyBody);
        }

        private static string GetTechnology(string position)
        {
            var technology = string.Empty;
            if (MatchToJava(position))
            {
                technology = "Java";
            }
            else if (MatchToCpp(position))
            {
                technology = "Cpp";
            }
            else if (MatchToDotNet(position))
            {
                technology = "DotNet";
            }
            else
            {
                technology = "Other";
            }

            return technology;
        }

        private bool MatchToTdd(string description)
        {
            return new TddMatcher().Match(description);
        }

        private static bool MatchToJava(string desciption)
        {
            return new JavaMatcher().Match(desciption);
        }

        private static bool MatchToCpp(string desciption)
        {
            return new CppMatcher().Match(desciption);
        }

        private static bool MatchToDotNet(string desciption)
        {
            return new DotNetMatcher().Match(desciption);
        }

    }
}
