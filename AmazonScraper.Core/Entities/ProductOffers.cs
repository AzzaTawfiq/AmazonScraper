
namespace AmazonScraper.Core.Entities
{
    public class ProductOffers
    {
        public string? ASIN { get; set; }
        public decimal? Price { get; set; }
        public string? Currency { get; set; }
        public string? Condition { get; set; }
        public Seller? Seller { get; set; }
        public Shipping? Shipping { get; set; }
        public string? IsBuyBoxWinner { get; set; }
        public string? IsFullFiledByAmazon { get; set; }
    }

    public class Seller {
        public string? Name { get; set; }
        public string? Rating { get; set; }
        public string? TotalRating { get; set; }
    }

    public class Shipping
    {
        public decimal? Price { get; set; }
        public string? IsFreeShipping { get; set; }
        public string? IsPrime { get; set; }
        public string? EstimatedDelivery { get; set; }
    }
}
