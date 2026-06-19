using HtmlAgilityPack;

namespace AmazonScraper.Core.Interfaces
{
    public interface IScrapingStrategy
    {
        public interface IScrapingStrategy<T>
        {
            T ScrapePage(HtmlDocument htmlDocument);
        }
    }
}
