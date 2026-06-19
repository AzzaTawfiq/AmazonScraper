
using AmazonScraper.Core.Interfaces;
using HtmlAgilityPack;
using System.Globalization;

namespace AmazonScraper.Infrastructure.Scraper
{
    public class GenericFieldExtractor : IGenericFieldExtractor
    {
        public HtmlNode? Extract(HtmlNode node, IEnumerable<string> selectors)
        {
            foreach (var selector in selectors)
            {
                var result = node
                    .SelectSingleNode(selector);

                if (result != null)
                    return result;
            }

            return null;
        }
        public List<HtmlNode>? ExtractMany(HtmlNode node, IEnumerable<string> selectors)
        {
            foreach (var selector in selectors)
            {
                var result = node
                    .SelectNodes(selector);

                if (result != null)
                    return result.ToList();
            }

            return null;
        }
        public string? ExtractString(HtmlNode node, IEnumerable<string> selectors)
        {
            foreach (var selector in selectors)
            {
                var result = node
                    .SelectSingleNode(selector);

                if (result != null)
                    return result.InnerText.Trim();
            }

            return null;
        }
        public (decimal?, string?) ExtractPrice(HtmlNode node, IEnumerable<string> selectors)
        {
            var result = Extract(node, selectors);
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
        public string? ExtractURL(HtmlNode node, IEnumerable<string> selectors, string baseURL)
        {
            var result = Extract(node, selectors);
            if (result != null)
            {
                var href = result.GetAttributeValue("href", null);
                if (href != null)
                    return href.StartsWith("http")
                        ? href
                        : $"{baseURL}{href}";
            }

            return null;
        }
        public string? ExtractImage(HtmlNode node, IEnumerable<string> selectors)
        {
            var result = Extract(node, selectors);
            if (result != null)
            {
                return result.GetAttributeValue("src", null);
                //if (src != null)
                //{
                //    return src.StartsWith("http")
                //            ? src
                //            : $"https://www.amazon.com{src}";
                //}
            }

            return null;
        }
    }
}
