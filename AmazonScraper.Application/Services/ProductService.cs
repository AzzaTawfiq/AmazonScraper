using AmazonScraper.Core.Entities;
using AmazonScraper.Core.Interfaces;
using AmazonScraper.Infrastructure.Scraper;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
        public async Task<List<Product>> GetProductAsync()
        {
            //var cached = await _cache.GetStringAsync($"prod:{id}");
            //if (cached != null) return JsonSerializer.Deserialize<Product>(cached);

            //var product = await _scraperService.ScrapeAmazonProductAsync();

            //// Cache for 60 minutes
            //await _cache.SetStringAsync($"prod:{id}", JsonSerializer.Serialize(product),
            //    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60) });

            var productsList = await _scraperService.ScrapeAmazonProductAsync();

            //string cacheKey = "all_products_list";

            //// 1. Fetch JSON string from PostgreSQL cache
            //string? cachedData = await _cache.GetStringAsync(cacheKey);

            //if (!string.IsNullOrEmpty(cachedData))
            //{
            //    // 2. Deserialize back into a C# list
            //    return JsonSerializer.Deserialize<List<Product>>(cachedData);
            //}

            //var productsList = await _scraperService.ScrapeAmazonProductAsync();

            //// 1. Serialize the list to JSON
            //string jsonString = JsonSerializer.Serialize(productsList);

            //// 2. Set expiration options
            //var options = new DistributedCacheEntryOptions
            //{
            //    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(
            //        60), // Expires in 10 mins
            //    SlidingExpiration = TimeSpan.FromMinutes(60) // Kept alive if requested within 2 mins
            //};

            //// 3. Save to PostgreSQL cache
            //await _cache.SetStringAsync(cacheKey, jsonString, options);

            return productsList;
        }

        public async Task<List<Product>> GetProductsbyPageAsync(string? query, int page = 1, bool refresh = false)
        {
            string cacheKey = query == null ? $"all_products_list:all:" + page : $"all_products_list:" + query + ":" + page;

            // 1. Fetch JSON string from PostgreSQL cache
            string? cachedData = await _cache.GetStringAsync(cacheKey);

            if (!refresh && !string.IsNullOrEmpty(cachedData) && cachedData != "[]")
            {
                // 2. Deserialize back into a C# list
                return JsonSerializer.Deserialize<List<Product>>(cachedData);
            }

            var productsList = await _scraperService.ScrapeAmazonProductsbyPageAsync(query, page);

            // 1. Serialize the list to JSON
            string jsonString = JsonSerializer.Serialize(productsList);

            // 2. Set expiration options
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(
                    60), // Expires in 60 mins
                SlidingExpiration = TimeSpan.FromMinutes(60) // Kept alive if requested within 60 mins
            };

            // 3. Save to PostgreSQL cache
            await _cache.SetStringAsync(cacheKey, jsonString, options);

            return productsList;
        }

        public async Task<Product> GetProductbyASINAsync(string asin)
        {
            //var cached = await _cache.GetStringAsync($"prod:{id}");
            //if (cached != null) return JsonSerializer.Deserialize<Product>(cached);

            //var product = await _scraperService.ScrapeAmazonProductAsync();

            //// Cache for 60 minutes
            //await _cache.SetStringAsync($"prod:{id}", JsonSerializer.Serialize(product),
            //    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60) });

            var product = await _scraperService.ScrapeAmazonProductbyASINAsync(asin);

            //string cacheKey = "all_products_list";

            //// 1. Fetch JSON string from PostgreSQL cache
            //string? cachedData = await _cache.GetStringAsync(cacheKey);

            //if (!string.IsNullOrEmpty(cachedData))
            //{
            //    // 2. Deserialize back into a C# list
            //    return JsonSerializer.Deserialize<List<Product>>(cachedData);
            //}

            //var productsList = await _scraperService.ScrapeAmazonProductAsync();

            //// 1. Serialize the list to JSON
            //string jsonString = JsonSerializer.Serialize(productsList);

            //// 2. Set expiration options
            //var options = new DistributedCacheEntryOptions
            //{
            //    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(
            //        60), // Expires in 10 mins
            //    SlidingExpiration = TimeSpan.FromMinutes(60) // Kept alive if requested within 2 mins
            //};

            //// 3. Save to PostgreSQL cache
            //await _cache.SetStringAsync(cacheKey, jsonString, options);

            return product;
        }
    }
}
