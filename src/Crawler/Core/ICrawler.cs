using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crawler.Core
{
    public interface ICrawler
    {
        void Crawl(IHtmlDocumentLoader loader, ICrawlerRepository context);
    }
}
