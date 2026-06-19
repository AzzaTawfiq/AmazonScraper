
namespace AmazonScraper.Core.Entities
{
    public class Product
    {
        public string? ASIN { get; set; } = string.Empty; //Amazon Standard Identification Number
        public string? Title { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public string? Currency { get; set; } = string.Empty;
        public string? Rating { get; set; } = string.Empty; //Star rating(out of 5)
        public string? TotalReviews { get; set; }
        public string? Image { get; set; } = string.Empty;
        public string? IsPrime { get; set; } = string.Empty; //Prime eligibility
        public string? URL { get; set; } = string.Empty;

        // product details
        public string? Brand { get; set; } = string.Empty;
        public string? Availability { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public List<string>? Features { get; set; }
        public List<string>? Images { get; set; }
        public Dictionary<string, string>? Specifications { get; set; }
        public List<string>? Variants { get; set; }
        public List<string>? Categories { get; set; }
    }
}
