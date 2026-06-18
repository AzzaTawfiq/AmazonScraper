using AmazonScraper.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonScraper.Core.Interfaces
{
    public interface IScraperService
    {
        Task<List<Product>> ScrapeAmazonProductAsync();
        Task<List<Product>> ScrapeAmazonProductsAllPagesAsync(string? query, int page = 1);
        Task<List<Product>> ScrapeAmazonProductsbyPageAsync(string? query, int page = 1);
    }
}
