using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using HtmlAgilityPack;
using AmazonScraper.Core.Interfaces;
using static System.Net.Mime.MediaTypeNames;
using AmazonScraper.Core.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.ComponentModel;
using System.Reflection.Metadata;
using System.Globalization;
using AmazonScraper.Shared.Common;
using Microsoft.Extensions.Options;
using AmazonScraper.Shared.DTOs;
using System.Net;
using AmazonScraper.Shared.Exceptions;

namespace AmazonScraper.Infrastructure.Scraper
{
    public class ScraperService : IScraperService
    {
        private static readonly List<string> UserAgents = new()
        {
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.2.1 Safari/605.1.15",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/121.0"
        };
        private readonly IOptions<AmazonSelectorOptions> _amazonSelectorOptions;
        public ScraperService(IOptions<AmazonSelectorOptions> amazonSelectorOptions)
        {
            _amazonSelectorOptions = amazonSelectorOptions;
        }

        public async Task<List<Product>> ScrapeAmazonProductAsync()
        {
            try
            {
                // 1. get url
                string url = getUrl();
                // 2. get data from amazon
                string htmlContent = await getDatafromAmazon(url);
                // 3. get html Document
                var htmlDoc = getHtmlDoc(htmlContent);
                // 3. Extract data fields safely using CSS/XPath selectors
                List<Product> data = extractDataFields(htmlDoc);

                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Product>> ScrapeAmazonProductsAllPagesAsync(string? query, int page = 1)
        {
            var products = new List<Product>();

            string? currentUrl = getSearchUrl(query);

            while (!string.IsNullOrEmpty(currentUrl))
            {
                var html = await getDatafromAmazon(currentUrl);

                var document = getHtmlDoc(html);

                var productsPage = extractDataFields(document);
                if (productsPage?.Count > 0)
                    products.AddRange(productsPage);

                var nextPageNode = document.DocumentNode.SelectSingleNode(
                    "//a[contains(@class,'s-pagination-next') and not(contains(@class,'s-pagination-disabled'))]"
                );

                if (nextPageNode == null)
                    break;

                var href = nextPageNode.GetAttributeValue("href", null);

                if (string.IsNullOrWhiteSpace(href))
                    break;

                currentUrl = href.StartsWith("http")
                    ? href
                    : $"https://www.amazon.com{href}";

                await Task.Delay(Random.Shared.Next(3000, 5000));
            }

            return products;
        }

        public async Task<List<Product>> ScrapeAmazonProductsbyPageAsync(string? query, int page = 1)
        {
            string? currentUrl = getPageUrl(query, page);

            var html = await getDatafromAmazon(currentUrl);

            var document = getHtmlDoc(html);

            var products = extractDataFields(document);

            //await Task.Delay(Random.Shared.Next(3000, 5000));

            return products;
        }
        private string getUrl()
        {
            // Example Amazon Product URL (Replace with your targeted URL)
            string url = "https://www.amazon.com/s?k=all&ref=nb_sb_noss";
            //"https://www.amazon.com/s?i=specialty-aps&bbn=16225007011&rh=n%3A16225007011%2Cn%3A13896617011&ref=nav_em__nav_desktop_sa_intl_computers_tablets_0_2_7_4"; 
            //"https://amazon.com";
            return url;
        }
        private string getSearchUrl(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                query = "all";
            string modifiedQuery = query?.Replace(" ", "+");
            string url = "https://www.Amazon.com/s?k=" + modifiedQuery + "&ref=nb_sb_noss";
            return url;
        }
        private string getPageUrl(string query, int page)
        {
            if (string.IsNullOrWhiteSpace(query))
                query = "all";
            string modifiedQuery = query?.Replace(" ", "+");
            string url = "https://www.Amazon.com/s?k=" + modifiedQuery + "&ref=nb_sb_noss"+ "&page=" + page;
            return url;
        }
        private async Task<string> getDatafromAmazon(string url)
        {
            // 1. Fetch HTML from Amazon
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
            };

            using var client = new HttpClient(handler);

            // 1. Setup Request headers to bypass fundamental bot detection
            var random = new Random();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", UserAgents[random.Next(UserAgents.Count)]);
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            client.DefaultRequestHeaders.Add("Sec-Ch-Ua", "\"Not_A Brand\";v=\"8\", \"Chromium\";v=\"120\", \"Google Chrome\";v=\"120\"");

            var response = await client.GetAsync(url);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    // Proxy was flagged or Amazon served a CAPTCHA challenge
                    throw new AmazonCaptchaException("Access denied by Amazon. Bot protection page served instead of product markup.");
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new ProductNotFoundException("");
                }
            }
            return await response.Content.ReadAsStringAsync();
        }
        private HtmlDocument getHtmlDoc(string htmlContent)
        {
            // 1. Load HTML into HtmlAgilityPack Document
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            return htmlDoc;
        }
        private List<Product> extractDataFields(HtmlDocument htmlDoc)
        {
            var products = new List<Product>();

            var productNodes = htmlDoc.DocumentNode
                .SelectNodes("//*[@data-component-type='s-search-result']");

            if (productNodes == null)
                return null;

            foreach (var node in productNodes)
            {
                (decimal?, string?) priceTuple = GetPrice(node);

                var product = new Product
                {
                    ASIN = node.GetAttributeValue("data-asin", null),
                    Title = GetTitle(node),
                    OriginalPrice = priceTuple.Item1 ?? 0,
                    Price = priceTuple.Item1 ?? 0,
                    Currency = priceTuple.Item2,
                    Rating = GetRating(node),
                    TotalReviews = GetReviewCount(node),
                    URL = GetProductUrl(node),
                    Image = GetImageUrl(node)
                };

                products.Add(product);
            }

            return products;
        }

