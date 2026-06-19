using AmazonScraper.Core.Entities;
using AmazonScraper.Core.Interfaces;
using AmazonScraper.Shared.Common;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using System.Globalization;
using static AmazonScraper.Core.Interfaces.IScrapingStrategy;

namespace AmazonScraper.Infrastructure.Scraper
{
    public class SearchPageScraper : IScrapingStrategy<List<Product>>
    {
        private readonly IOptions<AmazonSelectorOptions> _amazonSelectorOptions;
        private readonly IOptions<AmazonURL> _amazonURL;
        private readonly IGenericFieldExtractor _genericFieldExtractor;

        public SearchPageScraper(IOptions<AmazonSelectorOptions> amazonSelectorOptions,
            IOptions<AmazonURL> amazonURL,
            IGenericFieldExtractor genericFieldExtractor)
        {
            _amazonSelectorOptions = amazonSelectorOptions;
            _amazonURL = amazonURL;
            _genericFieldExtractor = genericFieldExtractor;
        }
        public List<Product> ScrapePage(HtmlDocument htmlDoc)
        {
            var products = new List<Product>();

            var productNodes = htmlDoc.DocumentNode
                .SelectNodes(_amazonSelectorOptions.Value.DataComponent);

            if (productNodes == null)
                return null;

            foreach (var node in productNodes)
            {
                (decimal?, string?) priceTuple = _genericFieldExtractor.ExtractPrice(node, _amazonSelectorOptions.Value.Price);

                var product = new Product
                {
                    ASIN = node.GetAttributeValue("data-asin", null),
                    Title = _genericFieldExtractor.ExtractString(node, _amazonSelectorOptions.Value.Title),
                    OriginalPrice = priceTuple.Item1 ?? 0,
                    Price = priceTuple.Item1 ?? 0,
                    Currency = priceTuple.Item2,
                    Rating = _genericFieldExtractor.ExtractString(node, _amazonSelectorOptions.Value.Rating),
                    TotalReviews = _genericFieldExtractor.ExtractString(node, _amazonSelectorOptions.Value.ReviewCount),
                    URL = _genericFieldExtractor.ExtractURL(node, _amazonSelectorOptions.Value.ProductURL, _amazonURL.Value.AmazonBaseURL),
                    Image = _genericFieldExtractor.ExtractImage(node, _amazonSelectorOptions.Value.MainProductImageURL),
                    IsPrime = _genericFieldExtractor.ExtractString(node, _amazonSelectorOptions.Value.IsPrime)
                };

                products.Add(product);
            }

            return products;
        }
    }
}
