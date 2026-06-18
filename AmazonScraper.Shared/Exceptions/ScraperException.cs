using System.Collections.Generic;

namespace AmazonScraper.Shared.Exceptions
{
//    PRODUCT_NOT_FOUND
//SEARCH_NO_RESULTS
//When
//ASIN doesn't exist on Amazon
//Search query returned no results
//SCRAPE_BLOCKED
//SCRAPE_RATE_LIMITED
//Amazon returned CAPTCHA or blocked the request
//Too many requests, hit rate limit
//SCRAPE_TIMEOUT
//INVALID_ASIN
//Request to Amazon timed out
//ASIN format is invalid(not 10 alphanumeric chars)
//INVALID_SEARCH_QUERY
//CACHE_ONLY_AVAILABLE
//Empty or whitespace-only query
//Fresh scrape failed but cached data exists(return cached + this warning)

    // Base exception for scraping failures
    public class AmazonScraperException : Exception
    {
        public AmazonScraperException(string message) : base(message) { }
    }

    // Thrown when Amazon blocks the request via CAPTCHA or bot detection
    public class AmazonCaptchaException : AmazonScraperException
    {
        public AmazonCaptchaException(string message = "Amazon bot detection triggered. CAPTCHA encountered.")
            : base(message) { }
    }

    // Thrown when an item is unavailable or the ASIN is incorrect
    public class ProductNotFoundException : AmazonScraperException
    {
        public ProductNotFoundException(string asin)
            : base($"Product with ASIN '{asin}' was not found or is unavailable on Amazon.") { }
    }

    // Thrown when a proxy fails or drops connection mid-scrape
    public class ProxyRotationException : AmazonScraperException
    {
        public ProxyRotationException(string message) : base(message) { }
    }
}
