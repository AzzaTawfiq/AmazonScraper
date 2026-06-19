using AmazonScraper.Core.Entities;

namespace AmazonScraper.Core.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetProductsbyPageAsync(string? query, int page = 1, bool refresh = false);
        Task<Product> GetProductbyASINAsync(string asin);
        Task<List<ProductOffers>> GetProductOffersAsync(string asin);
    }
}
