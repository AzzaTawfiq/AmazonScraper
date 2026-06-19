using HtmlAgilityPack;
using AmazonScraper.Core.Interfaces;
using static AmazonScraper.Core.Interfaces.IScrapingStrategy;

namespace AmazonScraper.Infrastructure.Scraper
{
    public class ScraperService<T> : IScraperService<T>
    {
        private readonly IClientHandler _clientHandler;
        private readonly IEnumerable<IScrapingStrategy<T>> _strategies;

        public ScraperService(IClientHandler clientHandler,
            IEnumerable<IScrapingStrategy<T>> strategies)
        {
            _clientHandler = clientHandler;
            _strategies = strategies;
        }

        public async Task<T> ScrapeDataAsync(string url, string scraperName)
        {
            var html = await _clientHandler.GetHTMLData(url);

            var document = GetHtmlDoc(html);

            // Dynamically find the correct strategy based on type name or an internal identifier
            var strategy = _strategies.FirstOrDefault(s => s.Name == scraperName);

            if (strategy != null)
                return strategy.ScrapePage(document);

            //await Task.Delay(Random.Shared.Next(3000, 5000));

            throw new InvalidOperationException($"Strategy '{scraperName}' not found.");//default(T);
        }

        private HtmlDocument GetHtmlDoc(string htmlContent)
        {
            // 1. Load HTML into HtmlAgilityPack Document
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);
            return htmlDoc;
        }

    }

}
