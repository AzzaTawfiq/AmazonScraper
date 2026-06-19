using AmazonScraper.Core.Entities;
using AmazonScraper.Core.Interfaces;
using AmazonScraper.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Distributed;

namespace AmazonScraper.Application.Services
{
    public class ProductService: IProductService
    {
        private readonly IScraperService _scraperService;
        private readonly IDistributedCache _cache;
        //private readonly IAmazonClient _client;
        public ProductService(IScraperService scraperService , IDistributedCache cache) {
            _scraperService = scraperService;
            _cache = cache;
        }

        public async Task<List<Product>> GetProductsbyPageAsync(string? query, int page = 1, bool refresh = false)
        {
            string cacheKey = query == null ? $"all_products_list:all:" + page : $"all_products_list:" + query + ":" + page;
            var cachedData = await _cache.GetCacheAsync<List<Product>>(cacheKey);

            if (!refresh && cachedData != null)
            {
                return cachedData;
            }

            var productsList = await _scraperService.ScrapeAmazonProductsbyPageAsync(query, page);

            await _cache.SetCacheAsync(cacheKey, productsList, 60); // cache for 1 hr

            return productsList;
        }

        public async Task<Product> GetProductbyASINAsync(string asin)
        {
            string cacheKey = $"ProductDetails:" + asin;
            var cachedData = await _cache.GetCacheAsync<Product>(cacheKey);

            if (cachedData != null)
            {
                return cachedData;
            }

            var product = await _scraperService.ScrapeAmazonProductbyASINAsync(asin);

            await _cache.SetCacheAsync(cacheKey, product, 360);  // cache for 6 hrs

            return product;
        }
    }
}
