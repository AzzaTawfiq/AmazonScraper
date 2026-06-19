
namespace AmazonScraper.Core.Interfaces
{
    public interface IScraperService<T>
    {
        Task<T> ScrapeDataAsync(string url, string scraperName);
    }
}
