using AmazonScraper.Core.Entities;
using AmazonScraper.Core.Interfaces;
using AmazonScraper.Shared.Common;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using System.Globalization;
using static AmazonScraper.Core.Interfaces.IScrapingStrategy;

namespace AmazonScraper.Infrastructure.Scraper
{
    public class ProductOffersScraper : IScrapingStrategy<List<ProductOffers>>
    {
        public string Name => "ProductOffersScraper";
        private readonly IOptions<AmazonSelectorOptions> _amazonSelectorOptions;
        private readonly IGenericFieldExtractor _genericFieldExtractor;

        public ProductOffersScraper(IOptions<AmazonSelectorOptions> amazonSelectorOptions,
            IGenericFieldExtractor genericFieldExtractor)
        {
            _amazonSelectorOptions = amazonSelectorOptions;
            _genericFieldExtractor = genericFieldExtractor;
        }
        public List<ProductOffers> ScrapePage(HtmlDocument htmlDoc)
        {
            var productOffers = new List<ProductOffers>();

            var productOfferNodes = htmlDoc.DocumentNode
               .SelectNodes(_amazonSelectorOptions.Value.OfferDataComponent);

            if (productOfferNodes == null)
                return null;

            foreach (var node in productOfferNodes)
            {
                (decimal?, string?) priceTuple = _genericFieldExtractor.ExtractPrice(node, _amazonSelectorOptions.Value.Price);

                var product = new ProductOffers
                {
                    ASIN = _genericFieldExtractor.ExtractString(node, _amazonSelectorOptions.Value.ASIN),
                    Price = priceTuple.Item1 ?? 0,
                    Currency = priceTuple.Item2,
                    Condition = _genericFieldExtractor.ExtractString(node, _amazonSelectorOptions.Value.Availability)
                };

                productOffers.Add(product);
            }

            return productOffers;
        }
    }
}