        private string? GetTitle(HtmlNode node)
        {
            List<string> selectors = _amazonSelectorOptions.Value.Title.ToList();
            foreach (var selector in selectors)
            {
                var result = node.SelectSingleNode(selector);

                if (result != null)
                    return result.InnerText?.Trim();
            }

            return null;
        }

        private (decimal?, string?) GetPrice(HtmlNode node)
        {
            List<string> selectors = _amazonSelectorOptions.Value.Price.ToList();
            foreach (var selector in selectors)
            {
                var result = node.SelectSingleNode(selector);

                if (result != null)
                {
                    string priceString = result.InnerText?.Trim();
                    string currencyOnly = priceString.Substring(0, 3);
                    string priceOnly = priceString.Substring(4, priceString.Length - 4);

                    if (decimal.TryParse(
                            priceOnly,
                            NumberStyles.Any,
                            CultureInfo.InvariantCulture,
                            out var price))
                    {
                        return (price, currencyOnly);
                    }
                }
            }

            return (0, "");
        }

        private string? GetRating(HtmlNode node)
        {
            List<string> selectors = _amazonSelectorOptions.Value.Rating.ToList();
            foreach (var selector in selectors)
            {
                var result = node.SelectSingleNode(selector);

                if (result != null)
                    return result.InnerText?.Trim();
            }

            return null;
        }

        private string? GetReviewCount(HtmlNode node)
        {
            List<string> selectors = _amazonSelectorOptions.Value.ReviewCount.ToList();
            foreach (var selector in selectors)
            {
                var result = node.SelectSingleNode(selector);

                if (result != null)
                    return result.InnerText?.Trim();
            }

            return null;
        }

        private string? GetProductUrl(HtmlNode node)
        {
            List<string> selectors = _amazonSelectorOptions.Value.ProductURL.ToList();
            foreach (var selector in selectors)
            {
                var result = node.SelectSingleNode(selector);

                if (result != null)
                {
                    var href = result.GetAttributeValue("href", null);
                    return href.StartsWith("http")
                        ? href
                        : $"https://www.amazon.com{href}";
                }
            }
            return null;
        }

        private string? GetImageUrl(HtmlNode node)
        {
            //var result = extractValue(node, "MainProductImageURL");
            //if (result != null)
            //    return result.GetAttributeValue("src", null);

            List<string> selectors = _amazonSelectorOptions.Value.MainProductImageURL.ToList();
            foreach (var selector in selectors)
            {
                var result = node.SelectSingleNode(selector);

                if (result != null)
                    return result.GetAttributeValue("src", null);
            }

            return null;
        }

        private HtmlNode? extractValue(HtmlNode node, string fieldName)
        {
            List<string> selectors = _amazonSelectorOptions.Value.MainProductImageURL.ToList();
            foreach (var selector in selectors)
            {
                var result = node.SelectSingleNode(selector);

                if (result != null)
                    return result;
            }
            return null;
        }
    }

}
