
using HtmlAgilityPack;

namespace AmazonScraper.Core.Interfaces
{
    public interface IGenericFieldExtractor
    {
        HtmlNode? Extract(HtmlNode doc, IEnumerable<string> selectors);
        public List<HtmlNode>? ExtractMany(HtmlNode node, IEnumerable<string> selectors);
        string? ExtractString(HtmlNode node, IEnumerable<string> selectors);
        (decimal?, string?) ExtractPrice(HtmlNode node, IEnumerable<string> selectors);
        string? ExtractURL(HtmlNode node, IEnumerable<string> selectors, string baseURL);
        string? ExtractImage(HtmlNode node, IEnumerable<string> selectors);
    }
}
