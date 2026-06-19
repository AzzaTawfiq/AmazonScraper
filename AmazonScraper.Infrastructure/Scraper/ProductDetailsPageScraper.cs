using AmazonScraper.Core.Entities;
using AmazonScraper.Core.Interfaces;
using AmazonScraper.Shared.Common;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using System.Globalization;
using static AmazonScraper.Core.Interfaces.IScrapingStrategy;

namespace AmazonScraper.Infrastructure.Scraper
{
    public class ProductDetailsPageScraper: IScrapingStrategy<Product>
    {
        public string Name => "ProductDetailsPageScraper";
        private readonly IOptions<AmazonSelectorOptions> _amazonSelectorOptions;
        private readonly IOptions<AmazonURL> _amazonURL;
        private readonly IGenericFieldExtractor _genericFieldExtractor;

        public ProductDetailsPageScraper(IOptions<AmazonSelectorOptions> amazonSelectorOptions,
            IOptions<AmazonURL> amazonURL,
            IGenericFieldExtractor genericFieldExtractor)
        {
            _amazonSelectorOptions = amazonSelectorOptions;
            _amazonURL = amazonURL;
            _genericFieldExtractor = genericFieldExtractor;
        }
        public Product ScrapePage(HtmlDocument htmlDoc)
        {
            var product = new Product();
            var node = htmlDoc.DocumentNode;

            if (node == null)
                return null;

            (decimal?, string?) priceTuple = _genericFieldExtractor.ExtractPrice(node, _amazonSelectorOptions.Value.Price);

            product = new Product
            {
                ASIN = _genericFieldExtractor.ExtractString(node, _amazonSelectorOptions.Value.ASIN),
                Title = _genericFieldExtractor.ExtractString(node, _amazonSelectorOptions.Value.Title),
                OriginalPrice = priceTuple.Item1 ?? 0,
                Price = priceTuple.Item1 ?? 0,
                Currency = priceTuple.Item2,
                Rating = _genericFieldExtractor.ExtractString(node, _amazonSelectorOptions.Value.Rating),
                TotalReviews = _genericFieldExtractor.ExtractString(node, _amazonSelectorOptions.Value.ReviewCount),
                URL = _genericFieldExtractor.ExtractURL(node, _amazonSelectorOptions.Value.ProductURL, _amazonURL.Value.AmazonBaseURL),
                Image = _genericFieldExtractor.ExtractImage(node, _amazonSelectorOptions.Value.MainProductImageURL),
                IsPrime = _genericFieldExtractor.ExtractString(node, _amazonSelectorOptions.Value.IsPrime),
                Brand = _genericFieldExtractor.ExtractString(node, _amazonSelectorOptions.Value.brand),
                Availability = _genericFieldExtractor.ExtractString(node, _amazonSelectorOptions.Value.Availability),
                Description = _genericFieldExtractor.ExtractString(node, _amazonSelectorOptions.Value.Description),
                Features = GetFeatureBullets(node, _amazonSelectorOptions.Value.FeatureBullets),
                Categories = GetCategories(node, _amazonSelectorOptions.Value.Categories),
                Images = GetImages(node, _amazonSelectorOptions.Value.AllProductImages),
                Specifications = GetProductSpecifications(node, _amazonSelectorOptions.Value.ProductSpecifications)
            };

            return product;
        }
        private List<string>? GetFeatureBullets(HtmlNode node, List<string> selectors)
        {
            var result = _genericFieldExtractor.ExtractMany(node, selectors);
            if (result != null)
                return result?.Select(x => x.InnerText.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            return null;
        }
        private List<string>? GetCategories(HtmlNode node, List<string> selectors)
        {
            var result = _genericFieldExtractor.ExtractMany(node, selectors);
            if (result != null)
                return result?.Select(x => x.InnerText.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            return null;
        }
        private List<string>? GetImages(HtmlNode node, List<string> selectors)
        {
            var result = _genericFieldExtractor.ExtractMany(node, selectors);
            if (result != null)
                return result?.Select(x => x.GetAttributeValue("src", ""))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            return null;
        }
        private Dictionary<string, string>? GetProductSpecifications(HtmlNode node, List<string> selectors)
        {
            var rows = _genericFieldExtractor.ExtractMany(node, selectors);
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
    }
}
