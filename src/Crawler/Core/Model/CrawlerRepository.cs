using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using Crawler.Core.Domain;

namespace Crawler.Core.Model
{
    internal class CrawlerDataContext : DbContext
    {
        public CrawlerDataContext() : base("crawlerdb")
        {
            
        }

        public DbSet<TddDemandRecord> Records { get; set; }
    }

    public class CrawlerRepository : ICrawlerRepository
    {
        private readonly CrawlerDataContext _context = new CrawlerDataContext();

        public void Add(TddDemandRecord record)
        {
            _context.Records.Add(record);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public Database Database
        {
            get { return _context.Database; }
        }


        public void Delete(TddDemandRecord record)
        {
            _context.Records.Remove(record);
        }

        public IEnumerable<TddDemandRecord> GetBySiteName(string site)
        {
            return _context.Records.Where(r => r.Site == site);
        }
    }
}
