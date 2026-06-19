using AmazonScraper.Core.Entities;
using AmazonScraper.Core.Interfaces;
using AmazonScraper.Infrastructure.Caching;
using AmazonScraper.Shared.Common;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace AmazonScraper.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IScraperService<List<Product>> _searchScraper;
        private readonly IScraperService<Product> _productDetailsScraper;
        private readonly IScraperService<List<ProductOffers>> _productOffersScraper;
        private readonly IDistributedCache _cache;
        private readonly IOptions<AmazonURL> _amazonURL;
        public ProductService(IScraperService<List<Product>> searchScraper, 
            IScraperService<Product> productDetailsScraper,
            IScraperService<List<ProductOffers>> productOffersScraper,
            IDistributedCache cache, IOptions<AmazonURL> amazonURL)
        {
            _searchScraper = searchScraper;
            _productDetailsScraper = productDetailsScraper;
            _productOffersScraper = productOffersScraper;
            _cache = cache;
            _amazonURL = amazonURL;
        }

        public async Task<List<Product>> GetProductsbyPageAsync(string? query, int page = 1, bool refresh = false)
        {
            string cacheKey = query == null ? $"all_products_list:all:" + page : $"all_products_list:" + query + ":" + page;
            var cachedData = await _cache.GetCacheAsync<List<Product>>(cacheKey);

            if (!refresh && cachedData != null)
            {
                return cachedData;
            }

            string? currentUrl = getSearchPageUrl(query, page);
            var productsList = await _searchScraper.ScrapeDataAsync(currentUrl, "SearchPageScraper");

            if (productsList != null)
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

            // when applying strategy pattern
            // 1. will get url here
            string url = getProductDetailsUrl(asin);
            var product = await _productDetailsScraper.ScrapeDataAsync(url, "ProductDetailsPageScraper");

            if (product != null)
                await _cache.SetCacheAsync(cacheKey, product, 360);  // cache for 6 hrs

            return product;
        }

        public async Task<List<ProductOffers>> GetProductOffersAsync(string asin)
        {
            string cacheKey = $"ProductOffers:" + asin;
            var cachedData = await _cache.GetCacheAsync<List<ProductOffers>>(cacheKey);

            if (cachedData != null)
            {
                return cachedData;
            }

            string url = getProductOffersUrl(asin);
            var product = await _productOffersScraper.ScrapeDataAsync(url, "ProductOffersScraper");

            if (product != null)
                await _cache.SetCacheAsync(cacheKey, product, 30);  // cache for 30 min

            return product;
        }

        private string getSearchPageUrl(string? query = null, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(query))
                query = "all";
            string modifiedQuery = query?.Replace(" ", "+");
            string url = _amazonURL.Value.SearchURL + modifiedQuery + "" + "&page=" + page;
            return url;
        }
        private string getProductDetailsUrl(string asin)
        {
            if (string.IsNullOrWhiteSpace(asin))
                return null;
            string url = _amazonURL.Value.ProductDetailsURL + asin;
            return url;
        }
        private string getProductOffersUrl(string asin)
        {
            if (string.IsNullOrWhiteSpace(asin))
                return null;
            string url = _amazonURL.Value.ProductOffersURL + asin;
            return url;
        }
    }
}
