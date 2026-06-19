using HtmlAgilityPack;

namespace AmazonScraper.Core.Interfaces
{
    public interface IScrapingStrategy
    {
        public interface IScrapingStrategy<T>
        {
            string Name { get; }
            T ScrapePage(HtmlDocument htmlDocument);
        }
    }
}
