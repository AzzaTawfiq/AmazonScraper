using HtmlAgilityPack;
using AmazonScraper.Core.Interfaces;
using AmazonScraper.Core.Entities;
using System.Globalization;
using AmazonScraper.Shared.Common;
using Microsoft.Extensions.Options;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace AmazonScraper.Infrastructure.Scraper
{
    public class ScraperService : IScraperService
    {
        private readonly IClientHandler _clientHandler;
        private readonly IOptions<AmazonSelectorOptions> _amazonSelectorOptions;
        private readonly IOptions<AmazonURL> _amazonURL;
        public ScraperService(IClientHandler clientHandler, IOptions<AmazonSelectorOptions> amazonSelectorOptions, IOptions<AmazonURL> amazonURL)
        {
            _clientHandler = clientHandler;
            _amazonSelectorOptions = amazonSelectorOptions;
            _amazonURL = amazonURL;
        }

        #region search
        public async Task<List<Product>> ScrapeAmazonProductAsync()
        {
            try
            {
                // 1. get url
                string url = getPageUrl();
                // 2. get data from amazon
                string htmlContent = await _clientHandler.getHTMLData(url);
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
        public async Task<List<Product>> ScrapeAmazonProductsbyPageAsync(string? query, int page = 1)
        {
            string? currentUrl = getPageUrl(query, page);

            var html = await _clientHandler.getHTMLData(currentUrl);

            var document = getHtmlDoc(html);

            var products = extractDataFields(document);

            //await Task.Delay(Random.Shared.Next(3000, 5000));

            return products;
        }
        private string getPageUrl(string? query = null, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(query))
                query = "all";
            string modifiedQuery = query?.Replace(" ", "+");
            string url = _amazonURL.Value.SearchURL + modifiedQuery + "" + "&page=" + page;
            return url;
        }
        private List<Product> extractDataFields(HtmlDocument htmlDoc)
        {
            var products = new List<Product>();

            var productNodes = htmlDoc.DocumentNode
                .SelectNodes(_amazonSelectorOptions.Value.DataComponent);

            if (productNodes == null)
                return null;

            foreach (var node in productNodes)
            {
                (decimal?, string?) priceTuple = GetPrice(node, _amazonSelectorOptions.Value.Price);

                var product = new Product
                {
                    ASIN = node.GetAttributeValue("data-asin", null),
                    Title = GetTitle(node, _amazonSelectorOptions.Value.Title),
                    OriginalPrice = priceTuple.Item1 ?? 0,
                    Price = priceTuple.Item1 ?? 0,
                    Currency = priceTuple.Item2,
                    Rating = GetRating(node, _amazonSelectorOptions.Value.Rating),
                    TotalReviews = GetReviewCount(node, _amazonSelectorOptions.Value.ReviewCount),
                    URL = GetProductUrl(node, _amazonSelectorOptions.Value.ProductURL),
                    Image = GetImageUrl(node, _amazonSelectorOptions.Value.MainProductImageURL),
                    IsPrime = GetIsPrime(node, _amazonSelectorOptions.Value.IsPrime)
                };

                products.Add(product);
            }

            return products;
        }

        private string? GetTitle(HtmlNode node, List<string> selectors)
        {
            var result = ExtractValue(node, selectors);
            if (result != null)
                return result.InnerText?.Trim();

            return null;
        }

        private (decimal?, string?) GetPrice(HtmlNode node, List<string> selectors)
        {
            var result = ExtractValue(node, selectors);
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

            return (0, "");
        }

        private string? GetRating(HtmlNode node, List<string> selectors)
        {
            var result = ExtractValue(node, selectors);
            if (result != null)
                return result.InnerText?.Trim();

            return null;
        }

        private string? GetReviewCount(HtmlNode node, List<string> selectors)
        {
            var result = ExtractValue(node, selectors);
            if (result != null)
                return result.InnerText?.Trim();

            return null;
        }

        private string? GetProductUrl(HtmlNode node, List<string> selectors)
        {
            var result = ExtractValue(node, selectors);
            if (result != null)
            {
                var href = result.GetAttributeValue("href", null);
                return href.StartsWith("http")
                    ? href
                    : $"https://www.amazon.com{href}";
            }

            return null;
        }

        private string? GetImageUrl(HtmlNode node, List<string> selectors)
        {
            var result = ExtractValue(node, selectors);
            if (result != null)
                return result.GetAttributeValue("src", null);

            return null;
        }

        private string? GetIsPrime(HtmlNode node, List<string> selectors)
        {
            var result = ExtractValue(node, selectors);
            if (result != null)
                return result.InnerText?.Trim();

            return null;
        }
        #endregion

        #region general
        private HtmlNode? ExtractValue(HtmlNode node, List<string> selectors)
        {
            foreach (var selector in selectors)
            {
                var result = node.SelectSingleNode(selector);

                if (result != null)
                    return result;
            }
            return null;
        }

        private List<HtmlNode>? ExtractManyValue(HtmlNode node, List<string> selectors)
        {
            foreach (var selector in selectors)
            {
                var result = node.SelectNodes(selector);

                if (result != null)
                    return result.ToList();
            }
            return null;
        }

        private HtmlDocument getHtmlDoc(string htmlContent)
        {
            // 1. Load HTML into HtmlAgilityPack Document
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);
            return htmlDoc;
        }
        #endregion

        #region product details
        public async Task<Product> ScrapeAmazonProductbyASINAsync(string asin)
        {
            try
            {
                // 1. get url
                string url = getProductDetailsUrl(asin);
                // 2. get data from amazon
                string htmlContent = await _clientHandler.getHTMLData(url);
                // 3. get html Document
                var htmlDoc = getHtmlDoc(htmlContent);
                // 3. Extract data fields safely using CSS/XPath selectors
                Product data = extractProductDetailsDataFields(htmlDoc);
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return null;
            }
        }

        private string getProductDetailsUrl(string asin)
        {
            if (string.IsNullOrWhiteSpace(asin))
                return null;
            string url = _amazonURL.Value.ProductDetailsURL + asin;
            return url;
        }

        private Product extractProductDetailsDataFields(HtmlDocument htmlDoc)
        {
            var product = new Product();
            var node = htmlDoc.DocumentNode;

            if (node == null)
                return null;

            (decimal?, string?) priceTuple = GetPrice(node, _amazonSelectorOptions.Value.Price);

            product = new Product
            {
                ASIN = GetAsin(node, _amazonSelectorOptions.Value.ASIN),
                Title = GetTitle(node, _amazonSelectorOptions.Value.Title),
                OriginalPrice = priceTuple.Item1 ?? 0,
                Price = priceTuple.Item1 ?? 0,
                Currency = priceTuple.Item2,
                Rating = GetRating(node, _amazonSelectorOptions.Value.Rating),
                TotalReviews = GetReviewCount(node, _amazonSelectorOptions.Value.ReviewCount),
                URL = GetProductUrl(node, _amazonSelectorOptions.Value.ProductURL),
                Image = GetImageUrl(node, _amazonSelectorOptions.Value.MainProductImageURL),
                IsPrime = GetIsPrime(node, _amazonSelectorOptions.Value.IsPrime),
                Brand = GetBrand(node, _amazonSelectorOptions.Value.brand),
                Availability = GetAvailability(node, _amazonSelectorOptions.Value.Availability),
                Description = GetDescription(node, _amazonSelectorOptions.Value.Description),
                Features = GetFeatureBullets(node, _amazonSelectorOptions.Value.FeatureBullets),
                Categories = GetCategories(node, _amazonSelectorOptions.Value.Categories),
                Images = GetImages(node, _amazonSelectorOptions.Value.AllProductImages),
                Specifications = GetProductSpecifications(node, _amazonSelectorOptions.Value.ProductSpecifications)
            };

            return product;
        }

        private string? GetAsin(HtmlNode node, List<string> selectors)
        {
            var result = ExtractValue(node, selectors);
            if (result != null)
                return result.InnerText?.Trim();

            return null;
        }
        private string? GetBrand(HtmlNode node, List<string> selectors)
        {
            var result = ExtractValue(node, selectors);
            if (result != null)
                return result.InnerText?.Trim();

            return null;
        }
        private string? GetAvailability(HtmlNode node, List<string> selectors)
        {
            var result = ExtractValue(node, selectors);
            if (result != null)
                return result.InnerText?.Trim();

            return null;
        }
        private string? GetDescription(HtmlNode node, List<string> selectors)
        {
            var result = ExtractValue(node, selectors);
            if (result != null)
                return result.InnerText?.Trim();

            return null;
        }
        private List<string>? GetFeatureBullets(HtmlNode node, List<string> selectors)
        {
            var result = ExtractManyValue(node, selectors);
            if (result != null)
                return result?.Select(x => x.InnerText.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            return null;
        }
        private List<string>? GetCategories(HtmlNode node, List<string> selectors)
        {
            var result = ExtractManyValue(node, selectors);
            if (result != null)
                return result?.Select(x => x.InnerText.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            return null;
        }
        private List<string>? GetImages(HtmlNode node, List<string> selectors)
        {
            var result = ExtractManyValue(node, selectors);
            if (result != null)
                return result?.Select(x => x.GetAttributeValue("src", ""))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            return null;
        }
        private Dictionary<string, string>? GetProductSpecifications(HtmlNode node, List<string> selectors)
        {
            var rows = ExtractManyValue(node, selectors);
            var specs = new Dictionary<string, string>();

            if (rows != null)
            {
                foreach (var row in rows)
                {
                    var key = row.SelectSingleNode("./th")?.InnerText.Trim();
                    var value = row.SelectSingleNode("./td")?.InnerText.Trim();

                    if (!string.IsNullOrWhiteSpace(key))
                        specs[key] = value;
                }
            }
            return specs;
        }

        #endregion
    }

}
