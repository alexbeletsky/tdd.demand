using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crawler.Core.Crawlers
{
    public class CareersStackoverfowComCrawler : ICrawler
    {
        private static readonly string _baseUrl = @"http://careers.stackoverflow.com";
        private static readonly string _searchBaseUrl = @"http://careers.stackoverflow.com/Jobs?searchTerm=.net,java,c%2B%2B&searchType=Any&location=&range=20";
        private IHtmlDocumentLoader _loader;
        private ICrawlerRepository _context;
        private ILogger _logger;

        public CareersStackoverfowComCrawler(ILogger logger)
        {
            _logger = logger;
        }

        public void Crawle(IHtmlDocumentLoader loader, ICrawlerRepository context)
        {
            _loader = loader;
            _context = context;
        }
    }
}
