using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crawler.Core.Domain;
using System.Text.RegularExpressions;
using Crawler.Core.Matchers;

namespace Crawler.Core.Crawlers
{
    public class RabotaUaCrawler : ICrawler
    {
        private static readonly string _baseUrl = @"http://rabota.ua";
        private static readonly string _searchBaseUrl = @"http://rabota.ua/jobsearch/vacancy_list?rubricIds=8,9&keyWords=&parentId=1";
        private IHtmlDocumentLoader _loader;
        private ICrawlerRepository _context;
        private ILogger _logger;

        public RabotaUaCrawler(ILogger logger)
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

            for (var nextPage = 0; ; nextPage++)
            {
                var url = CreateNextUrl(nextPage);

                _logger.Log("processing page: [" + nextPage.ToString() + "] with url: " + url); 

                var vacancyDivs = GetVacancyDivs(_loader.LoadDocument(url));
                var vacancyDivsCount = vacancyDivs.Count();

                _logger.Log("extracted " + vacancyDivsCount.ToString() + " vacations on page");
                if (vacancyDivsCount == 0)
                {
                    _logger.Log("no more vacancies to process, breaking main loop");
                    break;
                }

                _logger.Log("starting to process all vacancies");
                foreach (var div in vacancyDivs)
                {
                    _logger.Log("starting processing div, extracting vacancy href...");
                    var vacancyHref = GetVacancyHref(div);
                    if (vacancyHref == null)
                    {
                        _logger.Log("FAILED to extract vacancy href, not stopped, proceed with next one");
                        continue;
                    }

                    var vacancyUrl = CreateVacancyUrl(vacancyHref);
                    _logger.Log("started to process vacancy with url: " + vacancyUrl);
                    var vacancyBody = GetVacancyBody(_loader.LoadDocument(vacancyUrl));
                    if (vacancyBody == null)
                    {
                        _logger.Log("FAILED to extract vacancy body, not stopped, proceed with next one");
                        continue;
                    }

                    _logger.Log("extracted vacancy body successfully, start to processing...");
                    
                    var position = GetPosition(div);
                    var company = GetCompany(div);
                    var technology = GetTechnology(div, position);
                    var demand = GetDemand(div, vacancyBody);

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

        private static IEnumerable<HtmlAgilityPack.HtmlNode> GetVacancyDivs(HtmlAgilityPack.HtmlDocument resultsPage)
        {
            var vacancyDivs = resultsPage.DocumentNode.Descendants("div")
                .Where(d =>
                    d.Attributes.Contains("class") &&
                    d.Attributes["class"].Value.Contains("vacancyitem"));
            return vacancyDivs;
        }

        private static string GetVacancyHref(HtmlAgilityPack.HtmlNode div)
        {
            var vacancyHref = div.Descendants("a").Where(
                d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("vacancyDescription"))
                .Select(d => d.Attributes["href"].Value).SingleOrDefault();
            return vacancyHref;
        }

        private string CreateVacancyUrl(string vacancyHref)
        {
            return _baseUrl + vacancyHref;
        }

        private string CreateNextUrl(int nextPage)
        {
            return _searchBaseUrl + "&pg=" + (nextPage + 1).ToString();
        }

        private static string GetVacancyBody(HtmlAgilityPack.HtmlDocument vacancyPage)
        {
            if (vacancyPage == null)
            {
                //TODO: log event here and skip this page
                return null;
            }

            var description = vacancyPage.DocumentNode.Descendants("div")
                .Where(
                    d => d.Attributes.Contains("id") && d.Attributes["id"].Value.Contains("ctl00_centerZone_vcVwPopup_pnlBody"))
                .Select(d => d.InnerHtml).SingleOrDefault();
            return description;
        }


        private static string GetPosition(HtmlAgilityPack.HtmlNode div)
        {
            return div.Descendants("a").Where(
               d => d.Attributes.Contains("class") &&
               d.Attributes["class"].Value.Contains("vacancyName") || d.Attributes["class"].Value.Contains("jqKeywordHighlight")
               ).Select(d => d.InnerText).First();
        }

        private static string GetCompany(HtmlAgilityPack.HtmlNode div)
        {
            return div.Descendants("div").Where(
                d => d.Attributes.Contains("class") &&
                d.Attributes["class"].Value.Contains("companyName")).Select(d => d.FirstChild.InnerText).First();
        }

        private bool GetDemand(HtmlAgilityPack.HtmlNode div, string vacancyDesciption)
        {
            return MatchToTdd(vacancyDesciption);
        }

        private static string GetTechnology(HtmlAgilityPack.HtmlNode div, string postion)
        {
            var technology = string.Empty;
            if (MatchToJava(postion))
            {
                technology = "Java";
            }
            else if (MatchToCpp(postion))
            {
                technology = "Cpp";
            }
            else if (MatchToDotNet(postion))
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
