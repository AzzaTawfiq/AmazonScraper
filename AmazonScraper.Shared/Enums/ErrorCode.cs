using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonScraper.Shared.Enums
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
    public enum ErrorCode
    {
        PRODUCT_NOT_FOUND = 0,

    }
}
